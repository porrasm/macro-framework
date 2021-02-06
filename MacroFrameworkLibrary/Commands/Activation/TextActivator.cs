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
        /// <summary>
        /// Callback for text commands
        /// </summary>
        /// <param name="command">The command which is being executed</param>
        public delegate void TextCommandCallback(string command);

        private RegexWrapper[] matchers;
        #endregion

        /// <summary>
        /// Creates a new text command activator
        /// </summary>
        /// <param name="command">The command callback</param>
        /// <param name="matchers">Array of <see cref="RegexWrapper"/> objects</param>
        public TextActivator(Command.CommandCallback command, params RegexWrapper[] matchers) : base(command) {
            Init(matchers);
        }

        /// <summary>
        /// Creates a new text command activator
        /// </summary>
        /// <param name="command">The command callback</param>
        /// <param name="matchers">Array of <see cref="RegexWrapper"/> objects</param>
        public TextActivator(TextCommandCallback command, params RegexWrapper[] matchers) : base(WrapTextCommand(command)) {
            Init(matchers);
        }

        private void Init( params RegexWrapper[] matchers) {
            if (matchers == null || matchers.Length == 0) {
                throw new Exception("Matchers cannot be null or empty");
            }
            this.matchers = matchers;
        }
        private static Command.CommandCallback WrapTextCommand(TextCommandCallback cb) {
            return () => cb?.Invoke(TextCommandCreator.CurrentTextCommand);
        }

        public override bool IsActive() {
            foreach (RegexWrapper m in matchers) {
                if (TextCommands.IsMatchForCommand(m)) {
                    return true;
                }
            }

            return false;
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