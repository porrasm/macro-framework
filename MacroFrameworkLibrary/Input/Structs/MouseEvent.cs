using MacroFramework.Commands;
using MacroFramework.Tools;
using System;
using System.Runtime.InteropServices;

namespace MacroFramework.Input {
    public struct MouseEvent : IInputEvent {
        public ulong Index { get; }

        public KKey Key { get; set; }

        public bool State { get; set; }

        public bool Injected => flags.HasFlag(MsllFlags.Injected);

        public bool InjectedLower => flags.HasFlag(MsllFlags.InjectedLower);

        public UIntPtr ExtraInfo { get; set; }

        public long ReceiveTimestamp { get; set; }

        public InputEventType Type { get; set; }

        private MsllFlags flags;

        public bool Unique { get; set; }

        /// <summary>
        /// The screen coordinates in which the event occurred
        /// </summary>
        public POINT Point { get; set; }

        public ActivationEventType ActivationType { get; set; }

        public MouseEvent(IntPtr wParam, IntPtr lParam) {
            Index = InputEvents.NextInputEventIndex;
            ReceiveTimestamp = Timer.Milliseconds;
            MSLLHOOKSTRUCT raw = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

            ParaseMouseEvent((WindowsMessage)wParam, raw.mouseData, out KKey k, out bool s);
            Key = k;
            State = s;
            ExtraInfo = raw.dwExtraInfo;

            this.flags = raw.flags;

            Unique = KeyStates.IsUniqueEvent(k, s);
            if (k.IsStateless()) {
                ActivationType = ActivationEventType.OnPress;
            } else {
                ActivationType = KeyStates.GetCurrentActivationEventType(s);
            }

            Point = raw.pt;
            Type = InputEventType.Mouse;
        }

        /// <summary>Retrieve the Key equivalent from a <see cref="WM"/> message</summary>
        internal static void ParaseMouseEvent(WindowsMessage message, int data, out KKey key, out bool state) {
            switch (message) {
                case WindowsMessage.LBUTTONDOWN:
                    key = KKey.MouseLeft;
                    state = true;
                    return;
                case WindowsMessage.LBUTTONUP:
                    key = KKey.MouseLeft;
                    state = false;
                    return;
                case WindowsMessage.RBUTTONDOWN:
                    key = KKey.MouseRight;
                    state = true;
                    return;
                case WindowsMessage.RBUTTONUP:
                    key = KKey.MouseRight;
                    state = false;
                    return;
                case WindowsMessage.MBUTTONDOWN:
                    key = KKey.MouseMiddle;
                    state = true;
                    return;
                case WindowsMessage.MBUTTONUP:
                    key = KKey.MouseMiddle;
                    state = false;
                    return;
                case WindowsMessage.MOUSEWHEEL:
                    key = (KKey)(data >> 16 > 0 ? KKey.WheelUp : KKey.WheelDown);
                    state = true;
                    return;
                case WindowsMessage.MOUSEHWHEEL:
                    key = (KKey)(data >> 16 > 0 ? KKey.WheelRight : KKey.WheelLeft);
                    state = true;
                    return;
                case WindowsMessage.XBUTTONDOWN:
                    if (data >> 16 == 1) {
                        key = KKey.MouseXButton1;
                        state = true;
                    } else {
                        key = KKey.MouseXButton2;
                        state = true;
                    }
                    return;
                case WindowsMessage.XBUTTONUP:
                    if (data >> 16 == 1) {
                        key = KKey.MouseXButton1;
                        state = false;
                    } else {
                        key = KKey.MouseXButton2;
                        state = false;
                    }
                    return;
                case WindowsMessage.MOUSEMOVE:
                    key = KKey.MouseMove;
                    state = true;
                    return;
            }

            key = KKey.None;
            state = false;
        }

        public override string ToString() {
            return $"KeyEvent ({Key}, {State}, {ActivationType}, {Point})";
        }

        public IInputEvent GetCopy() {
            MouseEvent copy = this;
            return copy;
        }
    }
}
