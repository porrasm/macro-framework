using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands.Activation {
    public interface ICommandActivator {
        bool IsActive();
        void Execute();
    }
}
