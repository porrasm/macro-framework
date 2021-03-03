using MacroFramework.Input;
using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for key event callbacks
    /// </summary>
    public class KeyActivator : CommandActivator {

        #region fields
        /// <summary>
        /// The delegate for a key event callback
        /// </summary>
        /// <param name="k"></param>
        public delegate void InputEventCallback(IInputEvent i);

        /// <summary>
        /// The delegate for a key event callback. Auto casts an <see cref="IInputEvent"/> to a <see cref="KeyEvent"/>
        /// </summary>
        /// <param name="k"></param>
        public delegate void KeyEventCallback(KeyEvent k);

        /// <summary>
        /// The delegate for a key event callback. Auto casts an <see cref="IInputEvent"/> to a <see cref="MouseEvent"/>
        /// </summary>
        /// <param name="k"></param>
        public delegate void MouseEventCallback(MouseEvent m);

        /// <summary>
        /// Filter to use with the activator
        /// </summary>
        /// <param name="e">Incoming input event</param>
        public delegate bool InputFilter(IInputEvent e);

        private InputFilter keyFilter;
        private InputEventCallback cb;
        #endregion

        #region constructors
        /// <summary>
        /// Creates a <see cref="KeyActivator"/> instance
        /// </summary>
        /// <param name="callback">The key event callback</param>
        /// <param name="key">The key for which you wish to receive callbacks on</param>
        public KeyActivator(InputEventCallback callback, KKey key) : base(null) {
            this.keyFilter = (e) => e.Key == key;
            this.cb = callback;
        }

        /// <summary>
        /// Creates a <see cref="KeyActivator"/> instance
        /// </summary>
        /// <param name="callback">The key event callback</param>
        /// <param name="keyFilter">The keys for which you wish to receive callbacks on</param>
        public KeyActivator(InputEventCallback callback, InputFilter keyFilter) : base(null) {
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
        public KeyActivator(InputFilter keyFilter) : base(null) {
            this.keyFilter = keyFilter;
        }
        #endregion

        protected override bool IsActivatorActive() {
            return keyFilter(InputEvents.CurrentInputEvent);
        }

        public override void Execute() {
            try {
                cb?.Invoke(InputEvents.CurrentInputEvent);
            } catch (Exception e) {
                Logger.Log("Error executing KeyCallback: " + e.Message);
            }
        }
    }
}