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

        private KKey key;
        private InputEventCallback cb;
        #endregion

        #region constructors
        /// <summary>
        /// Creates a <see cref="KeyActivator"/> instance
        /// </summary>
        /// <param name="callback">The key event callback</param>
        /// <param name="key">The key for which you wish to receive callbacks on</param>
        public KeyActivator(InputEventCallback callback, KKey key) : base(null) {
            this.key = key;
            this.cb = callback;
        }

        private KeyActivator(KeyEventCallback callback, KKey key) : base(null) {
            this.key = key;
            this.cb = (i) => callback((KeyEvent)i);
        }

        private KeyActivator(MouseEventCallback callback, KKey key) : base(null) {
            this.key = key;
            this.cb = (i) => callback((MouseEvent)i);
        }

        /// <summary>
        /// Creates a <see cref="KeyActivator"/> instance
        /// </summary>
        /// <param name="callback">The key event callback</param>
        /// <param name="key">The key for which you wish to receive callbacks on</param>
        public static KeyActivator AutoCastToKeyEvent(KeyEventCallback c, KKey k) {
            return new KeyActivator(c, k);
        }

        /// <summary>
        /// Creates a <see cref="KeyActivator"/> instance
        /// </summary>
        /// <param name="callback">The key event callback</param>
        /// <param name="key">The key for which you wish to receive callbacks on</param>
        public static KeyActivator AutoCastToMouseEvent(MouseEventCallback c, KKey k) {
            return new KeyActivator(c, k);
        }
        #endregion

        protected override bool IsActivatorActive() {
            return InputEvents.CurrentInputEvent.Key == key;
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