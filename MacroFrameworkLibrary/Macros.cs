using MacroFramework.Commands;
using MacroFramework.Commands.Coroutines;
using MacroFramework.Input;
using System;
using System.Collections.Concurrent;
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
        /// The current setup instance
        /// </summary>
        public static Setup Setup { get; private set; }

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
        /// The delegate which is called at the start of every main loop iteration
        /// </summary>
        public static Action OnMainLoop { get; set; }

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

        private static Func<bool> continueDelegate;

        /// <summary>
        /// The thread on which the event loop and <see cref="InputHook"/> runs on.
        /// </summary>
        public static Thread MainThread { get; private set; }
        private static ConcurrentQueue<Action> mainThreadJobQueue = new ConcurrentQueue<Action>();

        /// <summary>
        /// The thread on which the main loop runs on. The <see cref="Command"/> classes run on this thread.
        /// </summary>
        public static Thread FunctionalityThread { get; private set; }

        /// <summary>
        /// Enqueue a job here to execute some action on the <see cref="FunctionalityThread"/>. The jobs are executed in a try clause on the next main loop iteration.
        /// </summary>
        public static ConcurrentQueue<Action> FunctionalityThreadJobQueue { get; private set; } = new ConcurrentQueue<Action>();

        internal static bool usingCustomEventLoop;

        /// <summary>
        /// The last timestamp of the main update loop
        /// </summary>
        public static long LastMainLoopStart { get; private set; }
        #endregion

        #region management
        /// <summary>
        /// Starts the synchronous MacrosFramework application. Should be called from a method with an <see cref="STAThreadAttribute"/>.
        /// </summary>
        /// <param name="setup">The setup options</param>
        /// <param name="runInLimitedMode">If true the application is set to <see cref="RunState.RunningInLimitedMode"/></param>
        /// <param name="customEventLoop">You can use this delegate to override the default event loop, which is <see cref="Application.Run"/> without a form. Leave as null to use default.</param>
        public static void Start(Setup setup, bool runInLimitedMode = false, Action customEventLoop = null) {
            setup.Logger?.LogMessage("Starting Macros");

            if (setup.Settings == null) {
                throw new NullReferenceException("The settings inside a setup object cannot be null");
            }

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA) {
                throw new Exception("MacroFramwork must be started on an STA thread. See the [STAThread] attribute.");
            }
            if (State != RunState.NotRunning) {
                return;
            }
            usingCustomEventLoop = customEventLoop != null;
            InitializeApplication(setup, runInLimitedMode);

            FunctionalityThread = new Thread(new ThreadStart(MainLoop));

            FunctionalityThread.SetApartmentState(ApartmentState.MTA);
            FunctionalityThread.Start();

            if (usingCustomEventLoop) {
                customEventLoop();
            } else {
                RunDefaultEventLoop();
            }

            FunctionalityThread.Join();
        }

        private static void RunDefaultEventLoop() {
            while (State != RunState.NotRunning) {
                Application.Run();
                while (mainThreadJobQueue.Count > 0) {
                    try {
                        if (mainThreadJobQueue.TryDequeue(out Action cb)) {
                            cb();
                        } else {
                            throw new Exception("Could not dequeue job");
                        }
                    } catch (Exception e) {
                        Logger.Exception(e, "MainThread job");
                    }
                }
            }
        }

        private static void InitializeApplication(Setup setup, bool runInLimitedMode) {
            MainThread = Thread.CurrentThread;
            Macros.Setup = setup;
            Logger.Instance = setup.Logger;

            if (runInLimitedMode) {
                State = RunState.RunningInLimitedMode;
            } else {
                State = RunState.Running;
                InputHook.StartHooks();
            }

            InputEvents.Initialize();
            CommandContainer.Start();

            // Subscriptions
            if (setup.Settings.UseGlobalExceptionHandler) {
                Application.ThreadException += ThreadException;
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;
                TaskScheduler.UnobservedTaskException += UnobservedTaskException;
            }
            AppDomain.CurrentDomain.ProcessExit += StopEvent;
        }

        /// <summary>
        /// Stops the MacroFramework application
        /// </summary>
        public static void Stop() {
            Logger.Log("Stopping macros");

            InputHook.StopHooks();
            CommandContainer.Exit();

            // Unsubscribe
            Application.ThreadException -= ThreadException;
            AppDomain.CurrentDomain.UnhandledException -= UnhandledException;
            TaskScheduler.UnobservedTaskException -= UnobservedTaskException;
            AppDomain.CurrentDomain.ProcessExit -= StopEvent;

            Application.Exit();
            Macros.Setup = null;
            State = RunState.NotRunning;
        }

        private static void StopEvent(object o, EventArgs e) {
            Stop();
        }

        /// <summary>
        /// Queues a job for the main STA thread which runs the event loop.
        /// </summary>
        /// <param name="action">The job to add to the queue</param>
        public static void QueueJobOnMainThread(Action action) {
            mainThreadJobQueue.Enqueue(action);
        }

        /// <summary>
        /// Stops the event loop using <see cref="Application.Exit"/>, executes the jobs in the queue and restarts the event loop using <see cref="Application.Run"/>.
        /// </summary>
        public static void ExecuteMainThreadJobs() {
            Application.Exit();
        }
        #endregion

        #region main loop
        private static void MainLoop() {
            while (State != RunState.NotRunning) {
                MainLoopStart();
                if (State == RunState.Paused) {
                    continue;
                }
                UpdateCommandFunctionality();
                MainLoopDelay();
                CommandContainer.ForEveryCommand(c => c.Coroutines.UpdateCoroutines(CoroutineUpdateGroup.OnAfterUpdate), false, $"Coroutine {CoroutineUpdateGroup.OnAfterUpdate}");
            }
        }
        private static void MainLoopStart() {
            LastMainLoopStart = Tools.Timer.Milliseconds;

            OnMainLoop?.Invoke();
            CommandContainer.ForEveryCommand(c => c.Coroutines.UpdateCoroutines(CoroutineUpdateGroup.OnBeforeUpdate), false, $"Coroutine {CoroutineUpdateGroup.OnBeforeUpdate}");

            if (State != RunState.Running) {
                TryContinue();
            }
        }


        private static void UpdateCommandFunctionality() {
            CommandContainer.ForEveryCommand((c) => c.OnUpdate(), false, "Main loop update");

            int keyFixTimestep = Macros.Setup.Settings.KeyStateFixTimestep;
            if (KeyStates.KeyDownCount < 0) {
                Logger.Log("KeyDownCount desynced, fixing state automatically.");
                KeyStates.ResetKeyStates(true);
            } else if (keyFixTimestep > 0 && KeyStates.KeyDownCount > 0 && Tools.Timer.PassedFrom(KeyStates.LastKeyResetTime) >= keyFixTimestep) {
                KeyStates.ResetKeyStates(false);
            }

            if (State == RunState.Running) {
                InputEvents.HandleQueuedKeyevents();
            }

            CommandContainer.UpdateActivators<TimerActivator>();
            CommandContainer.ForEveryCommand(c => c.Coroutines.UpdateCoroutines(CoroutineUpdateGroup.OnTimerUpdate), false, $"Coroutine {CoroutineUpdateGroup.OnTimerUpdate}");


            TextCommands.ExecuteTextCommandQueue();
        }

        private static void MainLoopDelay() {
            if (Macros.Setup == null) {
                return;
            }
            long delay = Macros.Setup.Settings.MainLoopTimestep - Tools.Timer.PassedFrom(LastMainLoopStart);
            delay = delay > 0 ? delay : 0;
            if (delay > 0) {
                Thread.Sleep((int)delay);
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
        public static void SetRunState(RunState state, Func<bool> continueDelegate = null) {
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

            Logger.Log("Resume Macros");

            continueDelegate = null;
            State = RunState.Running;
            CommandContainer.ForEveryCommand((c) => c.OnResume(), false);
            KeyStates.ResetKeyStates(true);
            QueueJobOnMainThread(() => InputHook.StartHooks());
            ExecuteMainThreadJobs();
        }

        /// <summary>
        /// Pauses the application. Input hooks are disabled and all events except the <see cref="OnMainLoop"/> are disabled.
        /// </summary>
        public static void Pause() {
            if (State == RunState.Paused) {
                return;
            }
            Logger.Log("Pause Macros");
            State = RunState.Paused;
            CommandContainer.ForEveryCommand((c) => c.OnPause(), false);
            InputHook.StopHooks();
        }

        /// <summary>
        /// Pauses the application until a certain condition becomes true
        /// </summary>
        /// <param name="continueDelegate">Continuation condition delegate</param>
        public static void PauseUntil(Func<bool> continueDelegate) {
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
            Logger.Log("Limit Macros");
            State = RunState.RunningInLimitedMode;
            InputHook.StopHooks();
        }

        /// <summary>
        /// Pauses the application until a certain condition becomes true
        /// </summary>
        /// <param name="continueDelegate">Continuation condition delegate</param>
        public static void SetLimitedModeUntil(Func<bool> continueDelegate) {
            Macros.continueDelegate = continueDelegate;
            SetLimitedMode();
        }
        #endregion
    }
}
