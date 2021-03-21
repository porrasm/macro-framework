using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    public class DynamicActivator : IDynamicActivator {

        #region fields
        public uint ID { get; }
        public IActivator Activator { get; private set; }
        public bool IsCanceled { get; set; }
        public delegate bool RemoveAfterExecutionDelegate();
        private RemoveAfterExecutionDelegate removeAfterFirstExecute;
        #endregion

        /// <summary>
        /// Creates a new <see cref="DynamicActivator"/> instance
        /// </summary>
        /// <param name="activator">The activator to use</param>
        /// <param name="removeAfterFirstExecute">Indicates whether the activator should be discarded after the first activation</param>
        public DynamicActivator(IActivator activator, bool removeAfterFirstExecute) {
            Activator = activator;
            if (removeAfterFirstExecute) {
                this.removeAfterFirstExecute = () => true;
            } else {
                this.removeAfterFirstExecute = () => false;
            }
            this.ID = CommandContainer.UniqueDynamicActivatorID;
        }

        /// <summary>
        /// Creates a new <see cref="DynamicActivator"/> instance
        /// </summary>
        /// <param name="activator">The activator to use</param>
        /// <param name="removeAfter">The delegate which indicates whether this <see cref="DynamicActivator"/> should be removed after execution. If null <see cref="RemoveAfterExecution"/> always returns true.</param>
        public DynamicActivator(IActivator activator, RemoveAfterExecutionDelegate removeAfter = null) {
            if (removeAfterFirstExecute == null) {
                throw new Exception("This delegate cannot be null");
            }
            Activator = activator;
            this.removeAfterFirstExecute = removeAfter;
            this.ID = CommandContainer.UniqueDynamicActivatorID;
        }

        public void Execute() {
            Activator.Execute();
        }

        public bool RemoveAfterExecution() {
            return removeAfterFirstExecute();
        }

        public void OnRemove() {
        }
    }
}
