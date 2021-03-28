using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// Class used to different types define keyevent combinations
    /// </summary>
    public class Bind {
        #region fields
        /// <summary>
        /// The keys which need to be pressed
        /// </summary>
        public KKey[] Keys { get; set; }

        /// <summary>
        /// The settings to use for this bind
        /// </summary>
        public BindSettings Settings { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new <see cref="Bind"/> instance
        /// </summary>
        public Bind(BindSettings settings, params KKey[] keys) {
            this.Settings = settings;
            this.Keys = keys;
        }

        /// <summary>
        /// Creates a new <see cref="Bind"/> instance
        /// </summary>
        public Bind(params KKey[] keys) {
            this.Settings = BindSettings.Default;
            this.Keys = keys;
        }
        #endregion
    }
}
