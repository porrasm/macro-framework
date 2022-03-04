using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands.Coroutines {
    public class CoroutineInstruction : YieldInstruction {
        #region fields
        private Coroutine coroutine;
        #endregion

        public CoroutineInstruction(Coroutine coroutine) {
            this.coroutine = coroutine;
        }

        public override CoroutineUpdateGroup UpdateGroup => CoroutineUpdateGroup.OnTimerUpdate;

        public override bool MoveNext() {
            return !coroutine.IsRunning;
        }
    }
}
