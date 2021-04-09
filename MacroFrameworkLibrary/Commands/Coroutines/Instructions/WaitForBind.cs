using MacroFramework.Commands.Coroutines;

namespace MacroFramework.Commands {
    /// <summary>
    /// Coroutine instruction for waiting until a <see cref="Bind"/> becomes active
    /// </summary>
    public class WaitForBind : YieldInstruction {
        #region fields
        private BindActivator bindActivator;
        public override CoroutineUpdateGroup UpdateGroup => CoroutineUpdateGroup.OnInputEvent;
        #endregion

        /// <summary>
        /// Waits until a certain bind is held down
        /// </summary>
        public WaitForBind(Bind bind) {
            this.bindActivator = new BindActivator(bind);
        }

        public override bool MoveNext() {
            return !bindActivator.IsActive();
        }
    }
}
