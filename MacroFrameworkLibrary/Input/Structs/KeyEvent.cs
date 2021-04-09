using MacroFramework.Commands;
using MacroFramework.Tools;
using System;

namespace MacroFramework.Input {
    /// <summary>
    /// Contains information about the current keyevent
    /// </summary>
    public struct KeyEvent : IInputEvent {
        /// <summary>
        /// The corresponding key
        /// </summary>
        public KKey Key { get; set; }

        /// <summary>
        /// The key up/down state of the key
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// True in regular press and release situations. False when holding down a key.
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// The activation type of this keyevent
        /// </summary>
        public ActivationEventType ActivationType { get; set; }

        #region fields
        public KbdllFlags Flags { get; set; }

        /// <summary>
        /// Specifies if the key has the extended property
        /// </summary>
        public bool Extended => Flags.HasFlag(KbdllFlags.Extended);
        public bool Injected => Flags.HasFlag(KbdllFlags.Injected);
        public bool InjectedLower => Flags.HasFlag(KbdllFlags.InjectedLower);
        public bool AltDown => Flags.HasFlag(KbdllFlags.AltDown);
        public bool Release => Flags.HasFlag(KbdllFlags.Release);
        #endregion

        public UIntPtr ExtraInfo { get; set; }

        public InputEventType Type { get; set; }

        public long ReceiveTimestamp { get; set; }

        /// <summary>
        /// Creates a new <see cref="KeyEvent"/> instance from a low level input event
        /// </summary>
        internal KeyEvent(IntPtr wParam, KBDLLHOOKSTRUCT rawData) {

            ReceiveTimestamp = Timer.Milliseconds;

            WindowsMessage msg = (WindowsMessage)wParam;
            State = msg == WindowsMessage.KEYDOWN || msg == WindowsMessage.SYSKEYDOWN;
            Flags = rawData.flags;

            Key = ((VKey)rawData.vkCode).AsCustom(Flags.HasFlag(KbdllFlags.Extended));
            if (Key == Macros.Setup.Settings.GeneralBindKey) {
                Key = KKey.GeneralBindKey;
            }

            ActivationType = KeyStates.GetCurrentActivationEventType(State);
            Unique = KeyStates.IsUniqueEvent(Key, State);

            ExtraInfo = rawData.dwExtraInfo;
            Type = InputEventType.Keyboard;
        }

        public override string ToString() {
            return $"KeyEvent ({Key}, {State}, {ActivationType})";
        }

        public IInputEvent GetCopy() {
            KeyEvent copy = this;
            return copy;
        }
    }
}
