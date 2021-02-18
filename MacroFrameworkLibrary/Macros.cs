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
        public static bool Paused { get; internal set; }

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
            UnpauseAndRestartHooks();
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
            InputHook.StopHooks();
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
                InputHook.StopHooks();
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

        #region helpers
        /// <summary>
        /// This effectively pauses the entire application by disabling input hooks and all <see cref="Command"/> and <see cref="MacroFramework.Commands.CommandActivator" /> instances. Use this method if you wish to pause all functionality. Only the <see cref="MainLoopCallback"/> and async methods which started before tha pause will run.
        /// </summary>
        public static void PauseAndStopHooks() {
            Paused = true;
            InputHook.StopHooks();
        }

        /// <summary>
        /// Resumes normal functionality by unpausing the application and restarting all enabled hooks.
        /// </summary>
        public static void UnpauseAndRestartHooks() {
            Paused = false;
            InputHook.StartHooks();
        }

        /// <summary>
        /// Setting <see cref="Paused"/> to true will disable all macro functionality. The hooks will stay on and the <see cref="MainLoopCallback"/> and <see cref="MacroFramework.Input.InputEvents.InputCallback"/> callbacks are still called as well as any async methods which which were started before tha pause.
        /// </summary>
        /// <param name="paused"></param>
        public static void SetPause(bool paused) {
            Paused = paused;
        }
        #endregion
    }
}
