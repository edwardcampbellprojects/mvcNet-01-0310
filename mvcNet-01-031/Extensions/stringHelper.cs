using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

/// <summary>
/// iterDev.CS.Extensions: some useful string extensions
/// </summary>
namespace iterDev.CS.Extensions
{
    public static class stringHelper
    {
        /// <summary>
        ///  Convert the string to camel case.
        /// </summary>
        /// <param name="the_string"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null || the_string.Length < 2)
                return the_string;

            // Split the string into words.
            string[] words = the_string.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result += words[i].Substring(0, 1).ToUpper() + words[i].Substring(1);
            }
            return result;
        }

        /// <summary>
        /// Convert the string to Pascal case.
        /// </summary>
        /// <param name="the_string"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null) return the_string;
            if (the_string.Length < 2) return the_string.ToUpper();

            // Split the string into words.
            string[] words = the_string.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result += word.Substring(0, 1).ToUpper() + word.Substring(1);
            }
            return result;
        }

        /// <summary>
        /// Capitalize the first character and add a space before each capitalized letter, except the first character to allow for acronyms
        /// </summary>
        /// <param name="the_string"></param>
        /// <returns>Proper or Title Case String</returns>
        public static string ToProperCase(this string the_string)
        {
            char lastChar = ' ';

            // If there are 0 or 1 characters, just return the string.
            if (the_string == null) return the_string;
            if (the_string.Length < 2) return the_string.ToUpper();

            // Start with the first character.
            string result = the_string.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (int i = 1; i < the_string.Length; i++)
            {
                if (char.IsUpper(the_string[i]) || (char.IsNumber(the_string[i]) && !char.IsNumber(lastChar)))
                    result += " ";

                lastChar = the_string[i];
                result += the_string[i];
            }
            return result;
        }

        /// <summary>
        /// To the title case.
        /// </summary>
        /// <param name="the_string">The string.</param>
        /// <returns>the_string in Title Case</returns>
        public static string ToTitleCase(this string the_string)
        {
            return the_string.ToPascalCase().ToProperCase();
        }

        //----------------------------------------------------------------------------------

        /// <summary>
        /// Removes the last character.
        /// http://www.danylkoweb.com/Blog/10-extremely-useful-net-extension-methods-8J
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <returns>System.String.</returns>
        public static string RemoveLastCharacter(this String instr)
        {
            return instr.Substring(0, instr.Length - 1);
        }

        /// <summary>
        /// Removes the last.
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <param name="number">The number.</param>
        /// <returns>System.String.</returns>
        public static string RemoveLast(this String instr, int number)
        {
            return instr.Substring(0, instr.Length - number);
        }

        /// <summary>
        /// Removes the first character.
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <returns>System.String.</returns>
        public static string RemoveFirstCharacter(this String instr)
        {
            return instr.Substring(1);
        }

        /// <summary>
        /// Removes the first.
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <param name="number">The number.</param>
        /// <returns>System.String.</returns>
        public static string RemoveFirst(this String instr, int number)
        {
            return instr.Substring(number);
        }
        //end http://www.danylkoweb.com/Blog/10-extremely-useful-net-extension-methods-8J

        /// <summary>
        /// Contains: adding a Contains() extension method to string that checks case-insensitively.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="comparer"></param>
        /// <returns>boolean indicating whether source contains target string</returns>
        public static bool Contains(this string source, string target, StringComparison comparer)
        {
            // good extension methods should not accept null (unless the name implies it can)
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return source.IndexOf(target, comparer) != -1;
        }

        /// <summary>
        /// EqualsIgnoreCase - case insensitive extension of string.Equals
        /// </summary>
        /// <param name="source">Method being extended</param>
        /// <param name="target">string to determine case-insensitive equality to</param>
        /// <returns>boolean indicating equality of source and target strings</returns>
        public static bool EqualsIgnoreCase(this string source, string target)
        {
            return string.Equals(source, target, StringComparison.InvariantCultureIgnoreCase);
        }

        #region Misc String Extensions
        /// <summary>
        /// This region contain extension functions for string objects
        /// </summary>
        /// example
        //var myEnum = "Pending".ToEnum<Status>();
        //var left = "Faraz Masood Khan".Left(100000);// works;
        //var right = "CoreSystem Library".Right(10);
        //var formatted = "My name is {0}.".Format("Faraz");
        //var result = "CoreSystem".In("CoreSystem", "Library");
        /// <summary>
        /// Checks string object's value to array of string values
        /// </summary>        
        /// <param name="stringValues">Array of string values to compare</param>
        /// <returns>Return true if any string value matches</returns>
        public static bool In(this string value, params string[] stringValues)
        {
            foreach (string otherValue in stringValues)
                if (string.Compare(value, otherValue) == 0)
                    return true;

            return false;
        }

        /// <summary>
        /// Converts string to enum object
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="value">String value to convert</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value)
            where T : struct
        {
            return (T)System.Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Returns characters from right of specified length
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="length">Max number of charaters to return</param>
        /// <returns>Returns string from right</returns>
        public static string Right(this string value, int length)
        {
            return value != null && value.Length > length ? value.Substring(value.Length - length) : value;
        }

        /// <summary>
        /// Returns characters from left of specified length
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="length">Max number of charaters to return</param>
        /// <returns>Returns string from left</returns>
        public static string Left(this string value, int length)
        {
            return value != null && value.Length > length ? value.Substring(0, length) : value;
        }

        /// <summary>
        ///  Replaces the format item in a specified System.String with the text equivalent
        ///  of the value of a specified System.Object instance.
        /// </summary>
        /// <param name="value">A composite format string</param>
        /// <param name="arg0">An System.Object to format</param>
        /// <returns>A copy of format in which the first format item has been replaced by the
        /// System.String equivalent of arg0</returns>
        public static string Format(this string value, object arg0)
        {
            return string.Format(value, arg0);
        }

        /// <summary>
        ///  Replaces the format item in a specified System.String with the text equivalent
        ///  of the value of a specified System.Object instance.
        /// </summary>
        /// <param name="value">A composite format string</param>
        /// <param name="args">An System.Object array containing zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the System.String
        /// equivalent of the corresponding instances of System.Object in args.</returns>
        public static string Format(this string value, params object[] args)
        {
            return string.Format(value, args);
        }
        #endregion

        #region GetHash

        /// <summary>
        /// Supported hash algorithms
        /// </summary>
        public enum eHashType
        {
            HMAC, HMACMD5, HMACSHA1, HMACSHA256, HMACSHA384, HMACSHA512,
            MACTripleDES, MD5, RIPEMD160, SHA1, SHA256, SHA384, SHA512
        }

        private static byte[] GetHash(string input, eHashType hash)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);

            switch (hash)
            {
                case eHashType.HMAC:
                    return HMAC.Create().ComputeHash(inputBytes);

                case eHashType.HMACMD5:
                    return HMACMD5.Create().ComputeHash(inputBytes);

                case eHashType.HMACSHA1:
                    return HMACSHA1.Create().ComputeHash(inputBytes);

                case eHashType.HMACSHA256:
                    return HMACSHA256.Create().ComputeHash(inputBytes);

                case eHashType.HMACSHA384:
                    return HMACSHA384.Create().ComputeHash(inputBytes);

                case eHashType.HMACSHA512:
                    return HMACSHA512.Create().ComputeHash(inputBytes);

                case eHashType.MACTripleDES:
                    return MACTripleDES.Create().ComputeHash(inputBytes);

                case eHashType.MD5:
                    return MD5.Create().ComputeHash(inputBytes);

                case eHashType.RIPEMD160:
                    return RIPEMD160.Create().ComputeHash(inputBytes);

                case eHashType.SHA1:
                    return SHA1.Create().ComputeHash(inputBytes);

                case eHashType.SHA256:
                    return SHA256.Create().ComputeHash(inputBytes);

                case eHashType.SHA384:
                    return SHA384.Create().ComputeHash(inputBytes);

                case eHashType.SHA512:
                    return SHA512.Create().ComputeHash(inputBytes);

                default:
                    return inputBytes;
            }
        }

        /// <summary>
        /// Computes the hash of the string using a specified hash algorithm
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <param name="hashType">The hash algorithm to use</param>
        /// <returns>The resulting hash or an empty string on error</returns>
        public static string ComputeHash(this string input, eHashType hashType)
        {
            try
            {
                byte[] hash = GetHash(input, hashType);
                StringBuilder ret = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                    ret.Append(hash[i].ToString("x2"));

                return ret.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
    }
}