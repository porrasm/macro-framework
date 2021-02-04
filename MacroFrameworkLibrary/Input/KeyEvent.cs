using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Input {
    public struct KeyEvent {
        public VKey Key;
        public bool KeyState;
        public KeyEvent(VKey key, bool state) {
            Key = key;
            KeyState = state;
        }
        public override string ToString() {
            return Key + ": " + KeyState;
        }
    }
}
