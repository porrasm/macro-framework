using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    public class TextActivator : CommandActivator {

        #region fields
        private string commandString;
        private RegexWrapper[] matchers;
        #endregion

        public TextActivator(Command.CommandCallback command, params RegexWrapper[] matchers) : base(command) {
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
}