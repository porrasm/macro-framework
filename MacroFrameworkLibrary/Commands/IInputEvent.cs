using MacroFramework.Commands;
using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// Interface for representing device input
    /// </summary>
    public interface IInputEvent {
        /// <summary>
        /// The corresponding key
        /// </summary>
        KKey Key { get; }

        /// <summary>
        /// The state of the key
        /// </summary>
        bool State { get; }

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
        UIntPtr ExtraInfo { get; }

        /// <summary>
        /// The time in milliseconds when it was received. See <see cref="MacroFramework.Tools.Timer"/>.
        /// </summary>
        long ReceiveTimestamp { get; }

        /// <summary>
        /// The type of the input event
        /// </summary>
        InputEventType Type { get; }

        /// <summary>
        /// The type of the activation event
        /// </summary>
        ActivationEventType ActivationType { get; }

        /// <summary>
        /// Determines if the event is unique. False in the case of a key being held down.
        /// </summary>
        bool Unique { get; }
    }
}
