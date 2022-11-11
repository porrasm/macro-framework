using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    public class CommandGroup<T> : IEnumerable<T> where T : CommandBase {
        internal struct ActivatorWithOwner {
            public IActivator Activator;
            public CommandBase Owner;
        }

        #region fields
        internal List<T> Commands { get; private set; }
        internal Dictionary<Type, List<ActivatorWithOwner>> ActivatorsByType { get; private set; }
        private Queue<T> addQueue;
        private Queue<T> removeQueue;

        /// <summary>
        /// Returns the amount of commands
        /// </summary>
        public int Count => Commands.Count;
        #endregion

        internal CommandGroup() {
            Initialize();
        }

        internal void Dispose() {
            foreach (T c in Commands) {
                c.Dispose();
            }
            Initialize();
        }

        private void Initialize() {
            removeQueue = new Queue<T>();
            Commands = new List<T>();
            ActivatorsByType = new Dictionary<Type, List<ActivatorWithOwner>>();
        }

        internal void QueueAdd(T c) {
            if (addQueue.Contains(c)) {
                return;
            }
            addQueue.Enqueue(c);
        }
        internal void QueueRemove(T c) {
            if (removeQueue.Contains(c)) {
                return;
            }
            removeQueue.Enqueue(c);
        }

        internal void Update() {
            while (removeQueue.Count > 0) {
                Remove(removeQueue.Dequeue());
            }
            while (addQueue.Count > 0) {
                Add(addQueue.Dequeue());
            }
        }

        internal void Add(T command) {
            int curr = command.ExecutionOrderIndex;

            for (int i = 0; i < Commands.Count; i++) {
                int prev = Commands[i].ExecutionOrderIndex;
                if (curr > prev) {
                    Commands.Insert(i, command);
                    return;
                }
            }

            Commands.Add(command);

            foreach (IActivator act in command.Activators) {
                ActivatorWithOwner withOwner = new ActivatorWithOwner() {
                    Activator = act,
                    Owner = command
                };


                Type g = act.UpdateGroup;
                if (!ActivatorsByType.ContainsKey(g)) {
                    ActivatorsByType.Add(g, new List<ActivatorWithOwner>());
                }
                ActivatorsByType[g].Add(withOwner);
            }

            command.OnStart();
        }
        private void Remove(T command) {
            Commands.Remove(command);

            foreach (IActivator act in command.Activators) {
                Type g = act.UpdateGroup;

                List<ActivatorWithOwner> typeActs = ActivatorsByType[g];
                typeActs.RemoveAll((a) => a.Owner == command);

                if (typeActs.Count == 0) {
                    ActivatorsByType.Remove(g);
                }
            }

            command.Dispose();
        }

        internal void UpdateActivators(Type t) {
            List<ActivatorWithOwner> acts;
            if (!ActivatorsByType.TryGetValue(t, out acts)) {
                return;
            }

            foreach (ActivatorWithOwner act in acts) {
                if (act.Activator.IsActive()) {
                    Callbacks.ExecuteAction(act.Activator.Execute, "ExecuteActivator");
                }
            }
        }


        /// <summary>
        /// Can be used to get retrieve a command of type <see cref="ST"/> by its type
        /// </summary>
        /// <param name="t">The type of command to get</param>
        /// <param name="command">The command</param>
        /// <returns></returns>
        public bool GetCommand<ST>(out ST command) where ST : T {
            foreach (T c in Commands) {
                if (c is ST subCommand) {
                    command = subCommand;
                    return true;
                }
            }

            command = default;
            return false;
        }

        /// <summary>
        /// Can be used to get retrieve a list of commands of type <see cref="ST"/> by their type
        /// </summary>
        /// <param name="t">The type of command to get</param>
        /// <param name="command">The command</param>
        /// <returns></returns>
        public List<ST> GetCommands<ST>() where ST : T {
            List<ST> commands = new List<ST>();
            foreach (T c in Commands) {
                if (c is ST subCommand) {
                    commands.Add(subCommand);
                }
            }
            return commands;
        }

        /// <summary>
        /// Executes some action for every command in a try clause
        /// </summary>
        /// <param name="it">The action to complete</param>
        /// <param name="ignoreActiveStatus">Whether to ignroe the <see cref="Command.IsActive"/> status</param>
        /// <param name="errorMessage">The error message to log should an error occur</param>
        public void ForEveryCommand(Action<T> it, bool ignoreActiveStatus, string errorMessage = "") {
            foreach (T c in Commands) {
                if (ignoreActiveStatus || c.IsActive()) {
                    Callbacks.ExecuteAction(it, c, $"Error iterating command with type {c.GetType()}: {errorMessage}");
                }
            }
        }

        public IEnumerator<T> GetEnumerator() {
            return Commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
