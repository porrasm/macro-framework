using MacroFramework.Commands;
using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MacroFramework {
    public static class Macros {

        public static bool Running { get; private set; }

        /// <summary>
        /// Starts the synchronous MacrosFramework application. Give an assembly as a parameter to automatically load all <see cref="Command"/> instances.
        /// </summary>
        /// <param name="macroAssembly">The assembly of your implementation</param>
        [STAThread]
        public static void Start(Setup setup) {
            if (Running) {
                return;
            }
            Setup.SetInstance(setup);
            Running = true;
            InputHook.StartHooks();
            CommandContainer.Start();
            Application.Run();
        }

        /// <summary>
        /// Stops the MacroFramework application.
        /// </summary>
        public static void Stop() {
            InputHook.StopHooks();
            CommandContainer.OnClose();
            Application.Exit();
            Setup.SetInstance(null);
            Running = false;
        }

        #region tools
        /// <summary>
        /// Executes a text command immediately.
        /// </summary>
        /// <param name="command"></param>
        public static void ExecuteTextCommand(string command) {
            TextCommandCreator.QueueTextCommand(command);

            // bug, may call keybinds twice
            CommandContainer.ExecuteCommands();
        }
        #endregion
    }
}
