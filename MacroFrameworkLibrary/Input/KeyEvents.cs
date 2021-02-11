using MacroFramework.Commands;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Input {
    public static class KeyEvents {

        #region fields
        /// <summary>
        /// A delegate for receiving keyevents from the framework
        /// </summary>
        public delegate bool KeyCallbackFunc(KeyEvent k);

        /// <summary>
        /// This delegate is invoked at every keypress, before it is registered by the <see cref="KeyStates"/>. Return true to intercept key from other applications and the <see cref="MacroFramework"/> itself. This delegate is blocking and slow execution will cause OS wide latency for key events.
        /// </summary>
        public static KeyCallbackFunc KeyCallback { get; set; }

        /// <summary>
        /// Use this block keys from other applications
        /// </summary>
        public static HashSet<KKey> BlockedKeys { get; set; }

        private static Queue<KeyEvent> keyEventQueue;

        /// <summary>
        /// Returns the current KeyEvent. This can be used by Command classes to access every KeyEvent.
        /// </summary>
        public static KeyEvent CurrentKeyEvent { get; private set; }
        #endregion

        public static void Initialize() {
            keyEventQueue = new Queue<KeyEvent>();
            BlockedKeys = new HashSet<KKey>();
        }

        #region hook key event
        /// <summary>
        /// You can use this method to send virtual input within the application.
        /// </summary>
        /// <param name="k"></param>
        /// <returns>True if key should be intercepted</returns>
        public static bool RegisterHookKeyEvent(KeyEvent k) {
            KeyStates.AddAbsoluteEvent(k);

            if (Macros.Paused) {
                Console.WriteLine("Paused");
                return false;
            }

            if (KeyCallback?.Invoke(k) ?? false) {
                Console.WriteLine("Skip queue");
                return true;
            }

            keyEventQueue.Enqueue(k);

            if (IsBlockedKey(k)) {
                return true;
            }

            return BlockedKeys.Contains(k.Key);
        }

        private static bool IsBlockedKey(KeyEvent k) {
            if (KeyStates.AbsoluteKeystates[KKey.GeneralBindKey]) {
                return true;
            }

            if (k.Key == Setup.Instance.Settings.CommandKey && !TextCommandCreator.IsCommandMode) {
                if (k.State) {
                    TextCommandCreator.CommandKeyPress(true, true);
                }
                return true;
            }

            return false;
        }
        #endregion

        #region handle keyevents
        public static void HandleQueuedKeyevents() {
            while (keyEventQueue.Count > 0) {
                KeyEvent k = keyEventQueue.Dequeue();
                HandleKeyEvent(k);
            }
        }

        private static void HandleKeyEvent(KeyEvent k) {
            Console.WriteLine("KeyEvent: " + k);
            CurrentKeyEvent = k;

            if (CheckCommandMode()) {
                return;
            }

            if (k.State) {
                KeyStates.AddKeyEvent(k);
            }
            if (k.Unique) {
                CommandContainer.UpdateActivators(typeof(KeyActivator), typeof(BindActivator));
            }
            if (!k.State) {
                KeyStates.AddKeyEvent(k);
            }
        }

        private static bool CheckCommandMode() {

            // Check command mode
            if (TextCommandCreator.IsCommandMode) {
                KeyStates.AddKeyEvent(CurrentKeyEvent);
                TextCommandCreator.OnCommandMode(CurrentKeyEvent);
                CommandContainer.UpdateActivators(typeof(KeyActivator), typeof(BindActivator));
                return true;
            }

            return false;
        }
        #endregion
    }
}
