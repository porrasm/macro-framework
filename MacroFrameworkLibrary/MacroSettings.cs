using System;
using System.Collections.Generic;
using System.Text;
using MacroFramework.Input;
using MacroFramework.Input;

namespace MacroFramework {
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
        /// Not yet implemented. The dedicated key which enabled the <see cref="InputHook"/>
        /// </summary>
        public KKey ListenerEnableKey = KKey.None;

        /// <summary>
        /// Not yet implemented. The dedicated key which disables the <see cref="InputHook"/>
        /// </summary>
        public KKey ListenerDisableKey = KKey.None;

        /// <summary>
        /// If true the <see cref="MacroSettings.GeneralBindKey"/> is intercepted such that other applications do not register it (not abosolutely certain)
        /// </summary>
        public bool InterceptGeneralBindKey = true;

        /// <summary>
        /// The timeout after the last keypress after which command mode is canceled
        /// </summary>
        public int TextCommandTimeout = 2500;

        /// <summary>
        /// The delay in milliseconds between every iteration of the main loop
        /// </summary>
        public int MainLoopTimestep = 250;
    }
}
