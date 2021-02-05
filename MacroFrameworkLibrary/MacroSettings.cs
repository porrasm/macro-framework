using System;
using System.Collections.Generic;
using System.Text;
using MacroFramework.Input;

namespace MacroFramework {
    public class MacroSettings {

        public static bool KeyListenerEnabled = true;

        /// <summary>
        /// The dedicated key on the keyboard which activates the text command mode.
        /// </summary>
        public VKey CommandKey = VKey.NONE;

        /// <summary>
        /// A dedicated bind key which is always intercepted and tranformed into <see cref="VKey.GENERAL_BIND_KEY"/>
        /// </summary>
        public VKey GeneralBindKey = VKey.NONE;

        /// <summary>
        /// The dedicated key on the keyboard which executes the current command mode
        /// </summary>
        public VKey CommandActivateKey = VKey.RETURN;

        /// <summary>
        /// The dedicated key which enabled the <see cref="InputHook"/>
        /// </summary>
        public VKey ListenerEnableKey = VKey.NONE;

        /// <summary>
        /// The dedicated key which disables the <see cref="InputHook"/>
        /// </summary>
        public VKey ListenerDisableKey = VKey.NONE;

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
        public int MainLoopTimestep = 1000;
    }
}
