using System;
using System.Collections.Generic;
using System.Text;
using MacroFramework.Input;

namespace MacroFramework {
    public static class MacroSettings {

        public static bool KeyListenerEnabled = true;

        /// <summary>
        /// The dedicated key on a the keyboard which activates the text command mode.
        /// </summary>
        public const VKey CommandKey = VKey.F13;
        public const VKey GeneralBindKey = VKey.F14;

        public const VKey MouseFrontKey = VKey.HOME;
        public const VKey MouseBackKey = VKey.END;
        public const VKey DPIKey = VKey.F15;

        public const VKey CommandCancelKey = VKey.ESCAPE;
        public const VKey CommandActivateKey = VKey.RETURN;

        public const VKey ListenerEnableKey = VKey.PAGEUP;
        public const VKey ListenerDisableKey = VKey.PAGEDOWN;

        public static bool InterceptGeneralBindKey = true;

        #region time settings
        public static int MainUpdateTime = 250;
        public static int TextCommandUpdate = 250;

        public static int TextCommandTimeout = 2500;

        public const int PORT = 8000;
        #endregion

        public static string BATCH_DIRECTORY = "P:/Stuff/BatchScripts";
    }
}
