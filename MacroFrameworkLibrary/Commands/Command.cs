using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    public abstract class Command {

        #region fields
        public delegate void CommandCallback(string command);
        public delegate void CommandCallbackParameterless();

        protected CommandActivatorGroup activator;
        public bool UsesActivator => activator.Activators.Count > 0;

        public virtual bool GetContext() => true;
        #endregion

        public Command() {
            InitializeActivators(out activator);
        }

        /// <summary>
        /// Abstract method for initializing CommandActivators and class functionality. Use this like you would use a constructor. CommandActivators array mustn't be null and has to have at least 1 activator.
        /// </summary>
        protected virtual void InitializeActivators(out CommandActivatorGroup activator) {
            activator = new CommandActivatorGroup();
        }

        /// <summary>
        /// Called before the execution starts.
        /// </summary>
        /// <param name="command"></param>
        protected virtual void OnExecuteStart() { }

        /// <summary>
        /// Called after the execution is complete.
        /// </summary>
        /// <param name="command"></param>
        protected virtual void OnExecutionComplete() { }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        public virtual void OnStart() { }

        /// <summary>
        /// Called when the application quits.
        /// </summary>
        public virtual void OnClose() { }

        /// <summary>
        /// This method is called whenever a text command is executed, even if it doesn't match any of the activators.
        /// </summary>
        /// <param name="command"></param>
        public virtual void OnCommand(string command) {  }

        public void ExecuteIfActive() {
            if (activator.IsActive(out CommandActivator a) && GetContext()) {
                OnExecuteStart();
                a.Execute();
                OnExecutionComplete();
            }
        }
    }
}
