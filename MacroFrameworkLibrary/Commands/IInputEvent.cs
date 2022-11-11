using MacroFramework.Input;
using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// Interface for representing device input
    /// </summary>
    public interface IInputEvent {
        /// <summary>
        /// The unique index of this input event which can be generated using <see cref="InputEvents.NextInputEventIndex"/>
        /// </summary>
        ulong Index { get; }

        /// <summary>
        /// The corresponding key
        /// </summary>
        KKey Key { get; set; }

        /// <summary>
        /// The state of the key
        /// </summary>
        bool State { get; set; }

        /// <summary>
        /// Specifies whether the event was emitted by a process
        /// </summary>
        bool Injected { get; }

        /// <summary>
        /// Specifies if the key was emitted by a lower integrity level process
        /// </summary>
        bool InjectedLower { get; }

        /// <summary>
        /// Extra information given by the event source
        /// </summary>
        UIntPtr ExtraInfo { get; set; }

        /// <summary>
        /// The time in milliseconds when it was received. See <see cref="MacroFramework.Tools.Timer"/>.
        /// </summary>
        long ReceiveTimestamp { get; set; }

        /// <summary>
        /// The type of the input event
        /// </summary>
        InputEventType Type { get; set; }

        /// <summary>
        /// The type of the activation event
        /// </summary>
        ActivationEventType ActivationType { get; set; }

        /// <summary>
        /// Determines if the event is unique. False in the case of a key being held down.
        /// </summary>
        bool Unique { get; set; }

        /// <summary>
        /// Returns a new copy of the event
        /// </summary>
        IInputEvent GetCopy();
    }
}
