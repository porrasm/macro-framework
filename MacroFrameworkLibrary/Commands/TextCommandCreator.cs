using MacroFramework.Input;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    internal static class TextCommandCreator {
        private static VKeysToCommand keyCommand = new VKeysToCommand();

        internal static string CurrentTextCommand { get; set; }

        public static void CommandKeyEvent(VKey key, bool value, bool isUniquePress) {
            if (value && isUniquePress) {
                keyCommand.AddKey(key);
            }
        }

        public static void StartCommand() {
            keyCommand.Clear();
        }
        public static void EndCommand(bool activate) {
            if (activate) {
                string command = keyCommand.ToString();
                Console.WriteLine("Command: " + command);
                TextCommands.Execute(command);
            }
            keyCommand.Clear();
        }
    }
}
