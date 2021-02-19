using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MCC.Utility.Text
{
    public static class StringExtensions
    {
        /// <summary>
        /// 特定のテキストを正規表現で抜き出します。
        /// </summary>
        /// <param name="text">対象の文字列</param>
        /// <param name="regex">正規表現</param>
        /// <param name="param">パラメーター</param>
        /// <returns></returns>
        public static string RegexString(this string text, string regex, string param)
        {
            var mc = Regex.Matches(text, regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match m in mc)
                return m.Groups[param].Value;

            return string.Empty;
        }

        /// <summary>
        /// 特定のテキストを正規表現で複数抜き出します。
        /// </summary>
        /// <param name="text">対象の文字列</param>
        /// <param name="regex">正規表現</param>
        /// <param name="param">パラメーター</param>
        /// <returns></returns>
        public static string[] RegexStrings(this string text, string regex, string param)
        {
            var mc = Regex.Matches(text, regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var value = new List<string>();

            foreach (Match m in mc)
                value.Add(m.Groups[param].Value);

            return value.ToArray();
        }

        /// <summary>
        /// 特定のテキストを正規表現で複数抜き出します。
        /// </summary>
        /// <param name="text">対象の文字列</param>
        /// <param name="regex">正規表現</param>
        /// <param name="param">パラメーター</param>
        /// <returns></returns>
        public static string RegexMatch(this string text, string regex)
        {
            var mc = Regex.Matches(text, regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match m in mc)
                return m.Value;

            return string.Empty;
        }
    }
}
