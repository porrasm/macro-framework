using MacroFramework.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Input {
    /// <summary>
    /// Contains information about the current keyevent
    /// </summary>
    public struct KeyEvent {
        /// <summary>
        /// The corresponding key
        /// </summary>
        public VKey Key;

        /// <summary>
        /// The key up/down state of the key
        /// </summary>
        public bool KeyState;

        /// <summary>
        /// The activation type of this keyevent
        /// </summary>
        public ActivationEventType ActivationType;
        public KeyEvent(VKey key, bool state, ActivationEventType activationType) {
            Key = key;
            KeyState = state;
            ActivationType = activationType;
        }
        public override string ToString() {
            return Key + ": " + KeyState;
        }
    }
}
