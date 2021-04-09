using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// Default implementtion of <see cref="IDynamicActivator"/>
    /// </summary>
    public class DynamicActivator : IDynamicActivator {

        #region fields
        public uint ID { get; }
        public IActivator Activator { get; private set; }
        public bool IsCanceled { get; set; }
        private Func<bool> removeAfterExecution;

        /// <summary>
        /// This delegate is called on <see cref="Execute"/> before the <see cref="IActivator.Execute"/> method
        /// </summary>
        public Action OnExecute { get; set; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="DynamicActivator"/> instance
        /// </summary>
        /// <param name="activator">The activator to use</param>
        /// <param name="removeAfterFirstExecute">Indicates whether the activator should be discarded after the first activation</param>
        public DynamicActivator(IActivator activator, bool removeAfterFirstExecute) {
            Activator = activator;
            if (removeAfterFirstExecute) {
                this.removeAfterExecution = () => true;
            } else {
                this.removeAfterExecution = () => false;
            }
            this.ID = CommandContainer.UniqueDynamicActivatorID;
        }

        /// <summary>
        /// Creates a new <see cref="DynamicActivator"/> instance
        /// </summary>
        /// <param name="activator">The activator to use</param>
        /// <param name="removeAfter">The delegate which indicates whether this <see cref="DynamicActivator"/> should be removed after execution. If null <see cref="RemoveAfterExecution"/> always returns true.</param>
        public DynamicActivator(IActivator activator, Func<bool> removeAfter = null) {
            if (removeAfterExecution == null) {
                throw new Exception("This delegate cannot be null");
            }
            Activator = activator;
            this.removeAfterExecution = removeAfter;
            this.ID = CommandContainer.UniqueDynamicActivatorID;
        }

        public void Execute() {
            OnExecute?.Invoke();
            Activator.Execute();
        }

        public bool RemoveAfterExecution() {
            return removeAfterExecution();
        }

        public void OnRemove() {
        }
    }
}
