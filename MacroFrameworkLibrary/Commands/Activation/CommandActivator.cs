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

        public Command Owner { get; set; }

        /// <summary>
        /// The current callback of this activator
        /// </summary>
        protected Command.TextCommandCallback commandCallback;

        /// <summary>
        /// Initializes this activator with a callback
        /// </summary>
        /// <param name="command"></param>
        public CommandActivator( Command.TextCommandCallback command) {
            this.commandCallback = command;
        }

        public abstract bool IsActive();

        public virtual void Execute() {
            commandCallback?.Invoke(null);
        }
    }
}