using MacroFramework.Commands.Activation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// The base class for all activators. Inherit this class for custom functionality.
    /// </summary>
    public abstract class CommandActivator : ICommandActivator {
        protected Command.TextCommandCallback command;
        public CommandActivator(Command.TextCommandCallback command) {
            this.command = command;
        }

        /// <summary>
        /// Returns true if the activator is active
        /// </summary>
        /// <returns></returns>
        public abstract bool IsActive();

        /// <summary>
        /// Executes the callback
        /// </summary>
        public void Execute() {
            ExecuteCallback();
        }

        protected abstract void ExecuteCallback();
    }
}