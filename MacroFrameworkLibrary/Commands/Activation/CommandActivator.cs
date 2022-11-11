using System;
using System.Threading;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// The base class for all included activators. Inherit this class or implement <see cref="IActivator"/> for custom functionality.
    /// </summary>
    public abstract class CommandActivator : IActivator {

        public CommandBase Owner { get; set; }

        /// <summary>
        /// If true, the active status of the owner (<see cref="Command.IsActiveDelegate"/>) is ignored
        /// </summary>
        public bool IgnoreOwnerActiveStatus { get; set; }

        /// <summary>
        /// The current callback of this activator
        /// </summary>
        internal Action CommandCallback { get; set; }

        /// <summary>
        /// Initializes this activator with a callback
        /// </summary>
        /// <param name="command">The callback to be called when this activator becomes active</param>
        /// <param name="ignoreOwnerContext"><see cref="CommandActivator.IgnoreOwnerActiveStatus"/></param>
        public CommandActivator(Action command, bool ignoreOwnerContext = false) {
            this.CommandCallback = command;
            this.IgnoreOwnerActiveStatus = ignoreOwnerContext;
        }

        /// <summary>
        /// Syntactic sugar for adding this <see cref="CommandActivator"/> to a <see cref="Command"/> using <see cref="ActivatorContainer.AddActivator(IActivator)"/>
        /// </summary>
        /// <param name="acts">The group to add this to</param>
        public void AssignTo(ActivatorContainer acts) {
            if (acts.Activators == null) {
                throw new Exception("Can't assign activator to a container which is not directly owned by a Command instance");
            }
            acts.AddActivator(this);
        }

        /// <summary>
        /// Returns true if the activator is active. Also takes into account the context of the owner: <see cref="Command.IsActiveDelegate"/>
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
    }
}