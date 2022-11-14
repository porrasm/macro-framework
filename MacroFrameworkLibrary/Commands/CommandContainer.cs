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
        /// Empty command instance required for e.g. running coroutines without a command. Has no functionality. Has an <see cref="CommandBase.ExecutionOrderIndex"/> of -1.
        /// </summary>
        public static Command DefaultCommand { get; private set; }

        /// <summary>
        /// Permanent set of commands which contains a single instance of every class which inherits <see cref="Command"/>. This does not change during execution.
        /// </summary>
        public static CommandGroup<Command> StaticCommands { get; private set; }

        /// <summary>
        /// A set of commands which can be changed during execution. This set can contain multiple command instances of the same type.
        /// </summary>
        public static CommandGroup<RuntimeCommand> DynamicCommands { get; private set; }
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
                if (Macros.Setup.CommandAssembly != null) {
                    foreach (Command c in ReflectiveEnumerator.GetEnumerableOfType<Command>(Macros.Setup.CommandAssembly)) {
                        StaticCommands.Add(c);
                    }
                }
            } catch (Exception e) {
                throw new Exception("Could not load Commands from the given assembly. Make sure the assembly is correct. If the error persists, inherit from 'RuntimeCommand' instead of 'Command' and add it manually.", e);
            }

            bool hasDefaultCommand = StaticCommands.GetCommand<DefaultCommand>(out var defaultCommand);
            if (!hasDefaultCommand) {
                DefaultCommand = new DefaultCommand();
                StaticCommands.Add(DefaultCommand);
            } else {
                DefaultCommand = defaultCommand;
            }

            Logger.Log($"Initialized command container with {StaticCommands.Count} commands");
        }

        private static void Deinitialize() {
            StaticCommands = new CommandGroup<Command>();
            DynamicCommands = new CommandGroup<RuntimeCommand>();
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

        /// <summary>
        /// Gets a static command of type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public static void GetCommand<T>(out T command) where T : Command {
            bool succes = StaticCommands.GetCommand(out command);
            if (!succes) {
                throw new Exception("Unexpected error: Could not find static command of type " + typeof(T));
            }
        }
    }
}
