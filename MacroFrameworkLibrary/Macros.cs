using MacroFramework.Commands;
using MacroFramework.Input;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MacroFramework {
    /// <summary>
    /// Static class used for starting the application with settings
    /// </summary>
    public static class Macros {

        #region fields
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
        #endregion

        #region management
        /// <summary>
        /// Starts the synchronous MacrosFramework application. Should be called from a method with an <see cref="STAThreadAttribute"/>.
        /// </summary>
        /// <param name="setup">The setup options</param>
        public static void Start(Setup setup) {
            Logger.Instance = setup.GetLogger();
            if (Running) {
                return;
            }
            Setup.SetInstance(setup);
            Running = true;
            InputEvents.Initialize();

            if (setup.Settings.AutoEnableKeyboardHook) {
                DeviceHook.StartKeyboardHook();
            }
            if (setup.Settings.AutoEnableMouseHook) {
                DeviceHook.StartMouseHook();
            }

            CommandContainer.Start();
            MainLoop();

            // Subscriptions
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(StopEvent);

            Application.Run();
        }

        /// <summary>
        /// Stops the MacroFramework application
        /// </summary>
        public static void Stop() {
            DeviceHook.StopHooks();
            CommandContainer.Exit();

            // Unsubscribe
            Application.ThreadException -= new ThreadExceptionEventHandler(ThreadException);
            AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(UnhandledException);
            AppDomain.CurrentDomain.ProcessExit -= new EventHandler(StopEvent);

            Application.Exit();
            Setup.SetInstance(null);
            Running = false;
        }

        private static void StopEvent(object o, EventArgs e) {
            Stop();
        }
        #endregion

        #region main loop
        private static async void MainLoop() {
            int timeStep = Setup.Instance.Settings.MainLoopTimestep;
            while (Running) {
                OnMainLoop?.Invoke();
                InputEvents.HandleQueuedKeyevents();
                CommandContainer.UpdateActivators<TimerActivator>();
                TextCommands.ExecuteTextCommandQueue();
                await Task.Delay(Max(1, timeStep));
            }
        }

        private static int Max(int a, int b) {
            return a > b ? a : b;
        }
        #endregion

        #region exception handling
        private static void ThreadException(object sender, ThreadExceptionEventArgs e) => HandleExceptions(e.Exception, "Thread Exception");
        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e) => HandleExceptions((Exception)e.ExceptionObject, "Unhandled Exception", e.IsTerminating);
        private static void HandleExceptions(Exception e, string type = null, bool terminating = true) {
            try {
                DeviceHook.StopHooks();
                var name = Process.GetCurrentProcess().ProcessName;
                Logger.Log(e.ToString() + "\n\n\nCopy exception to clipboard?" + name + (type == null ? "" : $" - {type}") + (terminating ? " - Fatal" : " - Non-fatal"));
            } catch (Exception ee) {
                try {
                    Logger.Log(ee.ToString() + "Exception handler critical error");

                    Macros.Stop();
                } catch {
                    Environment.Exit(1);
                }
            }

            if (Debugger.IsAttached) {
                throw e;
            } else {
                try {
                    Logger.Log("Exception occurred");
                    Stop();
                } catch {
                    Environment.Exit(1);
                }
            }
        }
        #endregion

        public static void Log() {

        }
    }
}
