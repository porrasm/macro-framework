using MacroFramework.Tools;
using System;
using System.Collections.Generic;

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

        private static Queue<string> textCommandQueue;

        static TextCommands() {
            textCommandQueue = new Queue<string>();
        }

        /// <summary>
        /// Executes or queues any given string command and notifies all <see cref="TextActivator"/> instances
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="runImmediately">If true the command is executed immediately. This can cause infinite loops if a <see cref="TextActivator"/> calls this method. Use with caution. If false the text command is executed at the next main update loop.</param>
        public static void Execute(string command, bool runImmediately = false) {
            if (runImmediately) {
                ExecuteStringCommand(command);
            } else {
                textCommandQueue.Enqueue(command);
            }
        }

        /// <summary>
        /// Executes the queued up string commands
        /// </summary>
        internal static void ExecuteTextCommandQueue() {
            while (textCommandQueue.Count > 0) {
                ExecuteStringCommand(textCommandQueue.Dequeue());
            }
        }

        private static void ExecuteStringCommand(string s) {
            CurrentTextCommand = s;
            CommandWasAccepted = false;
            CommandContainer.UpdateActivators<TextActivator>();

            foreach (Command c in CommandContainer.Commands) {
                try {
                    c.OnTextCommand(s, CommandWasAccepted);
                } catch (Exception e) {
                    Console.WriteLine($"Error on {c.GetType()} OnTextCommand: {e.Message}");
                }
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
