using MacroFramework.Commands;
using MacroFramework.Input;
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
        /// Pauses the application such that no activators are updated. Only the <see cref="OnMainLoop"/> delegate is called during pause mode.
        /// </summary>
        public static bool Paused { get; set; }

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
            KeyEvents.Initialize();
            DeviceHook.StartKeyboardHook();
            CommandContainer.Start();
            MainLoop();
            Application.Run();
        }

        private static async void MainLoop() {
            int timeStep = Setup.Instance.Settings.MainLoopTimestep;
            while (Running) {
                OnMainLoop?.Invoke();
                KeyEvents.HandleQueuedKeyevents();
                CommandContainer.UpdateActivators<TimerActivator>();
                TextCommands.ExecuteTextCommandQueue();
                await Task.Delay(Max(1, timeStep));
            }
        }

        private static int Max(int a, int b) {
            return a > b ? a : b;
        }

        /// <summary>
        /// Stops the MacroFramework application
        /// </summary>
        public static void Stop() {
            DeviceHook.StopKeyboardHook();
            CommandContainer.Exit();
            Application.Exit();
            Setup.SetInstance(null);
            Running = false;
        }
    }
}
