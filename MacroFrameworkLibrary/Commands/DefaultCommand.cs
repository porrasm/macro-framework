using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    internal class DefaultCommand : Command {
        public override int ExecutionOrderIndex => -1;
    }
}
