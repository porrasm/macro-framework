using System;
using System.Threading;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// The base class for all included activators. Inherit this class or implement <see cref="IActivator"/> for custom functionality.
    /// </summary>
    public abstract class CommandActivator : IActivator {

        public Command Owner { get; set; }

        /// <summary>
        /// If true, the active status of the owner (<see cref="Command.IsActive"/>) is ignored
        /// </summary>
        public bool IgnoreOwnerActiveStatus { get; set; }

        /// <summary>
        /// The current callback of this activator
        /// </summary>
        internal Command.CommandCallback CommandCallback { get; set; }

        /// <summary>
        /// Initializes this activator with a callback
        /// </summary>
        /// <param name="command">The callback to be called when this activator becomes active</param>
        /// <param name="ignoreOwnerContext"><see cref="CommandActivator.IgnoreOwnerActiveStatus"/></param>
        public CommandActivator(Command.CommandCallback command, bool ignoreOwnerContext = false) {
            this.CommandCallback = command;
            this.IgnoreOwnerActiveStatus = ignoreOwnerContext;
        }

        /// <summary>
        /// Returns true if the activator is active. Also takes into account the context of the owner: <see cref="Command.IsActive"/>
        /// </summary>
        /// <returns></returns>
        public bool IsActive() {
            if (IgnoreOwnerActiveStatus || (Owner?.IsActive() ?? true)) {
                return IsActivatorActive();
            }
            return false;
        }

        public virtual Type UpdateGroup => GetType();

        /// <summary>
        /// Abstract bool for individual activator functionality. Override for custom functionality.
        /// </summary>
        /// <returns></returns>
        protected abstract bool IsActivatorActive();

        /// <summary>
        /// Executes the callback of the activator as well as the <see cref="Command.OnExecuteStart"/> and <see cref="Command.OnExecutionComplete"/> methods if <see cref="Owner"/> is assigned
        /// </summary>
        public virtual void Execute() {
            Owner?.OnExecuteStart();
            CommandCallback?.Invoke();
            Owner?.OnExecutionComplete();
        }

        #region dynamic activators
        /// <summary>
        /// Wraps this <see cref="CommandActivator"/> into a <see cref="DynamicActivator"/> instance and adds it to the list of active activators using <see cref="CommandContainer.AddDynamicActivator(IDynamicActivator)"/>
        /// </summary>
        /// <param name="removeAfterFirstActivation">Indicates whether the dynamic activator should be discarded after the first activation</param>
        /// <returns></returns>
        public DynamicActivator RegisterDynamicActivator(bool removeAfterFirstActivation) {
            DynamicActivator dynamic = new DynamicActivator(this, removeAfterFirstActivation);
            CommandContainer.AddDynamicActivator(dynamic);
            return dynamic;
        }

        /// <summary>
        /// Wraps this <see cref="CommandActivator"/> into a <see cref="DynamicActivator"/> instance and adds it to the list of active activators using <see cref="CommandContainer.AddDynamicActivator(IDynamicActivator)"/>
        /// </summary>
        /// <param name="removeAfterFirstActivation"></param>
        /// <returns></returns>
        public DynamicActivator RegisterDynamicActivator(DynamicActivator.RemoveAfterExecutionDelegate removeAfterFirstActivation) {
            DynamicActivator dynamic = new DynamicActivator(this, removeAfterFirstActivation);
            CommandContainer.AddDynamicActivator(dynamic);
            return dynamic;
        }

        /// <summary>
        /// Asynchronously waits for the <see cref="IActivator"/> to become active using <see cref="IDynamicActivator"/>. This is called sometime after the activator becomes active, not immeditaly so using e.g. <see cref="Input.InputEvents.CurrentInputEvent"/> or <see cref="TextCommands.CurrentTextCommand"/> will not work. Returns false if the operation was cancelled.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds after which the operation is cancelled. Set to 0 or less to ignore timeout</param>
        public async Task<bool> WaitForActivation(int timeout = 10000) {
            TaskCompletionSource<bool> job = new TaskCompletionSource<bool>();
            AsyncDynamicActivator task = new AsyncDynamicActivator(this, job);
            CommandContainer.AddDynamicActivator(task);

            CancellationTokenSource timeoutCancel = new CancellationTokenSource();
            Task timeoutTask = timeout <= 0 ? Task.Delay(0) : Task.Delay(timeout, timeoutCancel.Token);
            await Task.WhenAny(job.Task, timeoutTask);

            if (job.Task.IsCompleted) {
                timeoutCancel.Cancel();
                timeoutTask?.Dispose();
                return true;
            } else {
                job.TrySetCanceled();
                job.Task.Dispose();
                task.IsCanceled = true;
                return false;
            }
        }
        #endregion
    }
}