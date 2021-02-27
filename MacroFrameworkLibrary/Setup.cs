using MacroFramework.Commands;
using System.Collections.Generic;
using System.Reflection;

namespace MacroFramework {
    /// <summary>
    /// Inherit this class in your project to finish the setup process
    /// </summary>
    public abstract class Setup {

        #region fields
        /// <summary>
        /// The current singleton setup instance
        /// </summary>
        public static Setup Instance { get; private set; }

        internal static void SetInstance(Setup setup) {
            Setup.Instance = setup;
        }

        /// <summary>
        /// The assembly in which your custom <see cref="Command"/> classes reside in (can be null).
        /// </summary>
        public Assembly CommandAssembly { get; private set; }

        /// <summary>
        /// The settings you wish to use
        /// </summary>
        public MacroSettings Settings { get; private set; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="Setup"/> instance
        /// </summary>
        public Setup() {
            CommandAssembly = GetMainAssembly();
            Settings = GetSettings();
        }

        /// <summary>
        /// Set the <see cref="MacroSettings"/> to fit your needs here
        /// </summary>
        /// <returns></returns>
        protected virtual MacroSettings GetSettings() {
            return new MacroSettings();
        }

        /// <summary>
        /// Return the <see cref="Assembly"/> in which your <see cref="Command"/> instances reside in (most likely <see cref="Assembly.GetExecutingAssembly"/>). This allows automatically enabling your <see cref="Commands.Command"/>. Return null if you don't wish to use this feature. Add commands manually either using <see cref="CommandContainer.AddCommand(Command)"/> or define a list of them with <see cref="GetActiveCommands"/>/>
        /// </summary>
        /// <returns></returns>
        protected virtual Assembly GetMainAssembly() {
            return null;
        }

        /// <summary>
        /// Initialize active commands here. Return null if you defined the main assembly in <see cref="GetMainAssembly"/> or if you wish to use <see cref="CommandContainer.AddCommand(Command)"/>
        /// </summary>
        /// <returns></returns>
        internal protected virtual List<Command> GetActiveCommands() {
            return null;
        }

        /// <summary>
        /// Use a custom <see cref="ILogger"/>
        /// </summary>
        /// <returns></returns>
        internal protected virtual ILogger GetLogger() {
            return new Logger();
        }
    }
}
