using MacroFramework.Commands.Coroutines;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// Coroutine instruction for waiting for a specific amount of time
    /// </summary>
    public class WaitFor : YieldInstruction {

        #region fields
        private long startTime;
        private long waitTime;
        public override CoroutineUpdateGroup UpdateGroup => CoroutineUpdateGroup.OnTimerUpdate;
        #endregion

        /// <summary>
        /// Waits for a specified amount of time
        /// </summary>
        /// <param name="amount">The amount of time</param>
        /// <param name="unit">The unit of time</param>
        public WaitFor(int amount, TimeUnit unit = TimeUnit.Seconds) {
            waitTime = TimerActivator.ToMilliseconds(amount, unit);
            startTime = Timer.Milliseconds;
        }

        public override bool MoveNext() {
            return Timer.PassedFrom(startTime) < waitTime;
        }
    }
}
