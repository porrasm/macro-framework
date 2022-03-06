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
        /// Permanent set of commands which contains a single instance of every class which inherits <see cref="Command"/>. This does not change during execution.
        /// </summary>
        public static CommandGroup<Command> StaticCommands { get; private set; }

        /// <summary>
        /// A set of commands which can be changed during execution. This set can contain multiple command instances of the same type.
        /// </summary>
        public static CommandGroup<RuntimeCommand> DynamicCommands { get; private set; }

        /// <summary>
        /// Contains the active commands
        /// </summary>

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
            try {
                foreach (Command c in ReflectiveEnumerator.GetEnumerableOfType<Command>(Macros.Setup.CommandAssembly)) {
                    StaticCommands.Add(c);
                }
            } catch (Exception e) {
                throw new Exception("Could not load Commands from the given assembly. Make sure the assembly is correct. If the error persists, inherit from 'RuntimeCommand' instead of 'Command' and add it manually.", e);
            }

            Logger.Log($"Initialized command container with {StaticCommands.Count} commands");
        }

        private static void Deinitialize() {
            StaticCommands = new CommandGroup<Command>();
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

            StaticCommands.UpdateActivators(t);
            DynamicCommands.UpdateActivators(t);

            UpdateDynamicActivators(t);
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
        /// Allows you to get the list of activators
        /// </summary>
        /// <param name="filter">Optional filter</param>
        /// <returns></returns>
        public static List<IActivator> GetActivators(Func<IActivator, bool> filter = null) {
            throw new NotImplementedException();
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
        public static void ForEveryCommand(Action<CommandBase> it, bool ignoreActiveStatus, string errorMessage = "") {
            StaticCommands.ForEveryCommand(it, ignoreActiveStatus, errorMessage);
            DynamicCommands.ForEveryCommand(it, ignoreActiveStatus, errorMessage);
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

        /// <summary>
        /// Adds a command to the application
        /// </summary>
        /// <param name="c"></param>
        public static void AddCommand(RuntimeCommand c) {
            DynamicCommands.QueueAdd(c);
        }

        /// <summary>
        /// Removes a command from the application
        /// </summary>
        /// <param name="c"></param>
        public static void RemoveCommand(RuntimeCommand c) {
            DynamicCommands.QueueRemove(c);
        }
    }
}
