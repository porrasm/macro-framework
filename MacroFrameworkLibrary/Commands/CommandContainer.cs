using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// A static class for handling all active <see cref="Command"/> instances
    /// </summary>
    public static class CommandContainer {

        #region fields
        /// <summary>
        /// List of active commands. You should not modify this collection.
        /// </summary>
        public static List<Command> Commands { get; private set; }
        private static Dictionary<Type, List<IActivator>> TypeActivators { get; set; }
        #endregion

        static CommandContainer() {
            Deinitialize();
        }

        private static void Initialize() {
            List<Command> setupCommands = Setup.Instance.GetActiveCommands();
            if (Setup.Instance.CommandAssembly != null && setupCommands == null) {
                foreach (Command c in ReflectiveEnumerator.GetEnumerableOfType<Command>(Setup.Instance.CommandAssembly)) {
                    AddCommand(c);
                }
            } else {
                foreach (Command c in setupCommands) {
                    AddCommand(c);
                }
            }
        }

        private static void Deinitialize() {
            Commands = new List<Command>();
            TypeActivators = new Dictionary<Type, List<IActivator>>();
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
        /// <typeparam name="T">The type to update which implement <see cref="IActivator"/>"/></param></typeparam>
        public static void UpdateActivators<T>() where T : IActivator {
            UpdateActivators(typeof(T));
        }

        private static void UpdateActivators(Type t) {

            if (Macros.Paused) {
                return;
            }

            if (!typeof(IActivator).IsAssignableFrom(t)) {
                throw new NotSupportedException("Invalid type argument given: " + t);
            }
            if (!TypeActivators.ContainsKey(t)) {
                return;
            }

            foreach (IActivator act in TypeActivators[t]) {
                if (act.IsActive()) {
                    try {
                        Logger.Log("Try clause open");
                        act.Execute();
                        Logger.Log("Try clause end");
                    } catch (Exception e) {
                        Logger.Log("Error executing command of type " + act.Owner.GetType() + ": " + e.Message);
                    }
                }
            }
        }

        internal static void Exit() {
            foreach (Command c in Commands) {
                c.OnClose();
            }
            Deinitialize();
        }

        internal static void Start() {
            Initialize();
            foreach (Command c in Commands) {
                c.OnStart();
            }
        }

        /// <summary>
        /// Adds a command to the active command pool
        /// </summary>
        /// <param name="c"></param>
        public static void AddCommand(Command c) {
            Commands.Add(c);
            AddActivators(c);
        }

        private static void AddActivators(Command c) {
            foreach (IActivator act in c.CommandActivators.Activators) {
                Type t = act.GetType();
                if (TypeActivators.ContainsKey(t)) {
                    TypeActivators[t].Add(act);
                } else {
                    TypeActivators.Add(t, new List<IActivator>());
                    TypeActivators[t].Add(act);
                }
            }
        }
    }
}
