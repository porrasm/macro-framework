using MacroFramework.Commands.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace MacroFramework.Commands {
    /// <summary>
    /// A class for executing coroutines
    /// </summary>
    public sealed class Coroutine : IEnumerator {

        #region fields
        private Action<Coroutine> onEnd;
        private IEnumerator enumerator;
        internal YieldInstruction CurrentInstruction { get; private set; }

        /// <summary>
        /// Unique ID for the coroutine
        /// </summary>
        public int ID { get; }

        private static int uniqueID;
        internal bool Consumed { get; set; }

        /// <summary>
        /// Indicates whether or not this coroutine is running
        /// </summary>
        public bool IsRunning { get; internal set; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="Coroutine"/> instance
        /// </summary>
        /// <param name="enumerator">The enumerator to use</param>
        /// <param name="onEnd">The action to call after the coroutine ends or is cancelled</param>
        public Coroutine(IEnumerator enumerator, Action<Coroutine> onEnd = null) {
            if (enumerator == null) {
                throw new Exception("Enumerator can't be null");
            }
            this.enumerator = enumerator;
            this.ID = ++uniqueID;
            this.onEnd = onEnd;
        }

        private YieldInstruction ObjectToYieldInstruction(object o) {
            if (o == null) {
                YieldInstruction inst = new WaitForStartOfUpdate();
                inst.Owner = this;
                return inst;
            }

            if (o is int) {
                throw new NotImplementedException();
            }

            if (o is YieldInstruction) {
                YieldInstruction inst = (YieldInstruction)o;
                inst.Owner = this;
                return inst;
            }

            if (o is Coroutine) {
                throw new NotImplementedException();
                // return new WaitForCoroutine((Coroutine)o);
            }

            throw new Exception("Invalid value returned from coroutine");
        }

        internal void Finish() {
            onEnd?.Invoke(this);
        }

        #region enumerator
        object IEnumerator.Current => throw new NotImplementedException();

        public bool MoveNext() {
            if (!enumerator.MoveNext()) {
                return false;
            }

            CurrentInstruction = ObjectToYieldInstruction(enumerator.Current);

            return true;
        }

        public void Reset() {
            throw new NotSupportedException("Coroutine does not support reset");
        }
        #endregion

        public override bool Equals(object obj) {
            return obj is Coroutine coroutine &&
                   ID == coroutine.ID;
        }

        public override int GetHashCode() {
            return 1213502048 + ID.GetHashCode();
        }
    }
}
