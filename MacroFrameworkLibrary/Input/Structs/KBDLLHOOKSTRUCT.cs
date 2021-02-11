using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MacroFramework.Input {
    [Flags]
    internal enum KbdllFlags : uint {
        Extended = 0x01,
        Injected = 0x10,
        InjectedLower = 0x02,
        AltDown = 0x20,
        Release = 0x80,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct KBDLLHOOKSTRUCT {
        public uint vkCode;
        public uint scanCode;
        public KbdllFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }
}
