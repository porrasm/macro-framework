using MacroFramework.Commands;
using MacroFramework.Commands.Coroutines;
using System;
using System.Collections.Generic;

namespace MacroFramework.Input {
    /// <summary>
    /// Static clas used to handle the keyevents captured by the <see cref="Input.InputHook"/>
    /// </summary>
    public static class InputEvents {

        #region fields
        /// <summary>
        /// Returns the current maximum index of any input event
        /// </summary>
        public static ulong MaximumInputEventIndex { get; private set; }

        /// <summary>
        /// Retrieves the next unique input event index
        /// </summary>
        public static ulong NextInputEventIndex { get => ++MaximumInputEventIndex; }

        /// <summary>
        /// This delegate is invoked at every keypress, before it is registered by the <see cref="KeyStates"/>. Return true to intercept key from other applications and the <see cref="MacroFramework"/> itself. This delegate is blocking and slow execution will cause OS wide latency for key events.
        /// </summary>
        public static Func<IInputEvent, KeyBlockType> InputCallback { get; set; }

        public enum KeyBlockType {
            None,
            BlockFromOS,
            BlockFromApp,
            BlockFromBoth
        }

        /// <summary>
        /// Special event for <see cref="KKey.MouseMove"/>.
        /// </summary>
        public static Action<MouseEvent> OnMouseMove { get; set; }

        /// <summary>
        /// Use this block keys from other applications
        /// </summary>
        public static HashSet<KKey> BlockedKeys { get; set; }

        private static Queue<IInputEvent> keyEventQueue;

        /// <summary>
        /// Returns the current KeyEvent. This can be used by Command classes to access the current KeyEvent.
        /// </summary>
        public static IInputEvent CurrentInputEvent { get; private set; }

        /// <summary>
        /// Executed right before an input event is handled in the framework. If true is returned this keybind will not be handled by any activators
        /// </summary>
        public static Func<IInputEvent, bool> OnBeforeInputEvent { get; set; }
        #endregion

        internal static void Initialize() {
            keyEventQueue = new Queue<IInputEvent>();
            BlockedKeys = new HashSet<KKey>();
        }

        #region hook inputevent
        /// <summary>
        /// You can use this method to send virtual input within the application.
        /// </summary>
        /// <param name="k"></param>
        /// <returns>True if key should be intercepted</returns>
        public static bool RegisterHookKeyEvent(IInputEvent k) {
            bool releaseForDownKey = !k.State && KeyStates.AbsoluteKeystates[k.Key];
            KeyStates.AddAbsoluteEvent(k);

            KeyBlockType block = Callbacks.ExecuteFunc(InputCallback, k, KeyBlockType.None, "Error in InputEventCallback", () => Console.WriteLine("optional fail callback"));

            if (block == KeyBlockType.BlockFromBoth) {
                return true;
            }
            if (block == KeyBlockType.BlockFromApp || k.Injected) {
                return false;
            }

            bool blockOS = block == KeyBlockType.BlockFromOS;

            if (k.Key == KKey.MouseMove) {
                Callbacks.ExecuteAction(OnMouseMove, (MouseEvent)k, "Error on OnMouseMove event");
                return blockOS;
            }

            keyEventQueue.Enqueue(k);

            if (IsBlockedKey(k, releaseForDownKey) || (TextCommandCreator.IsCommandMode && !releaseForDownKey)) {
                return true;
            }

            return blockOS || BlockedKeys.Contains(k.Key);
        }

        private static bool IsBlockedKey(IInputEvent k, bool releaseForDownKey) {
            if (KeyStates.AbsoluteKeystates[KKey.GeneralBindKey] && !releaseForDownKey) {
                return true;
            }

            if (k.Key == Macros.Setup.CommandKey && !TextCommandCreator.IsCommandMode) {
                if (k.State) {
                    TextCommandCreator.CommandKeyPress(true, true);
                }
                return true;
            }

            return false;
        }
        #endregion

        #region handle input events
        internal static void HandleQueuedKeyevents() {
            KeyStates.CleanStatelessKeys();
            while (keyEventQueue.Count > 0) {
                IInputEvent k = keyEventQueue.Dequeue();
                HandleKeyEvent(k);
            }
        }

        private static void HandleKeyEvent(IInputEvent k) {
            CurrentInputEvent = k;
            bool releaseForDownKey = !k.State && KeyStates.IsPressingKey(k.Key);

            if (CheckCommandMode() && !releaseForDownKey) {
                return;
            }

            if (Callbacks.ExecuteFunc(OnBeforeInputEvent, CurrentInputEvent, false, "OnBeforeInputEvent")) {
                return;
            }

            if (k.State) {
                KeyStates.AddKeyEvent(k);
            }
            if (k.Unique) {
                CommandContainer.UpdateActivators(typeof(KeyActivator), typeof(BindActivator));
                CommandContainer.ForEveryCommand(c => c.Coroutines.UpdateCoroutines(CoroutineUpdateGroup.OnInputEvent), false, $"Coroutine {CoroutineUpdateGroup.OnInputEvent}");
            }
            if (!k.State) {
                KeyStates.AddKeyEvent(k);
            }
        }

        private static bool CheckCommandMode() {

            // Check command mode
            if (TextCommandCreator.IsCommandMode) {
                KeyStates.AddKeyEvent(CurrentInputEvent);
                TextCommandCreator.OnCommandMode(CurrentInputEvent);
                CommandContainer.UpdateActivators(typeof(KeyActivator), typeof(BindActivator));
                return true;
            }

            return false;
        }

        internal static void ClearEvents() {
            keyEventQueue.Clear();
        }
        #endregion
    }
}
