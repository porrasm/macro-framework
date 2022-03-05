using MacroFramework.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MacroFramework {
    /// <summary>
    /// Inherit this class in your project to finish the setup process
    /// </summary>
    public class MacroSetup {

        #region fields
        /// <summary>
        /// The assembly in which your custom <see cref="Command"/> classes reside in. Defaults to <see cref="Assembly.GetExecutingAssembly"/>.
        /// </summary>
        public Assembly CommandAssembly { get; set; }

        /// <summary>
        /// The settings you wish to use
        /// </summary>
        public MacroSettings Settings { get; set; }

        /// <summary>
        /// The logger to use. Can leave to null.
        /// </summary>
        public ILogger Logger { get; set; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="MacroSetup"/> instance
        /// </summary>
        public MacroSetup() {
            CommandAssembly = Assembly.GetExecutingAssembly();
            Settings = new MacroSettings();
        }
    }
}
