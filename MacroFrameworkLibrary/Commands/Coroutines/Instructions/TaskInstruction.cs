using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands.Coroutines {
    public class TaskInstruction : YieldInstruction {
        #region fields
        private Task task;
        #endregion

        public TaskInstruction(Task task) {
            this.task = task;
        }

        public override CoroutineUpdateGroup UpdateGroup => CoroutineUpdateGroup.OnTimerUpdate;

        public override bool MoveNext() {
            return task.IsCompleted;
        }
    }
}
