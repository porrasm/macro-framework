using MacroFramework.Commands;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Input {
    internal struct MockInput : IInputEvent {
        public MockInput(KKey key, bool state, InputEventType type) : this() {
            Key = key;
            State = state;
            ReceiveTimestamp = Timer.Milliseconds;
            Type = type;
        }

        public KKey Key { get; set; }

        public bool State { get; set; }

        public bool Injected { get; set; }

        public bool InjectedLower { get; set; }

        public UIntPtr ExtraInfo { get; set; }

        public long ReceiveTimestamp { get; set; }

        public InputEventType Type { get; set; }

        public ActivationEventType ActivationType { get; set; }

        public bool Unique { get; set; }

        public IInputEvent GetCopy() {
            MockInput copy = this;
            return copy;
        }
    }
}
