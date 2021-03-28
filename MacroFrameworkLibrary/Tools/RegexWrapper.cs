using System.Text.RegularExpressions;

namespace MacroFramework.Tools {
    /// <summary>
    /// A simple wrapper class for Regexes. You can implicitly create a RegexWrapper instance from a string or a Regex.
    /// </summary>
    public class RegexWrapper {

        private string stringMatch;
        private bool useRegex;
        private Regex regex;

        private RegexWrapper() {

        }
        public RegexWrapper(Regex regex) {
            useRegex = true;
            this.regex = regex;
        }

        public bool IsMatch(string s) {
            if (useRegex) {
                return regex.IsMatch(s);
            } else {
                return stringMatch.Equals(s);
            }
        }

        public static implicit operator RegexWrapper(string s) {
            RegexWrapper r = new RegexWrapper();
            r.useRegex = false;
            r.stringMatch = s.ToLower();
            return r;
        }
        public static implicit operator RegexWrapper(Regex r) {
            return new RegexWrapper(r);
        }

        /// <summary>
        /// Syntactit sugar for array creation
        /// </summary>
        /// <param name="wrappers"></param>
        public static RegexWrapper[] Wrap(params RegexWrapper[] wrappers) {
            return wrappers;
        }
    }
}
