using System;
using System.Text;
using System.Text.RegularExpressions;

namespace NBasis
{
    public static class StringExtensions
    {
        private static Regex EmailRegex = new Regex(@"^(([^<>()[\]\\.,;:\s@\""]+"
                                                    + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                                    + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                                    + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                                    + @"[a-zA-Z]{2,}))$", RegexOptions.Compiled);

        /// <summary>
        /// Is the string a valid email
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidEmail(this string input)
        {
            if (input == null) return false;
            return EmailRegex.IsMatch(input);
        }

        /// <summary>
        /// A nicer way of calling <see cref="System.String.IsNullOrEmpty()"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string input)
        {
            return String.IsNullOrEmpty(input);
        }

        /// <summary>
        /// A nicer way of calling <see cref="System.String.IsNullOrWhiteSpace()"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string input)
        {
            return String.IsNullOrWhiteSpace(input);
        }

        /// <summary>
        /// A nicer way of calling <see cref="System.String.Format(string, object[])"/>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// Trim and convert to lower case
        /// </summary>
        /// <param name="input">Any string including nulls</param>
        /// <returns></returns>
        public static string TrimAndLower(this String input)
        {
            if (String.IsNullOrEmpty(input)) return input;
            return input.Trim().ToLower();
        }

        public static string ToBase64(this string input)
        {
            return Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(input.Or("")));
        }

        public static string FromBase64(this string input)
        {
            return UTF8Encoding.UTF8.GetString(Convert.FromBase64String(input.Or("")));
        }

        /// <summary>
        /// Make the first character upper case
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Capitalize(this string input)
        {
            if (input.IsNullOrWhiteSpace())
                return input;
            if (input.Length == 1)
                return input.ToUpper();
            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }
    }
}
