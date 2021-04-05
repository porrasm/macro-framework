using MacroFramework.Input;
using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for keybind callbacks
    /// </summary>
    public class BindActivator : CommandActivator {

        #region fields
        /// <summary>
        /// The bind to use
        /// </summary>
        public Bind Bind { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new <see cref="BindActivator"/> instance
        /// </summary>
        /// <param name="bind">The bind to use</param>
        public BindActivator(Bind bind) : base(null) {
            if (bind == null) {
                throw new Exception("Bind cannot be null");
            }
            this.Bind = bind;
        }

        /// <summary>
        /// Creates a new <see cref="BindActivator"/> instance
        /// </summary>
        /// <param name="bind">The bind to use</param>
        /// <param name="callback">The callback to use</param>
        public BindActivator(Bind bind, Action callback) : base(callback) {
            if (bind == null) {
                throw new Exception("Bind cannot be null");
            }
            this.Bind = bind;
        }
        #endregion

        /// <summary>
        /// Sets the callback for this activator
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public BindActivator SetCallback(Action cb) {
            CommandCallback = cb;
            return this;
        }

        protected override bool IsActivatorActive() {
            IInputEvent k = InputEvents.CurrentInputEvent;
            if (IsMatchingActivationEventType(k.ActivationType)) {
                return KeyStates.IsBindActive(Bind);
            }
            return false;
        }

        private bool IsMatchingActivationEventType(ActivationEventType type) {
            if (Bind.Settings.ActivationType == ActivationEventType.Any) {
                return true;
            }
            if (Bind.Settings.ActivationType == ActivationEventType.OnAnyRelease && (type == ActivationEventType.OnFirstRelease || type == ActivationEventType.OnAnyRelease)) {
                return true;
            }
            return Bind.Settings.ActivationType == type;
        }
    }
}