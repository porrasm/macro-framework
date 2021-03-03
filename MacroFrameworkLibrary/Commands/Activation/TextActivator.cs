using MacroFramework.Tools;
using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for text command callbacks
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

        #region constructors
        /// <summary>
        /// Creates a new <see cref="TextActivator"/> instance
        /// </summary>
        /// <param name="command">The command callback</param>
        /// <param name="matchers">Array of <see cref="RegexWrapper"/> objects which are used to match text commands</param>
        public TextActivator(Command.CommandCallback command, params RegexWrapper[] matchers) : base(command) {
            Init(matchers);
        }

        /// <summary>
        /// Creates a new <see cref="TextActivator"/> instance
        /// </summary>
        /// <param name="command">The text command callback</param>
        /// <param name="matchers">Array of <see cref="RegexWrapper"/> objects which are used to match text commands</param>
        public TextActivator(TextCommandCallback command, params RegexWrapper[] matchers) : base(WrapTextCommand(command)) {
            Init(matchers);
        }

        /// <summary>
        /// Creates a new <see cref="TextActivator"/> instance
        /// </summary>
        /// <param name="command">The command callback</param>
        /// <param name="matchers">Array of <see cref="RegexWrapper"/> objects which are used to match text commands</param>
        public TextActivator(params RegexWrapper[] matchers) : base(null) {
            Init(matchers);
        }
        #endregion

        private void Init(params RegexWrapper[] matchers) {
            if (matchers == null || matchers.Length == 0) {
                throw new Exception("Matchers cannot be null or empty");
            }
            this.matchers = matchers;
        }
        private static Command.CommandCallback WrapTextCommand(TextCommandCallback cb) {
            return () => cb?.Invoke(TextCommandCreator.CurrentTextCommand);
        }

        protected override bool IsActivatorActive() {
            foreach (RegexWrapper m in matchers) {
                if (TextCommands.IsMatchForCommand(m)) {
                    return true;
                }
            }

            return false;
        }
    }
}