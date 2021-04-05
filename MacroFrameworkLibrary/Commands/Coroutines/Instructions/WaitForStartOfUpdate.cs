using MacroFramework.Commands.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// Coroutine instruction for waiting until start of the next main update loop
    /// </summary>
    public class WaitForStartOfUpdate : YieldInstruction {

        #region fields
        public override CoroutineUpdateGroup UpdateGroup => CoroutineUpdateGroup.OnBeforeUpdate;
        #endregion

        /// <summary>
        /// Waits for the start of the next update loop
        /// </summary>
        public WaitForStartOfUpdate() { }

        public override bool MoveNext() {
            return false;
        }
    }
}
