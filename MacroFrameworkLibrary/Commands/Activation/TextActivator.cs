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
    public class TextActivator : CommandActivator {

        #region fields
        private string commandString;
        private RegexWrapper[] matchers;
        #endregion

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

    public class TextActivatorAttribute : ActivatorAttribute {

        #region fields
        private string match;
        private MatchType type;

        public enum MatchType {
            StringMatch,
            RegexPattern
        }
        #endregion

        public TextActivatorAttribute(string match, MatchType type = MatchType.StringMatch) {
            this.match = match;
            this.type = type;
        }

        public override ICommandActivator GetCommandActivator(MethodInfo m) {
            if (type == MatchType.StringMatch) {
                return new TextActivator((s) => m?.Invoke(this, null), match);
            } else {
                return new TextActivator((s) => m?.Invoke(this, null), new RegexWrapper(new Regex(match)));
            }
        }
    }
}