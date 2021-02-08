using MacroFramework.Commands;
using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MacroFramework {
    public static class Macros {

        /// <summary>
        /// True if the framework is running
        /// </summary>
        public static bool Running { get; private set; }

        /// <summary>
        /// Void callback
        /// </summary>
        public delegate void MainLoopCallback();
        /// <summary>
        /// The delegate which is called at the start of every main loop iteration
        /// </summary>
        public static MainLoopCallback OnMainLoop { get; set; }

        /// <summary>
        /// Starts the synchronous MacrosFramework application. Should be called from a method with an <see cref="STAThreadAttribute"/>.
        /// </summary>
        /// <param name="setup">The setup options</param>
        public static void Start(Setup setup) {
            if (Running) {
                return;
            }
            Setup.SetInstance(setup);
            Running = true;
            InputHook.StartHooks();
            CommandContainer.Start();
            MainLoop();
            Application.Run();
        }

        private static async void MainLoop() {
            while (Running) {
                OnMainLoop?.Invoke();
                CommandContainer.UpdateActivators<TimerActivator>();
                await Task.Delay(Setup.Instance.Settings.MainLoopTimestep);
            }
        }

        /// <summary>
        /// Stops the MacroFramework application
        /// </summary>
        public static void Stop() {
            InputHook.StopHooks();
            CommandContainer.Exit();
            Application.Exit();
            Setup.SetInstance(null);
            Running = false;
        }
    }
}
