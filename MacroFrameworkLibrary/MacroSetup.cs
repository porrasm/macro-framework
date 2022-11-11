using MacroFramework.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MacroFramework {
    /// <summary>
    /// Inherit this class in your project to finish the setup process
    /// </summary>
    public class MacroSetup {

        #region configuration
        /// <summary>
        /// The assembly in which your custom <see cref="Command"/> classes reside in. Defaults to <see cref="Assembly.GetExecutingAssembly"/>.
        /// </summary>
        public Assembly CommandAssembly { get; set; } = Assembly.GetExecutingAssembly();

        /// <summary>
        /// The logger to use. Can leave to null.
        /// </summary>
        public ILogger Logger { get; set; }
        #endregion

        #region settings
        /// <summary>
        /// Whether to use a global exception handler to catch uncaught exceptions. This can catch exception withing async Tasks. Default = true
        /// </summary>
        public bool UseGlobalExceptionHandler { get; set; } = true;

        /// <summary>
        /// If true injected events (e.g. virtual input events sent by processes) will be handled by the application. Use with care as handling injected events can cause unwanted behaviour. Default = false
        /// </summary>
        public bool HandleInjectedInputEvents { get; set; } = false;

        /// <summary>
        /// The dedicated key on the keyboard which activates the text command mode. Default = <see cref="KKey.None"/>
        /// </summary>
        public KKey CommandKey { get; set; } = KKey.None;

        /// <summary>
        /// A dedicated bind key which is always intercepted and tranformed into <see cref="VKey.GENERAL_BIND_KEY"/>. Default = <see cref="KKey.None"/>
        /// </summary>
        public KKey GeneralBindKey { get; set; } = KKey.None;

        /// <summary>
        /// The dedicated key on the keyboard which executes the current command mode. Default = <see cref="KKey.Enter"/>
        /// </summary>
        public KKey CommandActivateKey { get; set; } = KKey.Enter;

        /// <summary>
        /// The timeout after the last keypress after which command mode is canceled. Default = 2500ms
        /// </summary>
        public int TextCommandTimeout { get; set; } = 2500;

        /// <summary>
        /// Whether to allow the keyboard hook. Default = true
        /// </summary>
        public bool AllowKeyboardHook { get; set; } = true;

        /// <summary>
        /// Whether to allow the keyboard hook. Default = false
        /// </summary>
        public bool AllowMouseHook { get; set; } = false;

        /// <summary>
        /// The delay in milliseconds between every iteration of the main loop. Default = 10ms
        /// </summary>
        public int MainLoopTimestep { get; set; } = 10;

        /// <summary>
        /// The <see cref="Input.KeyStates"/> can lose track of keystates if another application intercepts keys. This can cause keys to get stuck down and <see cref="Bind"/>s stop working. Use this timestep to reset the keys using <see cref="Input.KeyStates.ResetKeyStates"/> at regular intervals if no keys have been pressed for a while. Set to 0 to disable automatic keystate resets. Default = 0 (disabled)
        /// </summary>
        public int KeyStateFixTimestep { get; set; } = 0;
        #endregion
    }
}
