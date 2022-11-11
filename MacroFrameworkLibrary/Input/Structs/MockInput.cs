using MacroFramework.Commands;
using MacroFramework.Tools;
using System;

namespace MacroFramework.Input {
    internal struct MockInput : IInputEvent {
        public ulong Index { get; }

        public KKey Key { get; set; }

        public bool State { get; set; }

        public bool Injected { get; set; }

        public bool InjectedLower { get; set; }

        public UIntPtr ExtraInfo { get; set; }

        public long ReceiveTimestamp { get; set; }

        public InputEventType Type { get; set; }

        public ActivationEventType ActivationType { get; set; }

        public bool Unique { get; set; }

        public MockInput(KKey key, bool state, InputEventType type) : this() {
            Index = InputEvents.NextInputEventIndex;
            Key = key;
            State = state;
            ReceiveTimestamp = Timer.Milliseconds;
            Type = type;
        }


        public IInputEvent GetCopy() {
            MockInput copy = this;
            return copy;
        }
    }
}
