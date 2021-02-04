using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Tools {
    public class VKeysToCommand {

        private StringBuilder b;
        public VKeysToCommand() {
            b = new StringBuilder();
        }

        public void AddKey(VKey key) {
            char c = KeyToChar(key);
            if (key == VKey.BACK) {
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

        public static char KeyToChar(VKey key) {

            if (key >= VKey.A & key <= VKey.Z) {
                return (char)((int)key);
            }

            if (key >= VKey.D0 && key <= VKey.D9) {
                int number = (int)(key - VKey.D0);
                return (char)('0' + number);
            }

            switch (key) {
                case VKey.TAB: return '\t';
                case VKey.SPACE: return ' ';
                case VKey.RETURN: return '\r';
                case VKey.OEM_MINUS: return '-';
                case VKey.OEM_PLUS: return '+';
            }

            return '\0';
        }
    }
}
