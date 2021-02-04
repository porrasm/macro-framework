using MacroFramework.Input;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    public static class TextCommandCreator {
        private static VKeysToCommand keyCommand = new VKeysToCommand();

        private static List<string> sentCommands = new List<string>();

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
                Console.WriteLine("Command: " + keyCommand.ToString());
                string command = keyCommand.ToString();
                QueueTextCommand(command);
            }
            keyCommand.Clear();
        }

        internal static void QueueTextCommand(string command) {
            Console.WriteLine("Executing text command: " + command);
            if (command == null || command.Length == 0) {
                return;
            }
            sentCommands.Add(command);
        }

        public static void Clear() {
            sentCommands.Clear();
        }

        public static bool CollectCommand(RegexWrapper matcher, out string commandString) {

            for (int i = 0; i < sentCommands.Count; i++) {
                if (matcher.IsMatch(sentCommands[i])) {
                    commandString = sentCommands[i];
                    sentCommands.RemoveAt(i);
                    return true;
                }
            }

            commandString = null;
            return false;
        }
    }
}
