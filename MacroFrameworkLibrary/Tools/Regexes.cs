using System;
using System.Text.RegularExpressions;

namespace MacroFramework.Tools {
    public static class Regexes {
        public const string AlphanumericChar = "[a-zA-Z0-9_]";
        public const string AlphanumericString = "[a-zA-Z0-9_]+";

        #region integer parameter
        public static RegexWrapper IntegerParameters(string command, int paramCount = 0) {
            string intRegex = "( +-?[0-9]+)";
            string amount = paramCount == 0 ? "+" : "{" + paramCount + "}";

            string regex = "^" + command + intRegex + amount + " *$";
            Logger.Log("reg len. " + regex.Length);
            return new Regex(regex);
        }

        public static int[] GetIntegerParameters(string s, bool verify = false) {

            if (!s.Contains(" ")) {
                return new int[0];
            }

            if (verify && !Regexes.IntegerParameters(Regexes.AlphanumericString).IsMatch(s)) {
                return new int[0];
            }

            string[] split = s.Split(' ');

            int index = 0;

            for (int i = 1; i < split.Length; i++) {
                if (split[i].Length > 0) {
                    index++;
                }
            }

            int[] parameters = new int[index];

            index = 0;

            for (int i = 1; i < split.Length; i++) {
                if (split[i].Length > 0) {
                    parameters[index] = int.Parse(split[i]);
                    index++;
                }
            }

            return parameters;
        }
        #endregion

        public static RegexWrapper StartsWith(string command) {
            return new Regex("^(" + command + "|" + command + " .*)$");
        }

        #region alphanumeric string parameters
        public static RegexWrapper AlphaNumericStringParameters(string command, int paramCount = 0) {
            string stringRegex = "( +" + AlphanumericString + ")";
            string amount = paramCount == 0 ? "+" : "{" + paramCount + "}";

            string regex = "^" + command + stringRegex + amount + " *$";
            return new Regex(regex);
        }

        public static RegexWrapper StringParameter(string command, bool requireParameter = true) {

            if (!requireParameter) {
                return StartsWith(command);
            }

            string stringRegex = @" +\S.+";

            string regex = "^" + command + stringRegex + "$";
            return new Regex(regex);
        }

        public static string GetStringParameter(string s, bool verify = false) {

            if (!s.Contains(" ")) {
                return null;
            }

            if (verify && !StringParameter(AlphanumericString).IsMatch(s)) {
                return null;
            }
            return s.Substring(s.IndexOf(' ') + 1);
        }
        public static string GetStringParameter(string s, bool verify, int paramIndex) {

            if (WordCount(s) < paramIndex) {
                return null;
            }

            string substring = s;
            for (int i = 0; i < paramIndex + 1; i++) {
                int index = substring.IndexOf(" ") + 1;
                if (index == 0) {
                    return null;
                }
                substring = substring.Substring(index);
            }

            return substring;
        }

        public static string[] GetAlphanumericStringParameters(string s, bool verify = false) {

            if (!s.Contains(" ")) {
                return new string[0];
            }

            if (verify && !IntegerParameters(Regexes.AlphanumericString).IsMatch(s)) {
                return new string[0];
            }

            string[] split = s.Split(' ');

            int index = 0;

            for (int i = 1; i < split.Length; i++) {
                if (split[i].Length > 0) {
                    index++;
                }
            }

            string[] parameters = new string[index];

            index = 0;

            for (int i = 1; i < split.Length; i++) {
                if (split[i].Length > 0) {
                    parameters[index] = split[i];
                    index++;
                }
            }

            return parameters;
        }
        #endregion

        #region helpers
        private static int WordCount(string text) {
            int wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;

            while (index < text.Length) {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;
            }

            return wordCount;
        }

        private static int IndexOfNth(this string str, string value, int nth = 1) {
            if (nth <= 0)
                throw new ArgumentException("Can not find the zeroth index of substring in string. Must start with 1");
            int offset = str.IndexOf(value);
            for (int i = 1; i < nth; i++) {
                if (offset == -1) return -1;
                offset = str.IndexOf(value, offset + 1);
            }
            return offset;
        }
        #endregion
    }
}
