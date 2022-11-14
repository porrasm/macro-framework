using System.Collections.Generic;

namespace MacroFramework.Commands {
    /// <summary>
    /// A container which is used in the initialize state to define all the <see cref="IActivator"/> instances a command owns.
    /// </summary>
    public struct ActivatorContainer {
        #region fields
        internal List<IActivator> Activators { get; private set; }
        #endregion

        /// <summary>
        /// Adds an activator to this group
        /// </summary>
        public void AddActivator(IActivator activator) {
            Activators.Add(activator);
        }

        /// <summary>
        /// Adds an activator to this group
        /// </summary>
        public void AddActivators(params IActivator[] activators) {
            foreach (IActivator a in activators) {
                Activators.Add(a);
            }
        }

        internal static ActivatorContainer New => new ActivatorContainer() { Activators = new List<IActivator>() };
    }
}
