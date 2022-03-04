using MacroFramework.Commands.Coroutines;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// A class for executing coroutines
    /// </summary>
    public sealed class Coroutine : IEnumerator {

        #region fields
        /// <summary>
        /// Function for getting the enumerator. Necessary for restarting of resetting the coroutine.
        /// </summary>
        public Func<IEnumerator> EnumeratorSource { get; set; }

        private CommandBase owner;
        private Action<Coroutine> onEnd;
        private IEnumerator enumerator;
        internal YieldInstruction CurrentInstruction { get; private set; }
        public object Current => null;

        /// <summary>
        /// Unique ID for the coroutine
        /// </summary>
        public int ID { get; }

        private static int uniqueID;

        /// <summary>
        /// If true the coroutine will not be updated
        /// </summary>
        public bool IsPaused { get; set; }

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

        /// <summary>
        /// Creates a new <see cref="Coroutine"/> instance
        /// </summary>
        /// <param name="enumeratorSource">The enumerator source to use</param>
        /// <param name="onEnd">The action to call after the coroutine ends or is cancelled</param>
        public Coroutine(Func<IEnumerator> enumeratorSource, Action<Coroutine> onEnd = null) {
            this.EnumeratorSource = enumeratorSource;
            this.enumerator = enumeratorSource();
            this.ID = ++uniqueID;
            this.onEnd = onEnd;
        }

        #region management
        /// <summary>
        /// Starts this coroutine
        /// </summary>
        public Coroutine Start() {
            return StartThisCoroutine();
        }

        /// <summary>
        /// Starts this coroutine in the given <see cref="Command"/> class
        /// </summary>
        /// <param name="owner"></param>
        public Coroutine Start(CommandBase owner) {
            SetOwner(owner);
            return StartThisCoroutine();
        }

        /// <summary>
        /// Stops the coroutine, resets it and restarts it. Must have defined <see cref="EnumeratorSource"/>.
        /// </summary>
        public void Restart() {
            Stop();
            Reset();
            StartThisCoroutine();
        }

        /// <summary>
        /// Stops the coroutine
        /// </summary>
        /// <returns></returns>
        public bool Stop() {
            return owner.Coroutines.StopCoroutine(this, false);
        }

        /// <summary>
        /// Set the owner of this coroutine
        /// </summary>
        public void SetOwner(CommandBase owner) {
            if (owner == null) {
                throw new Exception("Owner cannot be null");
            }
            this.owner = owner;
        }

        internal Coroutine StartThisCoroutine() {
            if (IsRunning) {
                throw new Exception("Coroutine already running");
            }

            if (enumerator == null) {
                throw new Exception("Cannot start with a null enumerator");
            }

            owner.Coroutines.StartCoroutine(this);
            return this;
        }

        internal void Finish() {
            onEnd?.Invoke(this);
        }
        #endregion

        #region enumerator

        public bool MoveNext() {

            if (!enumerator.MoveNext()) {
                return false;
            }

            CurrentInstruction = ObjectToYieldInstruction(enumerator.Current);

            return true;
        }

        /// <summary>
        /// Resets this coroutine. Must have defined <see cref="EnumeratorSource"/>.
        /// </summary>
        public void Reset() {
            if (IsRunning) {
                Stop();
            }
            this.enumerator = EnumeratorSource();
        }
        #endregion

        #region utility
        private YieldInstruction ObjectToYieldInstruction(object o) {
            if (o == null) {
                YieldInstruction inst = new WaitForStartOfUpdate();
                inst.Owner = this;
                return inst;
            }

            if (o is int) {
                return new WaitFor((int)o, TimeUnit.Milliseconds);
            }

            if (o is YieldInstruction) {
                YieldInstruction inst = (YieldInstruction)o;
                inst.Owner = this;
                return inst;
            }

            if (o is Coroutine) {
                return new CoroutineInstruction((Coroutine)o);
            }

            if (o is Task) {
                return new TaskInstruction((Task)o);
            }

            throw new Exception("Invalid value returned from coroutine");
        }

        public override bool Equals(object obj) {
            return obj is Coroutine coroutine &&
                   ID == coroutine.ID;
        }

        public override int GetHashCode() {
            return 1213502048 + ID.GetHashCode();
        }
        #endregion

        public static YieldInstruction Async(Func<Task> task) => new TaskInstruction(task());
    }
}
