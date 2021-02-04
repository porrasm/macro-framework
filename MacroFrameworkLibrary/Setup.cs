using MacroFramework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MacroFramework {
    /// <summary>
    /// Inherit this class in your project to finish the setup process
    /// </summary>
    public abstract class Setup {

        #region fields
        public static Setup Instance { get; private set; }
        internal static void SetInstance(Setup setup) {
            Setup.Instance = setup;
        }

        public Assembly MainAssembly { get; private set; }
        public MacroSettings Settings { get; private set; }
        #endregion

        public Setup() {
            MainAssembly = GetMainAssembly();
            Settings = GetSettings();
        }

        /// <summary>
        /// Set the <see cref="MacroSettings"/> to fit your needs here
        /// </summary>
        /// <returns></returns>
        protected abstract MacroSettings GetSettings();

        /// <summary>
        /// Return your projects main <see cref="Assembly"/> (most likely using <see cref="Assembly.GetExecutingAssembly"). This allows automatically enabling your <see cref="Commands.Command"/>. Return null if you don't wish to use this feature."/>/>
        /// </summary>
        /// <returns></returns>
        protected abstract Assembly GetMainAssembly();
    }
}
