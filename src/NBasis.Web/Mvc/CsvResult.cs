using NBasis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NBasis.Web.Mvc
{
    public static class CsvExtensions
    {
        public static void WriteCsvField(this StreamWriter writer, String value, bool includeComma = true)
        {
            if (includeComma)
                writer.Write(",");

            if (!String.IsNullOrWhiteSpace(value))
            {
                // cleanup value
                bool surround = (value.IndexOfAny(new char[] { ',', '\"' }) >= 0);
                value = CleanUp(value);

                if (surround)
                {
                    // output in quotes if needed
                    value = "\"" + value + "\"";
                }

                // write value
                writer.Write(value);
            }
        }

        internal static String CleanUp(String input)
        {
            if (String.IsNullOrEmpty(input))
                return String.Empty;
            // remove lines and double up the quotes
            return input.Replace("\r\n", " ").Replace("\n", " ").Replace("\"", "\"\"");
        }
    }

    public class QueryToCsvResult<TRecord> : ActionResult
    {
        public static int DefaultPageSize = 25;

        private readonly IQueryable<TRecord> _Query;
        private readonly Func<TRecord, object> _SelectMap;

        public QueryToCsvResult(IQueryable<TRecord> query, Func<TRecord, object> selectMap)
        {
            _Query = query;
            _SelectMap = selectMap;

            ShowHeader = true;
            PageSize = DefaultPageSize;
            HeaderMap = (input) => { return input; };
        }

        public bool ShowHeader { get; set; }

        public Func<String, String> HeaderMap { get; set; }

        public String Filename { get; set; }

        public int PageSize { get; set; }

        public IDisposable ToDispose { get; set; }

        #region Output Anonymous objects

        private int _RowCount = 0;
        private List<PropertyInfo> _Props = new List<PropertyInfo>();

        private void OutputAnonymous(StreamWriter sw, object map)
        {
            int i = 0;
            if (_Props.Count == 0)
            {
                // get the types and output header row
                Type mapType = map.GetType();
                mapType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ForEach((mapProp) =>
                {
                    if (ShowHeader)
                        sw.WriteCsvField(HeaderMap(mapProp.Name), i++ > 0);
                    _Props.Add(mapProp);
                });

                if (ShowHeader)
                    sw.Write("\r\n");
            }

            i = 0;
            _Props.ForEach((prop) =>
            {
                sw.WriteCsvField(Convert.ToString(prop.GetValue(map)), i++ > 0);
            });
            sw.Write("\r\n");
            _RowCount++;
        }

        #endregion

        #region Output Anonymous objects

        private void OutputDictionary(StreamWriter sw, IDictionary<string, object> map)
        {
            int i = 0;
            if (_RowCount++ == 0)
            {
                // get the types and output header row
                map.Keys.ForEach((key) =>
                {
                    if (ShowHeader)
                        sw.WriteCsvField(HeaderMap(key), i++ > 0);
                });

                if (ShowHeader)
                    sw.Write("\r\n");
            }

            i = 0;
            map.Keys.ForEach((key) =>
            {
                sw.WriteCsvField(Convert.ToString(map[key]), i++ > 0);
            });
            sw.Write("\r\n");
        }

        #endregion

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Clear();
            response.Buffer = false;
            response.ContentType = "text/csv";
            response.ContentEncoding = Encoding.UTF8;
            response.AppendHeader("Content-Disposition", "attachment; filename=\"{0}\"".FormatWith(Filename.Or(DateTime.UtcNow.ToString("_yyyy_MM_dd") + ".csv")));

            _Props.Clear();

            using (var sw = new StreamWriter(response.OutputStream, new System.Text.UTF8Encoding(false), 4096, true))
            {
                // paged query
                int skip = 0;
                int read = _Query.Count();
                while (read > 0)
                {
                    // take a page
                    var page = _Query.Skip(skip).Take(PageSize).ToList();
                    read = page.Count();
                    skip += read;

                    if (read > 0)
                    {
                        // write csv for the page
                        page.ForEach((row) =>
                        {
                            var map = _SelectMap(row);
                            if (map != null)
                            {
                                if (map is IDictionary<String, object>)
                                {
                                    OutputDictionary(sw, map as IDictionary<String, object>);
                                }
                                else
                                {
                                    OutputAnonymous(sw, map);
                                }
                            }
                        });
                    }
                }
            }

            if (ToDispose != null)
                ToDispose.Dispose();
        }
    }
}
