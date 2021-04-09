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

        public void Reset() {
            throw new NotSupportedException("Reset is not supported on YieldInstruction");
        }
    }
}
