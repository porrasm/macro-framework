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


        internal static void CommandKeyEvent(KeyEvent k) {
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
    }
}
