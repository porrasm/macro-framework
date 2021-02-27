using MacroFramework.Commands;
using MacroFramework.Input;
using System;
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

        /// <summary>
        /// Callback used to catch exceptions globally
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The exception which was handled</param>
        /// <param name="type">The type of exception</param>
        /// <param name="isTerminating">Indicates whether the common language runtime is terminating</param>
        public delegate void ExceptionCallback(object sender, Exception e, ExceptionType type, bool isTerminating);

        /// <summary>
        /// Called on any exception which was not caught by a trycatch clause
        /// </summary>
        public static ExceptionCallback OnException { get; set; }
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
            if (setup.Settings.UseGlobalExceptionHandler) {
                Application.ThreadException += ThreadException;
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;
                TaskScheduler.UnobservedTaskException += UnobservedTaskException;
            }

            AppDomain.CurrentDomain.ProcessExit += StopEvent;

            Application.Run();
        }

        /// <summary>
        /// Stops the MacroFramework application
        /// </summary>
        public static void Stop() {
            InputHook.StopHooks();
            CommandContainer.Exit();

            // Unsubscribe
            Application.ThreadException -= ThreadException;
            AppDomain.CurrentDomain.UnhandledException -= UnhandledException;
            TaskScheduler.UnobservedTaskException -= UnobservedTaskException;
            AppDomain.CurrentDomain.ProcessExit -= StopEvent;

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
        /// <summary>
        /// Enum containing the different types of exceptions which can be caught
        /// </summary>
        public enum ExceptionType {
            /// <summary>
            /// This exception was caught by the <see cref="Application.ThreadException"/> handler
            /// </summary>
            ThreadException,
            /// <summary>
            /// This exception was caught by the <see cref="AppDomain.CurrentDomain.UnhandledException "/> handler
            /// </summary>
            UnhandledException,
            /// <summary>
            /// This exception was caught by the <see cref="TaskScheduler.UnobservedTaskException"/> handler
            /// </summary>
            UnobservedTaskException
        }

        private static void ThreadException(object sender, ThreadExceptionEventArgs e) {
            Logger.Log("ThreadException");
            HandleExceptions(sender, e.Exception, ExceptionType.ThreadException);
        }
        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Logger.Log("UnhandledException");
            HandleExceptions(sender, (Exception)e.ExceptionObject, ExceptionType.UnhandledException, e.IsTerminating);
        }
        private static void UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
            Logger.Log("UnobservedTaskException");
            HandleExceptions(sender, (Exception)e.Exception, ExceptionType.UnobservedTaskException, false);
        }

        private static void HandleExceptions(object sender, Exception e, ExceptionType type, bool isTerminating = true) {
            OnException?.Invoke(sender, e, type, isTerminating);
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
