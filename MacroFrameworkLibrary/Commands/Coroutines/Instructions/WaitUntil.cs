using MacroFramework.Commands.Coroutines;
using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// Coroutine instruction for waiting until a specified condition becomes true
    /// </summary>
    public class WaitUntil : YieldInstruction {
        #region fields
        private Func<bool> condition;
        public override CoroutineUpdateGroup UpdateGroup => CoroutineUpdateGroup.OnTimerUpdate;
        #endregion

        /// <summary>
        /// Waits until the specified condition becomes true
        /// </summary>
        /// <param name="condition"></param>
        public WaitUntil(Func<bool> condition) {
            if (condition == null) {
                throw new Exception("Wait until condition cannot be null");
            }
            this.condition = condition;
        }

        public override bool MoveNext() {
            return !condition();
        }
    }
}
