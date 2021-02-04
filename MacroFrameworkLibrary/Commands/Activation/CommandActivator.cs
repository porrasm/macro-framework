using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    public abstract class CommandActivator {
        protected Command.CommandCallback command;
        public CommandActivator(Command.CommandCallback command) {
            this.command = command;
        }

        public abstract bool IsActive();

        public virtual void Execute() {
            try {
                ExecuteCallback();
            } catch (Exception e) {
                Console.WriteLine("Error executing command: \n\n" + e);
            }
        }

        protected abstract void ExecuteCallback();
    }
}
