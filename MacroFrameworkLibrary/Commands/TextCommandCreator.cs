using MacroFramework.Input;
using MacroFramework.Tools;

namespace MacroFramework.Commands {
    /// <summary>
    /// Internal class for creating text commands from key events
    /// </summary>
    internal static class TextCommandCreator {

        private static VKeysToCommand keyCommand = new VKeysToCommand();

        internal static string CurrentTextCommand { get; set; }

        internal static bool IsCommandMode { get; private set; }

        private static long lastCommandModeTimestamp;

        internal static void CommandKeyEvent(IInputEvent k) {
            if (k.State && k.Unique) {
                keyCommand.AddKey(k.Key);
            }
        }

        internal static void StartCommand() {
            lastCommandModeTimestamp = Timer.Milliseconds;
            keyCommand.Clear();
        }
        internal static void EndCommand(bool activate) {
            if (activate) {
                string command = keyCommand.ToString();
                Logger.Log("Command: " + command);
                TextCommands.Execute(command);
            }
            keyCommand.Clear();
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

            if (k.Key == Setup.Instance.Settings.CommandKey) {
                return;
            }

            if (!k.State) {
                return;
            }

            if (k.Key == Setup.Instance.Settings.CommandActivateKey) {
                CommandKeyPress(false, true);
                return;
            }

            if (VKeysToCommand.KeyToChar(k.Key) == '\0' && k.Unique) {
                Logger.Log("End wrong key");
                CommandKeyPress(false, false);
                return;
            }

            Logger.Log("Time since last command press: " + timeSince);
            if (timeSince >= Setup.Instance.Settings.TextCommandTimeout) {
                Logger.Log("End command mode on timeout");
                CommandKeyPress(false, false);
                return;
            }

            CommandKeyEvent(k);
        }
        #endregion
    }
}
