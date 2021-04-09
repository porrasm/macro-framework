using MacroFramework.Tools;
using System;
using System.Collections;

namespace MacroFramework.Commands.Coroutines {
    /// <summary>
    /// Base class for all coroutine functionality
    /// </summary>
    public abstract class YieldInstruction : IEnumerator {

        /// <summary>
        /// The coroutine which owns this instruction. Null if this instruction is an executing coroutine.
        /// </summary>
        public Coroutine Owner { get; internal set; }

        /// <summary>
        /// The update group of this instruction
        /// </summary>
        public abstract CoroutineUpdateGroup UpdateGroup { get; }

        public object Current { get; protected set; }

        public abstract bool MoveNext();

        private long startTime;
        internal long Timeout { get; private set; }

        public YieldInstruction() {
            startTime = Timer.Milliseconds;
        }

        /// <summary>
        /// Set a timeout in milliseconds. The coroutine will be canceled if the instruction did not continue within in the time limit.
        /// </summary>
        /// <param name="timeout">Timeout in ms</param>
        /// <returns></returns>
        public YieldInstruction SetTimeout(uint timeout, TimeUnit unit = TimeUnit.Seconds) {
            this.Timeout = TimerActivator.ToMilliseconds(timeout, unit); ;
            return this;
        }

        /// <summary>
        /// Returns true if the timeout has been exceeded
        /// </summary>
        public bool TimeoutExceeded() {
            return Timeout == 0 ? false : Timer.PassedFrom(startTime) >= Timeout;
        }

        public void Reset() {
            throw new NotSupportedException("Reset is not supported on YieldInstruction");
        }
    }
}
