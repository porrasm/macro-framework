using MacroFramework.Commands.Coroutines;
using System;
using System.Collections;

namespace MacroFramework.Commands {
    public abstract partial class CommandBase {

        #region fields
        internal CoroutineManager Coroutines { get; } = new CoroutineManager();
        #endregion

        #region wrappers
        /// <summary>
        /// Creates a new <see cref="Coroutine"/> instance from the given <see cref="IEnumerator"/> parameters, starts it and then returns the new coroutine.
        /// </summary>
        /// <param name="enumerator">The enumerator to use</param>
        /// <param name="onEnd">Action to run after the end or cancel of this coroutine</param>
        public Coroutine StartCoroutine(IEnumerator enumerator, Action<Coroutine> onEnd = null) {
            Coroutine coroutine = new Coroutine(enumerator, onEnd);
            return coroutine.Start(this);
        }

        /// <summary>
        /// Creates a new <see cref="Coroutine"/> instance from the given <see cref="IEnumerator"/> parameters, starts it and then returns the new coroutine.
        /// </summary>
        /// <param name="enumeratorSource">Function to get the enumerator to use</param>
        /// /// <param name="onEnd">Action to run after the end or cancel of this coroutine</param>
        public Coroutine StartCoroutine(Func<IEnumerator> enumeratorSource, Action<Coroutine> onEnd = null) {
            Coroutine coroutine = new Coroutine(enumeratorSource, onEnd);
            return coroutine.Start(this);
        }

        /// <summary>
        /// Returns true if the corotuine was running and stopped
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public bool StopCoroutine(Coroutine coroutine) {
            return Coroutines.StopCoroutine(coroutine, false);
        }

        /// <summary>
        /// Stops all coroutines running in this <see cref="Command"/> instance
        /// </summary>
        public void StopAllCoroutines() {
            Coroutines.StopAllCoroutines();
        }
        #endregion
    }
}
