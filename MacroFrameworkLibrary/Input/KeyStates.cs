using MacroFramework.Commands;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;

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

        private static IInputEvent currentKeyEvent;

        private static int keyDownCount = 0;

        private static Queue<IInputEvent> statelessKeysPressed;

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
            statelessKeysPressed = new Queue<IInputEvent>();
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
        internal static void AddAbsoluteEvent(IInputEvent k) {
            LastKeyEventTime = Timer.Milliseconds;
            currentKeyEvent = k;
            AbsoluteKeystates[k.Key] = k.State;
        }

        /// <summary>
        /// Whether or not this is a unique keyevent
        /// </summary>
        /// <param name="k"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsUniqueEvent(KKey k, bool state) {
            return k.IsStateless() || !state ? true : !AbsoluteKeystates[k];
        }

        internal static void AddKeyEvent(IInputEvent k) {
            if (k.Key.IsStateless() && k.State) {
                statelessKeysPressed.Enqueue(k);
            }
            SafeReset();
            if (!k.Unique) {
                return;
            }

            Logger.Log("Unique event: " + k);

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
        }

        internal static void CleanStatelessKeys() {
            while (statelessKeysPressed.Count > 0) {
                IInputEvent copy = statelessKeysPressed.Dequeue().GetCopy();
                copy.State = false;
                AddKeyEvent(copy);
            }
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
        /// <param name="bind">The bind to check</param>
        public static bool IsBindActive(Bind bind) {
            if (bind.Settings.MatchType == KeyMatchType.ExactKeyMatch) {
                return IsExactKeyMatch(bind.Settings.Order, bind.Keys);
            } else {
                return IsPressingKeys(bind.Settings.Order, bind.Keys);
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

        /// <summary>
        /// Resets the key states
        /// </summary>
        public static void ResetKeyStates() {
            foreach (KKey key in Enum.GetValues(typeof(KKey))) {
                MockInput input = new MockInput(key, false, InputEventType.Keyboard);
                input.ActivationType = GetCurrentActivationEventType(false);
                AddKeyEvent(input);
            }
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
