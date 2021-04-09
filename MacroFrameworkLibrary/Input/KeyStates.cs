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

        /// <summary>
        /// Number of keys held down
        /// </summary>
        public static int KeyDownCount { get; private set; }

        private static Queue<IInputEvent> statelessKeysPressed;

        /// <summary>
        /// How many keys are held down at any given moment
        /// </summary>

        private struct State {
            public long time;
            public uint globalIndex;
        }

        /// <summary>
        /// The timestamp for the last keyevent
        /// </summary>
        public static long LastKeyEventTime { get; private set; }

        internal static long LastKeyResetTime {get; private set; }
        #endregion

        static KeyStates() {
            Init();
        }

        internal static void Init() {
            globalIndex = 0;

            AbsoluteKeystates = new AutoDict<KKey, bool>();
            keyDown = new AutoDict<KKey, State>();
            keyUp = new AutoDict<KKey, State>();
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
            return k.IsStateless() || state != AbsoluteKeystates[k];
        }

        internal static void AddKeyEvent(IInputEvent k) {
            if (k.Key.IsStateless() && k.State) {
                statelessKeysPressed.Enqueue(k);
            }

            if (!k.Unique) {
                return;
            }

            Logger.Log("Unique event: " + k);

            if (k.State) {
                if (!IsPressingKey(k.Key)) {
                    KeyDownCount++;
                }
                State state = keyDown[k.Key];
                InitState(ref state);
                keyDown[k.Key] = state;
            } else {
                if (IsPressingKey(k.Key)) {
                    KeyDownCount--;
                }
                State state = keyUp[k.Key];
                InitState(ref state);
                keyUp[k.Key] = state;
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
        

        private static void InitState(ref State state) {
            globalIndex++;
            state.globalIndex = globalIndex;
            state.time = Timer.Milliseconds;
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
            return keys.Length == KeyDownCount;
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

        public static long GetKeyHoldTime(KKey key) {
            return IsPressingKey(key) ? Timer.PassedFrom(keyDown[key].time) : -1;
        }
        #endregion

        /// <summary>
        /// Resets the key states
        /// </summary>
        /// <param name="forced">If false, only resets keys that have been held down long enough. If true resets all keys which are held down.</param>
        public static void ResetKeyStates(bool forced) {
            Logger.Log("Reset keystates");
            foreach (KKey key in Enum.GetValues(typeof(KKey))) {

                long time = GetKeyHoldTime(key);

                if (time >= 0 && (forced || time >= Setup.Instance.Settings.KeyStateFixTimestep)) {
                    Logger.Log("Reset " + key);
                    MockInput input = new MockInput(key, false, InputEventType.Keyboard);
                    input.ActivationType = ActivationEventType.OnFirstRelease;
                    input.Unique = true;
                    AddAbsoluteEvent(input);
                    AddKeyEvent(input);
                }
            }
            KeyDownCountReset();
            LastKeyResetTime = Timer.Milliseconds;
        }

        private static void KeyDownCountReset() {
            KeyDownCount = 0;
            foreach (KKey k in keyDown.Dictionary.Keys) {
                if (IsPressingKey(k)) {
                    KeyDownCount++;
                }
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
