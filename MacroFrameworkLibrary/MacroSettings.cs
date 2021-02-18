using System;
using System.Collections.Generic;
using System.Text;
using MacroFramework.Commands;

namespace MacroFramework {
    /// <summary>
    /// A settings class used to configure the framework
    /// </summary>
    public class MacroSettings {

        /// <summary>
        /// Not yet implemented. Dooes not have any effect and the key listener is always active.
        /// </summary>
        internal static bool KeyListenerEnabled = true;

        /// <summary>
        /// The dedicated key on the keyboard which activates the text command mode.
        /// </summary>
        public KKey CommandKey = KKey.None;

        /// <summary>
        /// A dedicated bind key which is always intercepted and tranformed into <see cref="VKey.GENERAL_BIND_KEY"/>
        /// </summary>
        public KKey GeneralBindKey = KKey.None;

        /// <summary>
        /// The dedicated key on the keyboard which executes the current command mode
        /// </summary>
        public KKey CommandActivateKey = KKey.Enter;

        /// <summary>
        /// Not yet implemented. The dedicated key which enables the framework.
        /// </summary>
        public KKey ListenerEnableKey = KKey.None;

        /// <summary>
        /// Not yet implemented. The dedicated key which disables the <see cref="InputHook"/>
        /// </summary>
        public KKey ListenerDisableKey = KKey.None;

        /// <summary>
        /// The timeout after the last keypress after which command mode is canceled
        /// </summary>
        public int TextCommandTimeout = 2500;

        /// <summary>
        /// Whether to allow a certain device hook
        /// </summary>
        public bool AllowKeyboardHook = true, AllowMouseHook = false;

        /// <summary>
        /// The delay in milliseconds between every iteration of the main loop
        /// </summary>
        public int MainLoopTimestep = 250;
    }
}
