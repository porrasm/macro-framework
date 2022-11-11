using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// Interface for all command activators
    /// </summary>
    public interface IActivator {
        /// <summary>
        /// Returns true if activator is active
        /// </summary>
        /// <returns></returns>
        bool IsActive();
        /// <summary>
        /// Executes the action related to this activator
        /// </summary>
        void Execute();

        /// <summary>
        /// This activator will be updated with this update group when <see cref="CommandContainer.UpdateActivators(Type)"/> is called
        /// </summary>
        Type UpdateGroup { get; }
    }
}
