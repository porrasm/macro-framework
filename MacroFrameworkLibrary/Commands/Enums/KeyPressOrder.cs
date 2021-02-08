using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// Determines whether keys should be pressed in the give parameter order
    /// </summary>
    public enum KeyPressOrder {
        /// <summary>
        /// The <see cref="BindActivator"/> will only activate if <see cref="BindActivator.Keys"/> are pressed in order
        /// </summary>
        Ordered,
        /// <summary>
        /// The <see cref="BindActivator"/> will activate if <see cref="BindActivator.Keys"/> are pressed in any order
        /// </summary>
        Unordered
    }
}
