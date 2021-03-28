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

        /// <summary>
        /// The settings to use
        /// </summary>
        public Matchers Settings { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new <see cref="TextActivator"/> instance
        /// </summary>
        /// <param name="command">The command callback</param>
        /// <param name="settings">The settings to use</param>
        public TextActivator(Matchers settings, Command.CommandCallback command) : base(command) {
            this.Settings = settings;
        }

        /// <summary>
        /// Creates a new <see cref="TextActivator"/> instance
        /// </summary>
        /// <param name="command">The text command callback</param>
        /// <param name="settings">The settings to use</param>
        public TextActivator(Matchers settings, TextCommandCallback command) : base(WrapTextCommand(command)) {
            this.Settings = settings;
        }

        /// <summary>
        /// Creates a new <see cref="TextActivator"/> instance
        /// </summary>
        /// <param name="settings">The settings to use</param>
        public TextActivator(Matchers settings) : base(null) {
            this.Settings = settings;
        }
        #endregion

        /// <summary>
        /// Sets the callback for this activator
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public TextActivator SetCallback(Command.CommandCallback cb) {
            CommandCallback = cb;
            return this;
        }

        /// <summary>
        /// Sets the callback for this activator
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public TextActivator SetCallback(TextCommandCallback cb) {
            CommandCallback = WrapTextCommand(cb);
            return this;
        }

        private static Command.CommandCallback WrapTextCommand(TextCommandCallback cb) {
            return () => cb?.Invoke(TextCommandCreator.CurrentTextCommand);
        }

        protected override bool IsActivatorActive() {
            foreach (RegexWrapper m in Settings.TextMatchers) {
                if (TextCommands.IsMatchForCommand(m)) {
                    return true;
                }
            }

            return false;
        }
    }
}