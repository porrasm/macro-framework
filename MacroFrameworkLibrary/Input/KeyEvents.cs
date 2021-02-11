using MacroFramework.Commands;
using MacroFramework.Input;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Input {
    // Soon to be rewritten, code is smelly
    /// <summary>
    /// The static class handling all input key events.
    /// </summary>
    public static class KeyEvents {

        #region fields
        /// <summary>
        /// Returns the current KeyEvent. This can be used by Command classes to access every KeyEvent.
        /// </summary>
        public static KeyEvent CurrentKeyEvent { get; private set; }

        internal static bool CommandMode { get; private set; }

        internal static bool GeneralKeyBind { get; private set; }

        private static Queue<KeyEvent> keyEventQueue;

        private static HashSet<KKey> blockKeys;

        public delegate bool KeyCallbackFunc(KeyEvent k);



        /// <summary>
        /// This delegate is invoked at every keypress, before it is registered by the <see cref="KeyStates"/>. Return true to intercept key from other applications and the <see cref="MacroFramework"/> itself. This delegate is blocking and slow execution will cause OS wide latency for key events.
        /// </summary>
        public static KeyCallbackFunc KeyCallback { get; set; }
        #endregion

        static KeyEvents() {
            keyEventQueue = new Queue<KeyEvent>();
            blockKeys = new HashSet<KKey>();
        }

        /// <summary>
        /// Set the block status for a given key. If true is set, the key is blocked and other applications won't get the keyevent. The blocking of the key is not absolutely certain.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="status"></param>
        internal static void SetKeyBlockStatus(KKey key, bool status) {
            if (status) {
                blockKeys.Add(key);
            } else {
                blockKeys.Remove(key);
            }
        }

        #region hook
        /// <summary>
        /// Handles the key press event. When true is returned the key event is intercepted.
        /// </summary>
        internal static bool OnHookKeyEvent(KeyEvent k) {
            KeyStates.AddAbsoluteEvent(k);

            if (KeyCallback?.Invoke(k) ?? false) {
                return true;
            }

            HandleQueuedKeyEventsNonBlocking();

            long timeSincePreviousEvent = KeyStates.TimeSinceLastKeyPress();

            #region check enabled
            //if (k.Key == Setup.Instance.Settings.ListenerEnableKey) {
            //    MacroSettings.KeyListenerEnabled = true;
            //    KeyState.ResetKeys();
            //    return true;
            //} else if (k.Key == Setup.Instance.Settings.ListenerDisableKey) {
            //    MacroSettings.KeyListenerEnabled = false;
            //    KeyState.ResetKeys();
            //    return true;
            //}
            //if (!MacroSettings.KeyListenerEnabled) {
            //    return false;
            //}
            #endregion

            keyEventQueue.Enqueue(k);

            #region blocking keys
            if (k.Key == KKey.GeneralBindKey && Setup.Instance.Settings.InterceptGeneralBindKey) {
                GeneralKeyBind = k.State;
                return true;
            }
            if (k.Key == Setup.Instance.Settings.CommandKey && !CommandMode) {
                if (k.State) {
                    CommandKeyPress(true, true);
                }
                return true;
            }
            #endregion

            #region special modes
            // General bind mode
            if (GeneralKeyBind) {
                return true;
            }

            // Text command mode
            if (CommandMode) {
                if (timeSincePreviousEvent >= Setup.Instance.Settings.TextCommandTimeout) {
                    Console.WriteLine("End timeout: " + timeSincePreviousEvent);
                    CommandKeyPress(false, false);
                    return false;
                }
                return true;
            }
            #endregion

            return blockKeys.Contains(k.Key);
        }
        #endregion

        #region keyevent
        internal static async void HandleQueuedKeyEventsNonBlocking() {
            await Task.Delay(1);
            HandleQueuedKeyEvents();
        }
        internal static void HandleQueuedKeyEvents() {
            while (keyEventQueue.Count > 0) {
                OnKeyEvent(keyEventQueue.Dequeue());
            }
        }

        private static void OnKeyEvent(KeyEvent k) {

            if (CommandMode) {
                KeyStates.AddKeyEvent(k);
                OnCommandMode(k);
                CommandContainer.UpdateActivators(typeof(KeyActivator), typeof(BindActivator));
                return;
            }

            if (k.State) {
                KeyStates.AddKeyEvent(k);
            }

            CurrentKeyEvent = k;

            if (k.Unique) {
                CommandContainer.UpdateActivators(typeof(KeyActivator), typeof(BindActivator));
            }

            if (!k.State) {
                KeyStates.AddKeyEvent(k);
            }
        }

        private static void OnCommandMode(KeyEvent k) {

            if (k.Key == Setup.Instance.Settings.CommandKey) {
                return;
            }

            if (!k.State) {
                return;
            }

            if (k.Key == Setup.Instance.Settings.CommandActivateKey) {
                CommandKeyPress(false, true);
                return;
            }

            if (VKeysToCommand.KeyToChar(k.Key) == '\0' && k.Unique) {
                Console.WriteLine("End wrong key");
                CommandKeyPress(false, false);
                return;
            }

            TextCommandCreator.CommandKeyEvent(k);
        }
        private static void CommandKeyPress(bool state, bool acceptCommand) {
            if (state && !CommandMode) {
                Console.WriteLine("\n Command mode start");
                TextCommandCreator.StartCommand();
            } else if (!state && CommandMode) {
                Console.WriteLine("\n Command mode end");
                TextCommandCreator.EndCommand(acceptCommand);
            }
            CommandMode = state;
        }
        #endregion
    }
}
