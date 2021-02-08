using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// The static class which handles the execution of text commands
    /// </summary>
    public static class TextCommands {

        /// <summary>
        /// Returns the current text command which is being executed or null if none are being executed at this moment
        /// </summary>
        public static string CurrentTextCommand { get; private set; }

        /// <summary>
        /// True if the command was accepted by some <see cref="TextActivator"/> instance. Remains the same value until the next <see cref="Execute(string)"/>
        /// </summary>
        internal static bool CommandWasAccepted { get; private set; }

        /// <summary>
        /// Executes any given string command and notifies all <see cref="TextActivator"/> instances
        /// </summary>
        /// <param name="s"></param>
        public static void Execute(string s) {
            CurrentTextCommand = s;
            CommandWasAccepted = false;
            CommandContainer.UpdateActivators<TextActivator>();

            foreach (Command c in CommandContainer.Commands) {
                c.OnTextCommand(s, CommandWasAccepted);
            }

            CurrentTextCommand = null;
        }

        /// <summary>
        /// Returns true if the command was accepted by the matcher.
        /// </summary>
        /// <param name="matcher">A regex wrapper object</param>
        /// <param name="commandString">The command which is being executed</param>
        /// <returns></returns>
        internal static bool IsMatchForCommand(RegexWrapper matcher) {
            if (matcher.IsMatch(CurrentTextCommand)) {
                return CommandWasAccepted = true;
            }
            return false;
        }
    }
}
