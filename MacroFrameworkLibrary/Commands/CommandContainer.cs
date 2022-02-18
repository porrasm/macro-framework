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
        internal static List<Command> Commands { get; private set; }
        private static Dictionary<Type, List<IActivator>> commandActivatorGroups;

        private static Dictionary<Type, List<IDynamicActivator>> dynamicActivatorGroups;
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
            if (Macros.Setup.CommandAssembly != null && Macros.Setup.CommandsToUse == null) {
                try {
                    foreach (Command c in ReflectiveEnumerator.GetEnumerableOfType<Command>(Macros.Setup.CommandAssembly)) {
                        AddCommand(c);
                    }
                } catch (Exception e) {
                    throw new Exception("Could not load Commands from default assembly. Try to set Setup.CommandAssembly to null", e);
                }
            } else {
                foreach (Type t in Macros.Setup.CommandsToUse) {
                    Command c = (Command)Activator.CreateInstance(t);
                    AddCommand(c);
                }
            }

            Logger.Log($"Initialized command container with {Commands.Count} commands");
        }

        private static void AddCommand(Command c) {
            Commands.Add(c);
            foreach (IActivator act in c.Activators) {
                act.Owner = c;
                Type g = act.UpdateGroup;
                if (!commandActivatorGroups.ContainsKey(g)) {
                    commandActivatorGroups.Add(g, new List<IActivator>());
                }
                commandActivatorGroups[g].Add(act);
            }
        }

        private static void Deinitialize() {
            Commands = new List<Command>();
            commandActivatorGroups = new Dictionary<Type, List<IActivator>>();
            dynamicActivatorGroups = new Dictionary<Type, List<IDynamicActivator>>();
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
            List<IActivator> acts;
            if (!commandActivatorGroups.TryGetValue(t, out acts)) {
                return;
            }

            foreach (IActivator act in acts) {
                if (act.IsActive()) {
                    ExecuteActivator(act);
                }
            }
        }
        private static void ExecuteActivator(IActivator activator) {
            Callbacks.ExecuteAction(activator.Execute, "ExecuteActivator");
        }


        private static void UpdateDynamicActivators(Type t) {
            if (!dynamicActivatorGroups.ContainsKey(t)) {
                return;
            }

            List<IDynamicActivator> acts = dynamicActivatorGroups[t];
            for (int i = 0; i < acts.Count; i++) {
                IDynamicActivator task = acts[i];

                if (task.IsCanceled) {
                    RemoveFromList(acts, ref i);
                    continue;
                }

                if (task.Activator.IsActive()) {
                    Callbacks.ExecuteAction(task.Execute, $"Error on task finish, {task.Activator.Owner?.GetType()}");
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
        public static bool GetCommand<T>(out T command) where T : Command {
            foreach (Command c in Commands) {
                if (c.GetType() == typeof(T)) {
                    command = (T)c;
                    return true;
                }
            }

            command = default;
            return false;
        }

        /// <summary>
        /// Allows you to get the list of activators
        /// </summary>
        /// <param name="filter">Optional filter</param>
        /// <returns></returns>
        public static List<IActivator> GetActivators(Func<IActivator, bool> filter = null) {
            List<IActivator> activatorsToGet = new List<IActivator>();
            foreach (IActivator act in commandActivatorGroups.Values) {
                if (filter?.Invoke(act) ?? true) {
                    activatorsToGet.Add(act);
                }
            }
            return activatorsToGet;
        }

        /// <summary>
        /// Allows you to get the list of current dynamic activators
        /// </summary>
        /// <param name="filter">Optional filter</param>
        /// <returns></returns>
        public static List<IDynamicActivator> GetDynamicActivators(Func<IDynamicActivator, bool> filter = null) {
            List<IDynamicActivator> activatorsToGet = new List<IDynamicActivator>();
            foreach (IDynamicActivator act in dynamicActivatorGroups.Values) {
                if (filter?.Invoke(act) ?? true) {
                    activatorsToGet.Add(act);
                }
            }
            return activatorsToGet;
        }

        /// <summary>
        /// Can be used to add <see cref="IActivator"/> (wrapped inside <see cref="IDynamicActivator"/>) instances to the framework during runtime. Useful for e.g. events that should run only once.
        /// </summary>
        /// <param name="act">The dynamic activator to add</param>
        public static IDynamicActivator AddDynamicActivator(IActivator act) => AddDynamicActivator(new DynamicActivator(act));

        /// <summary>
        /// Can be used to add <see cref="IActivator"/> (wrapped inside <see cref="IDynamicActivator"/>) instances to the framework during runtime. Useful for e.g. events that should run only once.
        /// </summary>
        /// <param name="act">The dynamic activator to add</param>
        public static IDynamicActivator AddDynamicActivator(IDynamicActivator act) {
            Type t = act.Activator.UpdateGroup;
            if (dynamicActivatorGroups.ContainsKey(t)) {
                dynamicActivatorGroups[t].Add(act);
            } else {
                dynamicActivatorGroups.Add(t, new List<IDynamicActivator>());
                dynamicActivatorGroups[t].Add(act);
            }
            return act;
        }

        /// <summary>
        /// Executes some action for every command in a try clause
        /// </summary>
        /// <param name="it">The action to complete</param>
        /// <param name="ignoreActiveStatus">Whether to ignroe the <see cref="Command.IsActive"/> status</param>
        /// <param name="errorMessage">The error message to log should an error occur</param>
        public static void ForEveryCommand(Action<Command> it, bool ignoreActiveStatus, string errorMessage = "") {
            foreach (Command c in Commands) {
                if (ignoreActiveStatus || c.IsActive()) {
                    Callbacks.ExecuteAction(it, c, $"Error iterating command with type {c.GetType()}: {errorMessage}");
                }
            }
        }

        /// <summary>
        /// Removes a <see cref="IDynamicActivator"/> instance from the active list. Returns true if the element existed and was removed.
        /// </summary>
        public static bool RemoveDynamicActivator(IDynamicActivator activator) {
            foreach (var activators in dynamicActivatorGroups.Values) {
                for (int i = 0; i < activators.Count; i++) {
                    if (activators[i].ID == activator.ID) {
                        activators.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
