using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework {
    public static class Delegates {
        public delegate void Void();
        public delegate void VoidStringParam(string s);
        public delegate void VoidKeyeventParams(KeyEvent k);
    }
}
