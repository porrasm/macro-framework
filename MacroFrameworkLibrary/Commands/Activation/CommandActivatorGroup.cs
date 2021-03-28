using System;
using System.Collections.Generic;

namespace MacroFramework.Commands {
    /// <summary>
    /// A wrapper class for storing multiple <see cref="IActivator"/> instances
    /// </summary>
    public class CommandActivatorGroup {

        #region fields
        private Command owner;
        /// <summary>
        /// The list of all <see cref="IActivator"/> instances owner by this class.
        /// </summary>
        internal List<IActivator> Activators { get; }
        public IActivator[] GetActivators() {
            return Activators.ToArray();
        }
        #endregion

        /// <summary>
        /// Creates a new <see cref="CommandActivatorGroup"/> instance
        /// </summary>
        /// <param name="owner">The owner command class of this activator group</param>
        public CommandActivatorGroup(Command owner) {
            if (owner == null) {
                throw new Exception("A command activator group must have an owner");
            }
            this.owner = owner;
            Activators = new List<IActivator>();
        }

        /// <summary>
        /// Adds an activator to the group and sets the <see cref="IActivator.Owner"/> of the activator
        /// </summary>
        /// <param name="activator">The activator to add</param>
        public void Add(IActivator activator) {
            activator.Owner = owner;
            Activators.Add(activator);
        }

        /// <summary>
        /// Returns true if any of the <see cref="CommandActivator"/> instances is active
        /// </summary>
        /// <param name="activeActivator"></param>
        /// <returns></returns>
        public bool IsActive(out CommandActivator activeActivator) {
            foreach (CommandActivator act in Activators) {
                if (act.IsActive()) {
                    activeActivator = act;
                    return true;
                }
            }
            activeActivator = null;
            return false;
        }
    }
}