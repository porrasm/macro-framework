using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// The base class for all included activators. Inherit this class or implement <see cref="IActivator"/> for custom functionality.
    /// </summary>
    public abstract class CommandActivator : IActivator {

        public Command Owner { get; set; }

        /// <summary>
        /// If true, the context <see cref="Command.IsActive()"/> of the owner is ignored
        /// </summary>
        public bool IgnoreOwnerContext { get; set; }

        /// <summary>
        /// The current callback of this activator
        /// </summary>
        protected Command.CommandCallback commandCallback;

        /// <summary>
        /// Initializes this activator with a callback
        /// </summary>
        /// <param name="command">The callback to be called when this activator becomes active</param>
        /// <param name="ignoreOwnerContext"><see cref="CommandActivator.IgnoreOwnerContext"/></param>
        public CommandActivator( Command.CommandCallback command, bool ignoreOwnerContext = false) {
            this.commandCallback = command;
            this.IgnoreOwnerContext = ignoreOwnerContext;
        }

        /// <summary>
        /// Returns true if the activator is active. Also takes into account the context of the owner: <see cref="Command.IsActive"/>
        /// </summary>
        /// <returns></returns>
        public bool IsActive() {
            if (IgnoreOwnerContext || (Owner?.IsActive() ?? true)) {
                return IsActivatorActive();
            }
            return false;
        }

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
            commandCallback?.Invoke();
            Owner?.OnExecutionComplete();
        }
    }
}