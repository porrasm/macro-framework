using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    public class DynamicActivator : IDynamicActivator {

        #region fields
        public IActivator Activator { get; private set; }
        public bool IsCanceled { get; set; }
        public delegate bool RemoveAfterExecutionDelegate();
        private RemoveAfterExecutionDelegate removeAfter;
        #endregion

        /// <summary>
        /// Creates a new <see cref="DynamicActivator"/> instance
        /// </summary>
        /// <param name="activator">The activator to use</param>
        /// <param name="removeAfter">The delegate which indicates whether this <see cref="DynamicActivator"/> should be removed after execution. If null <see cref="RemoveAfterExecution"/> always returns true.</param>
        public DynamicActivator(IActivator activator, RemoveAfterExecutionDelegate removeAfter = null) {
            Activator = activator;
            this.removeAfter = removeAfter;
        }

        public void Execute() {
            Activator.Execute();
        }

        public bool RemoveAfterExecution() {
            if (removeAfter == null) {
                return true;
            }
            return removeAfter();
        }

        public void OnRemove() {
        }
    }
}
