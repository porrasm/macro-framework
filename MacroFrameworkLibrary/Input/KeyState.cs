using MacroFramework.Commands;
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
    public static class KeyState {

        #region fields
        /// <summary>
        /// Returns the dictionary containig VKeytates. This dictionary should not be used for keybinds.
        /// </summary>
        internal static Dictionary<VKey, bool> AbsoluteKeystates { get; private set; }

        private static Dictionary<VKey, State> keyDown;
        private static Dictionary<VKey, State> keyUp;

        private static uint globalIndex;

        /// <summary>
        /// The key up/down states of the last two key events
        /// </summary>
        private static bool prevKeyEventState, prevPrevKeyEventState;

        /// <summary>
        /// How many keys are held down at any given moment
        /// </summary>
        private static int keyDownCount;

        private struct State {
            public uint localIndex;
            public uint globalIndex;
        }

        public static long LastKeyPress { get; private set; }
        #endregion

        static KeyState() {
            Init();
        }

        internal static void Init() {

            globalIndex = 0;

            AbsoluteKeystates = new Dictionary<VKey, bool>();
            keyDown = new Dictionary<VKey, State>();
            keyUp = new Dictionary<VKey, State>();

            // RaspberryVKey = new RaspberryVKey();

            foreach (VKey key in Enum.GetValues(typeof(VKey))) {
                if (AbsoluteKeystates.ContainsKey(key)) {
                    continue;
                }
                AbsoluteKeystates.Add(key, false);
                keyDown.Add(key, new State());
                keyUp.Add(key, new State());
            }
        }

        /// <summary>
        /// Time since last keypress in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long TimeSinceLastKeyPress() {
            return Timer.PassedFrom(LastKeyPress);
        }

        /// <summary>
        /// Checks for an invalid VKey code e.g. a nonkey event sent by a Gamepad.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static bool IsInvalidKey(VKey key) {
            return !AbsoluteKeystates.ContainsKey(key);
        }

        /// <summary>
        /// This method is used to keep track on which VKey are pressed down.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void AddAbsoluteEvent(VKey key, bool state) {
            LastKeyPress = Timer.Milliseconds;
            prevPrevKeyEventState = prevKeyEventState;
            prevKeyEventState = state;
            AbsoluteKeystates[key] = state;
        }

        internal static bool IsUniqueEvent(KeyEvent k) {
            if (k.KeyState) {
                if (IsPressingKey(k.Key)) {
                    return false;
                }
            }
            return true;
        }

        internal static bool AddKeyEvent(KeyEvent k) {

            if (!IsUniqueEvent(k)) {
                return false;
            }

            Console.WriteLine("Key event: " + k);

            if (k.KeyState) {
                keyDownCount++;
                State state = keyDown[k.Key];
                SetState(ref state);
                keyDown[k.Key] = state;
            } else {
                keyDownCount--;
                State state = keyUp[k.Key];
                SetState(ref state);
                keyUp[k.Key] = state;
            }

            return true;
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
        public static bool IsMatchingKeyState(KeyMatchType matchType, KeyPressOrder order, params VKey[] keys) {
            if (matchType == KeyMatchType.ExactKeyMatch) {
                return IsExactKeyMatch(order, keys);
            } else {
                return IsPressingKeys(order, keys);
            }
        }

        private static bool IsExactKeyMatch(KeyPressOrder order, params VKey[] keys) {
            if (!IsPressingKeys(order, keys)) {
                return false;
            }
            return keys.Length == keyDownCount;
        }

        private static bool IsPressingKeys(KeyPressOrder order, params VKey[] keys) {
            if (order == KeyPressOrder.Ordered) {
                return IsPressingKeysInOrder(keys);
            } else {
                return IsPressingKeysInArbitraryOrder(keys);
            }
        }

        private static bool IsPressingKeysInOrder(VKey[] keys) {
            if (keys == null || keys.Length == 0) {
                return false;
            }
            uint prevIndex = 0;
            foreach (VKey k in keys) {
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

        private static bool IsPressingKeysInArbitraryOrder(VKey[] keys) {
            foreach (VKey k in keys) {
                if (!IsPressingKey(k)) {
                    return false;
                }
            }
            return true;
        }

        public static bool  IsPressingKey(VKey key) {
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
        public static ActivationEventType GetCurrentActivationEventType() {
            if (prevKeyEventState) {
                return ActivationEventType.OnPress;
            } else {
                if (prevPrevKeyEventState) {
                    return ActivationEventType.OnFirstRelease;
                } else {
                    return ActivationEventType.OnAnyRelease;
                }
            }
        }
    }
}
