using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands.Activation {
    /// <summary>
    /// Interface for command activators
    /// </summary>
    public interface ICommandActivator {
        /// <summary>
        /// Return true if activator is active
        /// </summary>
        /// <returns></returns>
        bool IsActive();
        void Execute();
    }
}
