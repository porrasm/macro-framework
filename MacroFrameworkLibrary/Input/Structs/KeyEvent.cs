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
        private KbdllFlags flags;

        /// <summary>
        /// Specifies if the key has the extended property
        /// </summary>
        public bool Extended => flags.HasFlag(KbdllFlags.Extended);
        public bool Injected => flags.HasFlag(KbdllFlags.Injected);
        public bool InjectedLower => flags.HasFlag(KbdllFlags.InjectedLower);
        public bool AltDown => flags.HasFlag(KbdllFlags.AltDown);
        public bool Release => flags.HasFlag(KbdllFlags.Release);
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
            flags = rawData.flags;

            Key = ((VKey)rawData.vkCode).AsCustom(flags.HasFlag(KbdllFlags.Extended));
            if (Key == Setup.Instance.Settings.GeneralBindKey) {
                Key = KKey.GeneralBindKey;
            }

            ActivationType = KeyStates.GetCurrentActivationEventType(State);
            Unique = KeyStates.IsUniqueEvent(Key, State);

            ExtraInfo = rawData.dwExtraInfo;
            Type = InputEventType.Keyboard;
        }

        public override string ToString() {
            return Key + ": " + State + ", " + ActivationType;
        }
    }
}
