using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MacroFramework.Input {

    public struct POINT {
        public int X;
        public int Y;

        public POINT(int x, int y) {
            this.X = x;
            this.Y = y;
        }
    }

    [Flags]
    public enum MsllFlags : uint {
        None,
        Injected,
        InjectedLower
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MSLLHOOKSTRUCT {
        public POINT pt;
        public int mouseData;
        public MsllFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }
}
