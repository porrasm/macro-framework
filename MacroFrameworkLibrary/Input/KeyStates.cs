using MacroFramework.Commands;
using MacroFramework.Input;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Input {
    /// <summary>
    /// The static class handling the key states
    /// </summary>
    public static class KeyStates {

        #region fields
        /// <summary>
        /// Returns the dictionary containig VKeytates. This dictionary should not be used for keybinds.
        /// </summary>
        internal static AutoDict<KKey, bool> AbsoluteKeystates { get; private set; }

        private static AutoDict<KKey, State> keyDown;
        private static AutoDict<KKey, State> keyUp;

        private static uint globalIndex;

        private static KeyEvent currentKeyEvent;

        private static int keyDownCount = 0;

        /// <summary>
        /// How many keys are held down at any given moment
        /// </summary>

        private struct State {
            public uint localIndex;
            public uint globalIndex;
        }

        /// <summary>
        /// The timestamp for the last keyevent
        /// </summary>
        public static long LastKeyEventTime { get; private set; }

        private static long lastSafeReset;
        #endregion

        static KeyStates() {
            Init();
        }

        internal static void Init() {
            globalIndex = 0;

            AbsoluteKeystates = new AutoDict<KKey, bool>();
            keyDown = new AutoDict<KKey, State>();
            keyUp = new AutoDict<KKey, State>();
            lastSafeReset = Timer.Milliseconds;
        }

        /// <summary>
        /// Time since last keypress in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long TimeSinceLastKeyPress() {
            return Timer.PassedFrom(LastKeyEventTime);
        }

        /// <summary>
        /// This method is used to keep track on which VKey are pressed down. Returns true if the keyevent is unique.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void AddAbsoluteEvent(KeyEvent k) {
            LastKeyEventTime = Timer.Milliseconds;
            currentKeyEvent = k;
            AbsoluteKeystates[k.Key] = k.State;
        }

        internal static bool IsUniqueEvent(KKey k, bool state) {
            return state ? !AbsoluteKeystates[k] : true;
        }

        internal static bool AddKeyEvent(KeyEvent k) {
            SafeReset();
            if (!k.Unique) {
                return false;
            }

            // Bug with keydowncount

            if (k.State) {
                State state = keyDown[k.Key];
                SetState(ref state);
                keyDown[k.Key] = state;
                keyDownCount++;
            } else {
                State state = keyUp[k.Key];
                SetState(ref state);
                keyUp[k.Key] = state;
                keyDownCount--;
            }

            return true;
        }

        /// <summary>
        /// Temporary solution for possible state mismatch due to lag/exceptions
        /// </summary>
        private static void SafeReset() {
            if (Timer.PassedFrom(lastSafeReset) < 5000) {
                return;
            }
            lastSafeReset = Timer.Milliseconds;
            keyDownCount = 0;
            foreach (KKey k in keyDown.Dictionary.Keys) {
                if (IsPressingKey(k)) {
                    keyDownCount++;
                }
            }
        }

        private static void SetState(ref State state) {
            globalIndex++;
            state.localIndex++;
            state.globalIndex = globalIndex;
        }

        #region keystate matching
        /// <summary>
        /// Returns true if the current keystate matches the given parameters
        /// </summary>
        /// <param name="matchType">The match type</param>
        /// <param name="order">Whether keyevents should be in the correct order</param>
        /// <param name="keys">The keys to match</param>
        /// <returns></returns>
        public static bool IsMatchingKeyState(KeyMatchType matchType, KeyPressOrder order, params KKey[] keys) {
            if (matchType == KeyMatchType.ExactKeyMatch) {
                return IsExactKeyMatch(order, keys);
            } else {
                return IsPressingKeys(order, keys);
            }
        }

        private static bool IsExactKeyMatch(KeyPressOrder order, params KKey[] keys) {
            if (!IsPressingKeys(order, keys)) {
                return false;
            }
            return keys.Length == keyDownCount;
        }

        private static bool IsPressingKeys(KeyPressOrder order, params KKey[] keys) {
            if (order == KeyPressOrder.Ordered) {
                return IsPressingKeysInOrder(keys);
            } else {
                return IsPressingKeysInArbitraryOrder(keys);
            }
        }

        private static bool IsPressingKeysInOrder(KKey[] keys) {
            if (keys == null || keys.Length == 0) {
                return false;
            }
            uint prevIndex = 0;
            foreach (KKey k in keys) {
                if (!IsPressingKey(k)) {
                    return false;
                }
                State state = keyDown[k];
                if (prevIndex > state.globalIndex) {
                    return false;
                }
                prevIndex = state.globalIndex;
            }

            return true;
        }

        private static bool IsPressingKeysInArbitraryOrder(KKey[] keys) {
            foreach (KKey k in keys) {
                if (!IsPressingKey(k)) {
                    return false;
                }
            }
            return true;
        }

        public static bool IsPressingKey(KKey key) {
            return keyDown[key].globalIndex > keyUp[key].globalIndex;
        }
        #endregion

        internal static void ResetKeys() {
            throw new NotImplementedException();
            //List<VKey> pressedVKey = new List<VKey>();
            //foreach (var pair in keyDown) {
            //    if (pair.Value.globalIndex > keyUp[pair.Key].globalIndex) {
            //        pressedVKey.Add(pair.Key);
            //    }
            //}

            //foreach (VKey key in pressedVKey) {
            //    WindowsInput.InputSimulator asd = new InputSimulator();
            //    asd.Keyboard.KeyUp((VirtualKeyCode)key);
            //}
        }

        /// <summary>
        /// Returns the current activation type parameter
        /// </summary>
        /// <returns></returns>
        public static ActivationEventType GetCurrentActivationEventType(bool currentKeyState) {
            if (currentKeyState) {
                return ActivationEventType.OnPress;
            } else {
                if (currentKeyEvent.State) {
                    return ActivationEventType.OnFirstRelease;
                } else {
                    return ActivationEventType.OnAnyRelease;
                }
            }
        }
    }
}
