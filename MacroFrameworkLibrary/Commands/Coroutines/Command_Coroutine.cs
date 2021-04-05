using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MacroFramework.Commands.Coroutines;

namespace MacroFramework.Commands {
    public abstract partial class Command {

        #region fields
        internal CoroutineManager Coroutines { get; } = new CoroutineManager();
        #endregion

        #region wrappers
        /// <summary>
        /// Creates a new <see cref="Coroutine"/> instance from the given <see cref="IEnumerator"/> parameters, starts it and then returns the new coroutine.
        /// </summary>
        /// <param name="enumerator">The enumerator to use</param>
        /// <returns></returns>
        public Coroutine StartCoroutine(IEnumerator enumerator) {
            Coroutine coroutine = new Coroutine(enumerator);
            Coroutines.StartCoroutine(coroutine);
            return coroutine;
        }

        /// <summary>
        /// Starts a coroutine
        /// </summary>
        /// <param name="coroutine">The coroutine to start</param>
        public void StartCoroutine(Coroutine coroutine) {
            Coroutines.StartCoroutine(coroutine);
        }

        /// <summary>
        /// Returns true if the corotuine was running and stopped
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public bool StopCoroutine(Coroutine coroutine) {
            return Coroutines.StopCoroutine(coroutine);
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
