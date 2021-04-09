namespace MacroFramework.Commands {
    /// <summary>
    /// An interface for adding <see cref="IActivator"/> instance based actions during runtime
    /// </summary>
    public interface IDynamicActivator {
        /// <summary>
        /// Unique ID of the dynamic activator
        /// </summary>
        uint ID { get; }

        /// <summary>
        /// Indicates whether the operation was cancelled. If true, the activator is removed before the next execution.
        /// </summary>
        bool IsCanceled { get; set; }

        /// <summary>
        /// The activator to use
        /// </summary>
        IActivator Activator { get; }

        /// <summary>
        /// The action to execute whenever the <see cref="Activator"/> is active
        /// </summary>
        void Execute();

        /// <summary>
        /// Executed on remove to perform cleanup
        /// </summary>
        void OnRemove();

        /// <summary>
        /// Indicates whether the activator should be removed from the list of active activators. If this returns true the activator is removed after execution.
        /// </summary>
        bool RemoveAfterExecution();
    }
}
