using MacroFramework.Tools;
using System;
using System.Collections.Generic;

namespace MacroFramework.Commands {
    /// <summary>
    /// A static class for handling all active <see cref="Command"/> instances
    /// </summary>
    public static class CommandContainer {

        #region fields
        /// <summary>
        /// Contains the active commands
        /// </summary>
        internal static Dictionary<Type, Command> Commands { get; private set; }

        private static Dictionary<Type, List<IDynamicActivator>> dynamicActivators;
        private static uint dynamicActivatorID;
        internal static uint UniqueDynamicActivatorID => ++dynamicActivatorID;
        #endregion

        static CommandContainer() {
            Deinitialize();
        }

        internal static void Start() {
            Initialize();
            ForEveryCommand((c) => c.OnStart(), true, "OnStart");
        }

        internal static void Exit() {
            ForEveryCommand((c) => c.Dispose(), true, "OnClose");
            Deinitialize();
        }

        private static void Initialize() {
            HashSet<Type> setupCommands = Setup.Instance.GetActiveCommands();
            if (Setup.Instance.CommandAssembly != null && setupCommands == null) {
                foreach (Command c in ReflectiveEnumerator.GetEnumerableOfType<Command>(Setup.Instance.CommandAssembly)) {
                    Commands.Add(c.GetType(), c);
                }
            } else {
                foreach (Type t in setupCommands) {
                    Command c = (Command)Activator.CreateInstance(t);
                    Commands.Add(t, c);
                }
            }
        }

        private static void Deinitialize() {
            Commands = new Dictionary<Type, Command>();
            dynamicActivators = new Dictionary<Type, List<IDynamicActivator>>();
        }

        /// <summary>
        /// Executes all activatos of certain type. This may call multiple activators from a single command instance.
        /// </summary>
        /// <param name="types">The list of types to update which implement <see cref="IActivator"/>"/></param>
        public static void UpdateActivators(params Type[] types) {
            if (types == null) {
                return;
            }
            foreach (Type t in types) {
                UpdateActivators(t);
            }
        }
        /// <summary>
        /// Executes all activatos of certain type. This may call multiple activators from a single command instance.
        /// </summary>
        /// <typeparam name="T">The type to update which implement <see cref="IActivator"/></typeparam>
        public static void UpdateActivators<T>() where T : IActivator {
            UpdateActivators(typeof(T));
        }

        private static void UpdateActivators(Type t) {
            if (Macros.State == Macros.RunState.Paused) {
                return;
            }

            if (!typeof(IActivator).IsAssignableFrom(t)) {
                throw new NotSupportedException("Invalid type argument given: " + t);
            }

            UpdateStaticActivators(t);
            UpdateDynamicActivators(t);
        }

        private static void UpdateStaticActivators(Type t) {
            Command c;
            if (!Commands.TryGetValue(t, out c)) {
                return;
            }

            foreach (IActivator act in c.Activators) {
                if (act.IsActive()) {
                    ExecuteActivator(act);
                }
            }
        }
        private static void ExecuteActivator(IActivator activator) {
            try {
                activator.Execute();
            } catch (Exception e) {
                Logger.Exception(e, "ExecuteActivator");
            }
        }


        private static void UpdateDynamicActivators(Type t) {
            if (!dynamicActivators.ContainsKey(t)) {
                return;
            }

            List<IDynamicActivator> acts = dynamicActivators[t];
            for (int i = 0; i < acts.Count; i++) {
                IDynamicActivator task = acts[i];

                if (task.IsCanceled) {
                    RemoveFromList(acts, ref i);
                    continue;
                }

                if (task.Activator.IsActive()) {
                    try {
                        task.Execute();
                    } catch (Exception e) {
                        Logger.Exception(e, $"Error on task finish, {task.Activator.Owner?.GetType()}");
                    }
                    if (task.RemoveAfterExecution()) {
                        RemoveFromList(acts, ref i);
                    }
                }
            }
        }
        private static void RemoveFromList<T>(List<T> list, ref int index) {
            list.RemoveAt(index);
            index--;
        }


        /// <summary>
        /// Can be used to get retrieve a <see cref="Command"/> by its type
        /// </summary>
        /// <param name="t">The type of command to get</param>
        /// <param name="command">The command</param>
        /// <returns></returns>
        public static bool GetCommand(Type t, out Command command) {
            return Commands.TryGetValue(t, out command);
        }

        /// <summary>
        /// Can be used to add <see cref="IActivator"/> (wrapped inside <see cref="IDynamicActivator"/>) instances to the framework during runtime. Useful for e.g. events that should run only once.
        /// </summary>
        /// <param name="act">The dynamic activator to add</param>
        public static void AddDynamicActivator(IDynamicActivator act) {
            Type t = act.Activator.UpdateGroup;
            if (dynamicActivators.ContainsKey(t)) {
                dynamicActivators[t].Add(act);
            } else {
                dynamicActivators.Add(t, new List<IDynamicActivator>());
                dynamicActivators[t].Add(act);
            }
        }

        /// <summary>
        /// Executes some action for every command in a try clause
        /// </summary>
        /// <param name="it">The action to complete</param>
        /// <param name="ignoreActiveStatus">Whether to ignroe the <see cref="Command.IsActive"/> status</param>
        /// <param name="errorMessage">The error message to log should an error occur</param>
        public static void ForEveryCommand(Action<Command> it, bool ignoreActiveStatus, string errorMessage = "") {
            foreach (Command c in Commands.Values) {
                try {
                    if (ignoreActiveStatus || c.IsActive()) {
                        it(c);
                    }
                } catch (Exception e) {
                    Logger.Exception(e, $"Error iterating command with type {c.GetType()}: {errorMessage}");
                }
            }
        }

        /// <summary>
        /// Removes a <see cref="IDynamicActivator"/> instance from the active list. Returns true if the element existed and was removed.
        /// </summary>
        public static bool RemoveDynamicActivator(uint id) {
            foreach (var activators in dynamicActivators.Values) {
                for (int i = 0; i < activators.Count; i++) {
                    if (activators[i].ID == id) {
                        activators.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
