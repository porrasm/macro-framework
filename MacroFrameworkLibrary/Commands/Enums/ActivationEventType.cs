using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// Filters activations based on whether the current event was a keydown or a keyup event
    /// </summary>
    /// <remarks>You should be fine with <see cref="OnFirstRelease"/> most of the time.</remarks>
    public enum ActivationEventType {
        /// <summary>
        /// Only activates a keybind on the first key release after a keypress. E.g. if you press [A, B, C] and release the keys in order, only the binds with [A, B, C] will be activated.
        /// </summary>
        OnFirstRelease,
        /// <summary>
        /// Activates a keybind on any key up event. E.g. if you press [A, B, C] and release the keys in order, the binds for [A, B, C], [B, C] and [C] will be activated.
        /// </summary>
        OnAnyRelease,
        /// <summary>
        /// Activates a keybind on any key down event. E.g. if you press and hold [A, B, C] in order, the binds with [A], [A, B] and [A, B, C] will be activated.
        /// </summary>
        OnPress,
        /// <summary>
        /// ACtivates a keybind on any key event. Use with caution as binds with this option are activated twice, once on key down and once again on key up events. Use <see cref="Input.InputEvents.CurrentInputEvent"/> to determine whether the current event is a key down or up event.
        /// </summary>
        Any
    }
}
