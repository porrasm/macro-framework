using MacroFramework.Tools;
using System.Text.RegularExpressions;

namespace MacroFramework.Commands {
    /// <summary>
    /// Settings class for <see cref="TextActivator"/>
    /// </summary>
    public class Matchers {
        #region fields
        /// <summary>
        /// The matchers to use
        /// </summary>
        public RegexWrapper[] TextMatchers { get; set; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="Matchers"/> instance
        /// </summary>
        /// <param name="matchers">Matchers to use</param>
        public Matchers(params RegexWrapper[] matchers) {
            this.TextMatchers = matchers;
        }

        /// <summary>
        /// Implicit operator for <see cref="string"/>
        /// </summary>
        public static implicit operator Matchers(string s) {
            return new Matchers(s);
        }

        /// <summary>
        /// Implicit operator for <see cref="Regex"/>
        /// </summary>
        public static implicit operator Matchers(Regex r) {
            return new Matchers(r);
        }
    }
}
