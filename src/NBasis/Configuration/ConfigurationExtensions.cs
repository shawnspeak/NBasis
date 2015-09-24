using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NBasis.Configuration
{
    public static class ConfigurationSetting
    {
        public static String Get(String key)
        {
            return Get(key, null);
        }

        public static String Get(String key, String defaultValue)
        {
            try
            {
                return ConfigurationManager.AppSettings[key] ?? defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            int val;
            return Int32.TryParse(Get(key), out val) ? val : defaultValue;
        }

        static List<String> _Truths = new List<String>(new String[] { "true", "1", "yes" });
        public static bool GetBool(string key, bool defaultValue)
        {
            return _Truths.Contains(Get(key, defaultValue.ToString()).ToLower());
        }

        public static string GetConnectionString(string key, string defaultValue = "")
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[key].Value(x => x.ConnectionString) ?? defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
