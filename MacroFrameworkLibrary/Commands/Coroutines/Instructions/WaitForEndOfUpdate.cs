using MacroFramework.Commands.Coroutines;

namespace MacroFramework.Commands {
    /// <summary>
    /// Coroutine instruction for waiting until the end of the main update loop
    /// </summary>
    public class WaitForEndOfUpdate : YieldInstruction {

        #region fields
        public override CoroutineUpdateGroup UpdateGroup => CoroutineUpdateGroup.OnAfterUpdate;
        #endregion

        /// <summary>
        /// Waits for the end of the this update loop
        /// </summary>
        public WaitForEndOfUpdate() { }

        public override bool MoveNext() {
            return false;
        }
    }
}
