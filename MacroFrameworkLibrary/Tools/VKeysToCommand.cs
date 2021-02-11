using MacroFramework.Input;
using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Tools {
    internal class VKeysToCommand {

        private StringBuilder b;
        public VKeysToCommand() {
            b = new StringBuilder();
        }

        public void AddKey(KKey key) {
            char c = KeyToChar(key);
            if (key == KKey.Backspace) {
                if (b.Length > 0) {
                    b.Remove(b.Length - 1, 1);
                }
            } else if (c != '\0') {
                b.Append(c);
            }
        }

        public void Clear() {
            b.Clear();
        }

        public override string ToString() {
            return b.ToString().ToLower();
        }

        public bool Matches(string other) {
            return other.Equals(ToString());
        }

        public static char KeyToChar(KKey key) {

            if (key >= KKey.A & key <= KKey.Z) {
                return (char)((int)key);
            }

            if (key >= KKey.D0 && key <= KKey.D9) {
                int number = (int)(key - KKey.D0);
                return (char)('0' + number);
            }

            switch (key) {
                case KKey.Tab: return '\t';
                case KKey.Space: return ' ';
                case KKey.Enter: return '\r';
                case KKey.Minus: return '-';
                case KKey.Plus: return '+';
            }

            return '\0';
        }
    }
}
