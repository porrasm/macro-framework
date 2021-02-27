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
        /// The owner of this activator or null if it is used independently
        /// </summary>
        Command Owner { get; set; }
    }
}
