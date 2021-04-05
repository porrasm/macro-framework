using MacroFramework.Commands;

namespace MacroFramework {
    /// <summary>
    /// A settings class used to configure the framework
    /// </summary>
    public class MacroSettings {

        /// <summary>
        /// Whether to use a global exception handler to catch uncaught exceptions. This can catch exception withing async Tasks.
        /// </summary>
        public bool UseGlobalExceptionHandler = true;

        /// <summary>
        /// If true injected events (e.g. virtual input events sent by processes) will be handled by the application. Use with care as handling injected events can cause unwanted behaviour.
        /// </summary>
        public bool HandleInjectedInputEvents = false;

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
        public bool AllowKeyboardHook = false, AllowMouseHook = false;

        /// <summary>
        /// The delay in milliseconds between every iteration of the main loop
        /// </summary>
        public int MainLoopTimestep = 250;

        /// <summary>
        /// The <see cref="Input.KeyStates"/> can lose track of keystates if another application intercepts keys. This can cause keys to get stuck down and <see cref="Bind"/>s stop working. Use this timestep to reset the keys using <see cref="Input.KeyStates.ResetKeyStates"/> at regular intervals if no keys have been pressed for a while. Set to 0 to disable automatic keystate resets.
        /// </summary>
        public int KeyStateFixTimestep = 5000;
    }
}
