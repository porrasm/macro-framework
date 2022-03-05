using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    public class CommandGroup<T> : IEnumerable<T> where T : CommandBase {
        #region fields
        internal List<T> Commands { get; private set; }
        internal Dictionary<Type, List<IActivator>> ActivatorsByType { get; private set; }

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
            Commands = new List<T>();
            ActivatorsByType = new Dictionary<Type, List<IActivator>>();
        }

        internal void Add(T c) {
            int curr = c.ExecutionOrderIndex;

            for (int i = 0; i < Commands.Count; i++) {
                int prev = Commands[i].ExecutionOrderIndex;
                if (curr > prev) {
                    Commands.Insert(i, c);
                    return;
                }
            }

            Commands.Add(c);

            foreach (IActivator act in c.Activators) {
                act.Owner = c;
                Type g = act.UpdateGroup;
                if (!ActivatorsByType.ContainsKey(g)) {
                    ActivatorsByType.Add(g, new List<IActivator>());
                }
                ActivatorsByType[g].Add(act);
            }
        }

        internal void Remove(T c) {
            Commands.Remove(c);

            foreach (IActivator act in c.Activators) {
                act.Owner = c;
                Type g = act.UpdateGroup;

                List<IActivator> typeActs = ActivatorsByType[g];
                typeActs.Remove(act);

                if (typeActs.Count == 0) {
                    ActivatorsByType.Remove(g);
                }
            }
        }

        internal void UpdateActivators(Type t) {
            List<IActivator> acts;
            if (!ActivatorsByType.TryGetValue(t, out acts)) {
                return;
            }

            foreach (IActivator act in acts) {
                if (act.IsActive()) {
                    Callbacks.ExecuteAction(act.Execute, "ExecuteActivator");
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
