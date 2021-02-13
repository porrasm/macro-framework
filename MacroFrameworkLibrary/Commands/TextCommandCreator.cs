using MacroFramework.Input;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// Internal class for creating text commands from key events
    /// </summary>
    internal static class TextCommandCreator {

        private static VKeysToCommand keyCommand = new VKeysToCommand();

        internal static string CurrentTextCommand { get; set; }

        internal static bool IsCommandMode { get; private set; }

        internal static void CommandKeyEvent(IInputEvent k) {
            if (k.State && k.Unique) {
                keyCommand.AddKey(k.Key);
            }
        }

        internal static void StartCommand() {
            keyCommand.Clear();
        }
        internal static void EndCommand(bool activate) {
            if (activate) {
                string command = keyCommand.ToString();
                Console.WriteLine("Command: " + command);
                TextCommands.Execute(command);
            }
            keyCommand.Clear();
        }

        #region keyevents
        internal static void CommandModeKeyEvent(KeyEvent k) {
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
                Console.WriteLine("End wrong key");
                CommandKeyPress(false, false);
                return;
            }

            CommandKeyEvent(k);
        }
        internal static void CommandKeyPress(bool state, bool acceptCommand) {
            if (state && !IsCommandMode) {
                Console.WriteLine("\n Command mode start");
                TextCommandCreator.StartCommand();
            } else if (!state && IsCommandMode) {
                Console.WriteLine("\n Command mode end");
                TextCommandCreator.EndCommand(acceptCommand);
            }
            IsCommandMode = state;
        }

        internal static void OnCommandMode(IInputEvent k) {
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
                Console.WriteLine("End wrong key");
                CommandKeyPress(false, false);
                return;
            }

            CommandKeyEvent(k);
        }
        #endregion
    }
}
