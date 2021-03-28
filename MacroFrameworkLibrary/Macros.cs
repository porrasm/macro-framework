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
        /// The different run states the application can be in
        /// </summary>
        public enum RunState {
            /// <summary>
            /// The application is not running
            /// </summary>
            NotRunning,
            /// <summary>
            /// The application is running in a normal state
            /// </summary>
            Running,
            /// <summary>
            /// The application is running in limited mode in which only certain <see cref="IActivator"/> are updated. Input is disabled.
            /// </summary>
            RunningInLimitedMode,
            /// <summary>
            /// The application is paused and no <see cref="Command"/> or <see cref="IActivator"/> instances are automatically updated
            /// </summary>
            Paused
        }

        /// <summary>
        /// The current state of the application
        /// </summary>
        public static RunState State { get; private set; } = RunState.NotRunning;

        /// <summary>
        /// Void callback
        /// </summary>
        public delegate void VoidDelegate();

        /// <summary>
        /// Delegate used for continuing the app
        /// </summary>
        public delegate bool ContinueDelegate();

        /// <summary>
        /// The delegate which is called at the start of every main loop iteration
        /// </summary>
        public static VoidDelegate OnMainLoop { get; set; }

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

        private static ContinueDelegate continueDelegate;

        public static Thread MainThread { get; private set; }
        #endregion

        #region management
        /// <summary>
        /// Starts the synchronous MacrosFramework application. Should be called from a method with an <see cref="STAThreadAttribute"/>.
        /// </summary>
        /// <param name="setup">The setup options</param>
        /// <param name="runInLimitedMode">If true the application is set to <see cref="RunState.RunningInLimitedMode"/></param>
        /// <param name="afterStart">Called immediately after start. You can use this delegate to start your own application on the same thread as the <see cref="MainThread"/>.</param>
        public static void Start(Setup setup, bool runInLimitedMode = false, VoidDelegate afterStart = null) {
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA) {
                throw new Exception("MacroFramwork must be started on an STA thread. See the [STAThread] attribute.");
            }
            if (State != RunState.NotRunning) {
                return;
            }
            Setup.SetInstance(setup);

            if (runInLimitedMode) {
                State = RunState.RunningInLimitedMode;
            } else {
                State = RunState.Running;
                InputHook.StartHooks();
            }

            Logger.Instance = setup.GetLogger();
            InputEvents.Initialize();
            CommandContainer.Start();

            CancellationTokenSource cancel = new CancellationTokenSource();
            Task mainLoop = Task.Run(MainLoop, cancel.Token);

            // Subscriptions
            if (setup.Settings.UseGlobalExceptionHandler) {
                Application.ThreadException += ThreadException;
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;
                TaskScheduler.UnobservedTaskException += UnobservedTaskException;
            }

            AppDomain.CurrentDomain.ProcessExit += StopEvent;

            Application.Run();
            cancel.Cancel();
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
            State = RunState.NotRunning;
        }

        private static void StopEvent(object o, EventArgs e) {
            Stop();
        }
        #endregion

        #region main loop
        private static async Task MainLoop() {
            int timeStep = Setup.Instance.Settings.MainLoopTimestep;
            while (true) {
                OnMainLoop?.Invoke();

                if (State != RunState.Running) {
                    TryContinue();
                }
                if (State == RunState.Paused) {
                    continue;
                }

                CommandContainer.ForEveryCommand((c) => c.OnUpdate());

                if (State == RunState.Running) {
                    InputEvents.HandleQueuedKeyevents();
                }

                CommandContainer.UpdateActivators<TimerActivator>();
                TextCommands.ExecuteTextCommandQueue();
                await Task.Delay(timeStep > 1 ? timeStep : 1);
            }
        }

        private static void TryContinue() {
            try {
                if (continueDelegate?.Invoke() ?? false) {
                    Resume();
                }
            } catch (Exception e) {
                Logger.Exception(e, "MainLoop continue delegate");
            }
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

        #region application state
        /// <summary>
        /// Set the run state of the application
        /// </summary>
        /// <param name="state">The state to set the application in</param>
        /// <param name="continueDelegate">If you set the application to limited run mode or paused mode this delegate can be used to return to <see cref="RunState.Running"/> state after the delegate returns true.></param>
        public static void SetRunState(RunState state, ContinueDelegate continueDelegate = null) {
            switch (state) {
                case RunState.Running:
                    Resume();
                    break;
                case RunState.Paused:
                    PauseUntil(continueDelegate);
                    break;
                case RunState.RunningInLimitedMode:
                    SetLimitedModeUntil(continueDelegate);
                    break;
                case RunState.NotRunning:
                    Stop();
                    break;
            }
        }

        /// <summary>
        /// Resumes the application
        /// </summary>
        public static void Resume() {
            if (State == RunState.Running) {
                return;
            }

            continueDelegate = null;
            State = RunState.Running;
            CommandContainer.ForEveryCommand((c) => c.OnResume());
            KeyStates.ResetKeyStates();
            InputHook.StartHooks();
        }

        /// <summary>
        /// Pauses the application. Input hooks are disabled and all events except the <see cref="OnMainLoop"/> are disabled.
        /// </summary>
        public static void Pause() {
            if (State == RunState.Paused) {
                return;
            }
            State = RunState.Paused;
            CommandContainer.ForEveryCommand((c) => c.OnPause());
            InputHook.StopHooks();
        }

        /// <summary>
        /// Pauses the application until a certain condition becomes true
        /// </summary>
        /// <param name="continueDelegate">Continuation condition delegate</param>
        public static void PauseUntil(ContinueDelegate continueDelegate) {
            Macros.continueDelegate = continueDelegate;
            Pause();
        }

        /// <summary>
        /// Pauses the application. Input hooks are disabled and all events except the <see cref="OnMainLoop"/> are disabled.
        /// </summary>
        public static void SetLimitedMode() {
            if (State == RunState.RunningInLimitedMode) {
                return;
            }
            State = RunState.RunningInLimitedMode;
            InputHook.StopHooks();
        }

        /// <summary>
        /// Pauses the application until a certain condition becomes true
        /// </summary>
        /// <param name="continueDelegate">Continuation condition delegate</param>
        public static void SetLimitedModeUntil(ContinueDelegate continueDelegate) {
            Macros.continueDelegate = continueDelegate;
            SetLimitedMode();
        }
        #endregion
    }
}
