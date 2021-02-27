using System;
using System.Runtime.InteropServices;

namespace MacroFramework.Input {

    public struct POINT {
        public int X;
        public int Y;

        public POINT(int x, int y) {
            this.X = x;
            this.Y = y;
        }

        public override string ToString() {
            return $"({X}, {Y})";
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
