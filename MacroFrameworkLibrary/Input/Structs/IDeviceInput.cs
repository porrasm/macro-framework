using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Input {
    /// <summary>
    /// Interface for representing device input
    /// </summary>
    public interface IDeviceInput {
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
        /// Specifies if the event is a mouse key
        /// </summary>
        bool IsMouse { get; }

        /// <summary>
        /// The time in milliseconds when it was received. See <see cref="MacroFramework.Tools.Timer"/>.
        /// </summary>
        long ReceiveTimestamp { get; }
    }
}
