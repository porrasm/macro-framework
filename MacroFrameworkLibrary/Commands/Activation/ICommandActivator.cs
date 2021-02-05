using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// Interface for command activators
    /// </summary>
    public interface ICommandActivator {
        /// <summary>
        /// Return true if activator is active
        /// </summary>
        /// <returns></returns>
        bool IsActive();
        /// <summary>
        /// Executes the action related to this activator
        /// </summary>
        void Execute();

        /// <summary>
        /// Owner of this activator or null if it is used independently
        /// </summary>
        Command Owner { get; set; }
    }
}
