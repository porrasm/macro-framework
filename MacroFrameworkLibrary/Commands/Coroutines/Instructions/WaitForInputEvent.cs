using MacroFramework.Commands.Coroutines;
using MacroFramework.Input;
using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// Coroutine instruction for waiting on a specific key input. The current key event is accessible from the coroutine using <see cref="Input.InputEvents.CurrentInputEvent"/>.
    /// </summary>
    public class WaitForInputEvent : YieldInstruction {

        #region fields
        private Func<IInputEvent, bool> eventFilter;
        public override CoroutineUpdateGroup UpdateGroup => CoroutineUpdateGroup.OnInputEvent;
        #endregion

        /// <summary>
        /// Waits until the next <see cref="IInputEvent"/> is handled
        /// </summary>
        public WaitForInputEvent() {
            eventFilter = (e) => true;
        }

        /// <summary>
        /// Waits until the next matching <see cref="IInputEvent"/> is handled
        /// </summary>
        public WaitForInputEvent(Func<IInputEvent, bool> eventFilter) {
            if (eventFilter == null) {
                throw new Exception("Input event filter cannot be null");
            }
            this.eventFilter = eventFilter;
        }

        public override bool MoveNext() {
            return !eventFilter(InputEvents.CurrentInputEvent);
        }
    }
}
