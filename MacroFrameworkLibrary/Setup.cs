using MacroFramework.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MacroFramework {
    /// <summary>
    /// Inherit this class in your project to finish the setup process
    /// </summary>
    public class Setup {

        #region fields
        /// <summary>
        /// The assembly in which your custom <see cref="Command"/> classes reside in (can be null).
        /// </summary>
        public Assembly CommandAssembly { get; set; }

        /// <summary>
        /// The settings you wish to use
        /// </summary>
        public MacroSettings Settings { get; set; }

        /// <summary>
        /// Leave this untouched if you wish to use <see cref="System.Reflection"/> for automatically creating commands
        /// </summary>
        public HashSet<Type> CommandsToUse { get; set; }

        /// <summary>
        /// The logger to use. Can leave to null.
        /// </summary>
        public ILogger Logger { get; set; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="Setup"/> instance
        /// </summary>
        public Setup() {
            Settings = new MacroSettings();
        }

        /// <summary>
        /// A default setup to use with the framewework
        /// </summary>
        public static Setup DefaultSetup() {
            Setup setup = new Setup();

            setup.CommandAssembly = Assembly.GetExecutingAssembly();
            setup.Logger = new Logger();
            setup.Settings = new MacroSettings();

            return setup;
        }
    }
}
