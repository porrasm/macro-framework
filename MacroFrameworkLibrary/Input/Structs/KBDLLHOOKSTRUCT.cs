using System;
using System.Runtime.InteropServices;

namespace MacroFramework.Input {
    [Flags]
    public enum KbdllFlags : uint {
        Extended = 0x01,
        Injected = 0x10,
        InjectedLower = 0x02,
        AltDown = 0x20,
        Release = 0x80,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT {
        public uint vkCode;
        public uint scanCode;
        public KbdllFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }
}
