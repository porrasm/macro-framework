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
        OnRelease,
        OnPress,
        Any
    }
}
