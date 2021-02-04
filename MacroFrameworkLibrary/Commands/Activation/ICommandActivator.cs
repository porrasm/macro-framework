using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands.Activation {
    /// <summary>
    /// Interface for command activators
    /// </summary>
    public interface ICommandActivator {
        bool IsActive();
        void Execute();
    }
}
