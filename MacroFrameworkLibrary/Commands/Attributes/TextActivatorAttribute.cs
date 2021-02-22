using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="Attribute"/> for easily creating a <see cref="TextActivator"/>. Attribute methods are parameterless, use <see cref="Commands.TextCommands.CurrentTextCommand"/> to get the current text command.
    /// </summary>
    public class TextActivatorAttribute : ActivatorAttribute {

        #region fields
        private string[] matches;
        private MatchType type;

        /// <summary>
        /// Determines whether to use the matcher string as a regex or a string match
        /// </summary>
        public enum MatchType {
            /// <summary>
            /// Use string match
            /// </summary>
            StringMatch,
            /// <summary>
            /// Use regex match
            /// </summary>
            RegexPattern
        }
        #endregion

        /// <summary>
        /// Creates a new <see cref="TextActivator"/> instance using either string or regex matching at the start of the application from this method
        /// </summary>
        /// <param name="match">The exact string match or regex pattern</param>
        /// <param name="type"><see cref="MatchType"/></param>
        public TextActivatorAttribute(string match, MatchType type = MatchType.StringMatch) {
            this.matches = new string[] { match };
            this.type = type;
        }

        /// <summary>
        /// Creates a new <see cref="TextActivator"/> instance using string matching at the start of the application from this method
        /// </summary>
        /// <param name="match">The exact string match or regex pattern</param>
        /// <param name="type"><see cref="MatchType"/></param>
        public TextActivatorAttribute(params string[] wrappers) {
            this.matches = wrappers;
            this.type = MatchType.StringMatch;
        }

        public override IActivator GetCommandActivator(Command command, MethodInfo assignedMethod) {
            return new TextActivator((s) => assignedMethod?.Invoke(command, null), GetRegexWrappers());
        }

        private RegexWrapper[] GetRegexWrappers() {
            RegexWrapper[] newWrappers = new RegexWrapper[matches.Length];
            if (type == MatchType.RegexPattern) {
                for (int i = 0; i < matches.Length; i++) {
                    newWrappers[i] = new Regex(matches[i]);
                }
            } else if (type == MatchType.StringMatch) {
                for (int i = 0; i < matches.Length; i++) {
                    newWrappers[i] = matches[i];
                }
            }
            return newWrappers;
        }
    }
}
