using MacroFramework.Commands.Activation;
using MacroFramework.Commands.Attributes;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// Text command activator
    /// </summary>
    public class TextActivator : CommandActivator {

        #region fields
        private string commandString;
        private RegexWrapper[] matchers;
        #endregion

        /// <summary>
        /// Creates a new text command activator
        /// </summary>
        /// <param name="command">The command callback</param>
        /// <param name="matchers">Array of <see cref="RegexWrapper"/> objects</param>
        public TextActivator(Command.TextCommandCallback command, params RegexWrapper[] matchers) : base(command) {
            if (matchers == null || matchers.Length == 0) {
                throw new Exception("Matchers cannot be null or empty");
            }
            this.matchers = matchers;
        }

        public override bool IsActive() {
            foreach (RegexWrapper m in matchers) {
                if (TextCommandCreator.CollectCommand(m, out commandString)) {
                    return true;
                }
            }

            return false;
        }

        protected override void ExecuteCallback() {
            command?.Invoke(commandString);
        }
    }

    /// <summary>
    /// Attribute activator for <see cref="TextActivator"/>
    /// </summary>
    public class TextActivatorAttribute : ActivatorAttribute {

        #region fields
        private string match;
        private MatchType type;

        public enum MatchType {
            StringMatch,
            RegexPattern
        }
        #endregion

        /// <summary>
        /// Creates a new text command activator
        /// </summary>
        /// <param name="match">The exact string match or regex pattern</param>
        /// <param name="type"></param>
        public TextActivatorAttribute(string match, MatchType type = MatchType.StringMatch) {
            this.match = match;
            this.type = type;
        }

        public override ICommandActivator GetCommandActivator(Command c, MethodInfo m) {
            if (type == MatchType.StringMatch) {
                return new TextActivator((s) => m?.Invoke(c, null), match);
            } else {
                return new TextActivator((s) => m?.Invoke(c, null), new RegexWrapper(new Regex(match)));
            }
        }
    }
}