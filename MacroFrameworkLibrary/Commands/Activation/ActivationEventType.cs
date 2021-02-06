using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// Filters activations based on whether the current event was a keydown or a keyup event
    /// </summary>
    public enum ActivationEventType {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        OnFirstRelease,
        OnAnyRelease,
        OnPress,
        Any
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
