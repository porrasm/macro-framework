using MacroFramework.Input;
using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for key event callbacks
    /// </summary>
    public class KeyActivator : CommandActivator {

        #region fields
        private Func<IInputEvent, bool> keyFilter;
        private Action<IInputEvent> cb;
        #endregion

        #region constructors
        /// <summary>
        /// Creates a <see cref="KeyActivator"/> instance
        /// </summary>
        /// <param name="key">The key for which you wish to receive callbacks on</param>
        /// <param name="callback">The key event callback</param>
        public KeyActivator(KKey key, Action<IInputEvent> callback) : base(null) {
            this.keyFilter = (e) => e.Key == key;
            this.cb = callback;
        }

        /// <summary>
        /// Creates a <see cref="KeyActivator"/> instance
        /// </summary>
        /// <param name="keyFilter">The keys for which you wish to receive callbacks on</param>
        /// <param name="callback">The key event callback</param>
        public KeyActivator(Func<IInputEvent, bool> keyFilter, Action<IInputEvent> callback) : base(null) {
            this.keyFilter = keyFilter;
            this.cb = callback;
        }

        /// <summary>
        /// Creates a <see cref="KeyActivator"/> instance
        /// </summary>
        /// <param name="key">The key for which you wish to receive callbacks on</param>
        public KeyActivator(KKey key) : base(null) {
            this.keyFilter = (e) => e.Key == key;
        }

        /// <summary>
        /// Creates a <see cref="KeyActivator"/> instance
        /// </summary>
        /// <param name="keyFilter">The keys for which you wish to receive callbacks on</param>
        public KeyActivator(Func<IInputEvent, bool> keyFilter) : base(null) {
            this.keyFilter = keyFilter;
        }
        #endregion

        /// <summary>
        /// Sets the callback for this activator
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public KeyActivator SetCallback(Action<IInputEvent> cb) {
            this.cb = cb;
            return this;
        }

        protected override bool IsActivatorActive() {
            return keyFilter == null ? true : keyFilter(InputEvents.CurrentInputEvent);
        }

        public override void Execute() {
            Callbacks.ExecuteAction(cb, InputEvents.CurrentInputEvent, "Error executing KeyCallback");
        }
    }
}