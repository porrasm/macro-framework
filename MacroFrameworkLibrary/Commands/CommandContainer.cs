using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    public static class CommandContainer {

        #region fields
        public static List<Command> Commands { get; private set; }

        private static Queue<QueueCallback> queueCallbacks;

        public delegate void QueueCallback();
        #endregion

        static CommandContainer() {
            Deinitialize();
        }

        private static void Initialize() {
            if (Macros.macroAssembly != null) {
                foreach (var c in ReflectiveEnumerator.GetEnumerableOfType<Command>(Macros.macroAssembly)) {
                    Commands.Add(c);
                }
            }

            queueCallbacks = new Queue<QueueCallback>();
        }

        private static void Deinitialize() {
            Commands = new List<Command>();
            queueCallbacks = null;
        }

        /// <summary>
        /// Executes all commands and binds which are active.
        /// </summary>
        public static void ExecuteCommands() {
            foreach (Command c in Commands) {
                c.ExecuteIfActive();
            }
        }

        public static void OnClose() {
            foreach (Command c in Commands) {
                c.OnClose();
            }
        }

        /// <summary>
        /// Queues a job which will be executed on the primary thread. You queue a job from a secondary thread.
        /// </summary>
        /// <param name="cb"></param>
        public static void EnqueuePrimaryThreadJob(QueueCallback cb) {
            queueCallbacks.Enqueue(cb);
        }


        public static void ExecuteSecondaryThreadJobs() {
            while (queueCallbacks.Count > 0) {
                queueCallbacks.Dequeue()?.Invoke();
            }
        }

        internal static void Start() {
            Initialize();
            foreach (Command c in Commands) {
                c.OnStart();
            }
        }
        internal static void Exit() {
            foreach (Command c in Commands) {
                c.OnClose();
            }
            Deinitialize();
        }

        /// <summary>
        /// Adds a command to the active command pool
        /// </summary>
        /// <param name="c"></param>
        public static void AddCommand(Command c) {
            Commands.Add(c);
        }
    }
}
