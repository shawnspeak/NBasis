using System;
using System.Collections.Specialized;

namespace NBasis
{
    public static class UriExtensions
    {
        public static NameValueCollection ParseQueryString(this UriBuilder builder)
        {
            NameValueCollection nvc = new NameValueCollection();
            String s = (builder.Query ?? String.Empty).TrimStart('?');
            if (String.IsNullOrWhiteSpace(s)) return nvc;  // do nothing

            int num = (s != null) ? s.Length : 0;
            for (int i = 0; i < num; i++)
            {
                int num2 = i;
                int num3 = -1;
                while (i < num)
                {
                    char c = s[i];
                    if (c == '=')
                    {
                        if (num3 < 0)
                        {
                            num3 = i;
                        }
                    }
                    else
                    {
                        if (c == '&')
                        {
                            break;
                        }
                    }
                    i++;
                }
                string text = null;
                string text2;
                if (num3 >= 0)
                {
                    text = s.Substring(num2, num3 - num2);
                    text2 = s.Substring(num3 + 1, i - num3 - 1);
                }
                else
                {
                    text2 = s.Substring(num2, i - num2);
                }
                if (true)
                {
                    nvc.Add(Uri.UnescapeDataString(text), Uri.UnescapeDataString(text2));
                }                
                if (i == num - 1 && s[i] == '&')
                {
                    nvc.Add(null, string.Empty);
                }
            }

            return nvc;
        }

        public static void AddToQuery(this UriBuilder builder, String name, UriBuilder uri)
        {
            builder.AddToQuery(name, uri.Uri.AbsoluteUri);
        }

        public static void AddToQuery(this UriBuilder builder, String name, String value)
        {
            if ((String.IsNullOrWhiteSpace(name)) ||
                (String.IsNullOrWhiteSpace(value)))
                throw new ArgumentNullException("Name or value is missing");

            String query = "";
            NameValueCollection nvc = builder.ParseQueryString();
            foreach (var key in nvc.AllKeys)
            {
                if (!key.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    query += String.Format("{0}={1}&", Uri.EscapeDataString(key), Uri.EscapeDataString(nvc[key]));
                }
            }
            query += String.Format("{0}={1}", Uri.EscapeDataString(name), Uri.EscapeDataString(value));
            builder.Query = query;
        }

        public static void AddToQuery(this UriBuilder builder, String name, Guid id)
        {
            builder.AddToQuery(name, id.ToString("N"));
        }

        public static void RemoveFromQuery(this UriBuilder builder, String name)
        {
            String query = String.Empty;
            NameValueCollection nvc = builder.ParseQueryString();
            foreach (var key in nvc.AllKeys)
            {
                if (!key.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    query += String.Format("{0}={1}&", Uri.EscapeDataString(key), Uri.EscapeDataString(nvc[key]));
                }
            }
            builder.Query = query.TrimEnd('&');
        }
    }
}
