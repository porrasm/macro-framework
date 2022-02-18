using MacroFramework.Input;
using MacroFramework.Tools;

namespace MacroFramework.Commands {
    /// <summary>
    /// Class for creating text commands from key events
    /// </summary>
    public static class TextCommandCreator {

        private static VKeysToCommand keyCommand = new VKeysToCommand();

        /// <summary>
        /// Returns the current active text command which is currently being typed in.
        /// </summary>
        public static string BuildTextCommand => keyCommand.ToString();

        internal static string CurrentTextCommand { get; set; }

        /// <summary>
        /// Returns true if the text command mode is currently active
        /// </summary>
        public static bool IsCommandMode { get; private set; }

        private static long lastCommandModeTimestamp;

        internal static void CommandKeyEvent(IInputEvent k) {
            if (k.State && k.Unique) {
                keyCommand.AddKey(k.Key);
            }
        }

        internal static void StartCommand() {
            lastCommandModeTimestamp = Timer.Milliseconds;
            keyCommand.Clear();
            Callbacks.ExecuteAction(TextCommands.OnTextCommandModeStart, $"Error executing OnTextCommandModeStart");
        }
        internal static void EndCommand(bool activate) {
            if (activate) {
                string command = BuildTextCommand;
                Logger.Log("Command: " + command);
                TextCommands.Execute(command);
            }
            keyCommand.Clear();
            Callbacks.ExecuteAction(TextCommands.OnTextCommandModeEnd, $"Error executing OnTextCommandModeEnd");
        }

        #region keyevents
        internal static void CommandKeyPress(bool state, bool acceptCommand) {
            if (state && !IsCommandMode) {
                Logger.Log("\n Command mode start");
                TextCommandCreator.StartCommand();
            } else if (!state && IsCommandMode) {
                Logger.Log("\n Command mode end");
                TextCommandCreator.EndCommand(acceptCommand);
            }
            IsCommandMode = state;
        }

        internal static void OnCommandMode(IInputEvent k) {
            long timeSince = Timer.PassedFrom(lastCommandModeTimestamp);
            lastCommandModeTimestamp = k.ReceiveTimestamp;

            if (k.Type == InputEventType.Mouse) {
                return;
            }

            if (k.Key == Macros.Setup.Settings.CommandKey) {
                return;
            }

            if (!k.State) {
                return;
            }

            if (k.Key == Macros.Setup.Settings.CommandActivateKey) {
                CommandKeyPress(false, true);
                return;
            }

            if (VKeysToCommand.KeyToChar(k.Key) == '\0' && k.Unique) {
                Logger.Log("End wrong key");
                CommandKeyPress(false, false);
                return;
            }

            Logger.Log("Time since last command press: " + timeSince);
            if (timeSince >= Macros.Setup.Settings.TextCommandTimeout) {
                Logger.Log("End command mode on timeout");
                CommandKeyPress(false, false);
                return;
            }

            CommandKeyEvent(k);
        }
        #endregion
    }
}
