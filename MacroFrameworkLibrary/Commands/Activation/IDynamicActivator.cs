using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// An interface for adding <see cref="IActivator"/> instance based actions during runtime
    /// </summary>
    public interface IDynamicActivator {
        /// <summary>
        /// Indicates whether the operation was cancelled. If true, the activator is removed before the next execution.
        /// </summary>
        bool IsCanceled { get; set; }

        /// <summary>
        /// The activator to use
        /// </summary>
        IActivator Activator { get; }

        /// <summary>
        /// The action to execute after finishing
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
