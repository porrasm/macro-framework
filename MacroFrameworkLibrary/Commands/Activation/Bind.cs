using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// Class used to define different types of keyevent combinations
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
        /// Creates a new <see cref="Bind"/> instance and use <see cref="BindSettings.Default"/>
        /// </summary>
        public Bind(params KKey[] keys) {
            this.Keys = keys;
            this.Settings = BindSettings.Default;
        }

        #region activators
        /// <summary>
        /// Create a <see cref="BindActivator"/> from this bind
        /// </summary>
        /// <param name="cb">Callback</param>
        public BindActivator CreateBindActivator(Action cb) {
            return new BindActivator(this, cb);
        }

        /// <summary>
        /// Creates a <see cref="BindHoldActivator"/> from this bind
        /// </summary>
        public BindHoldActivator CreateBindHoldActivator(Action onActivate, Action<int> onUpdate, Action<int> onDeactivate) {
            return new BindHoldActivator(this, onActivate, onUpdate, onDeactivate);
        }
        #endregion
        #endregion
    }
}
