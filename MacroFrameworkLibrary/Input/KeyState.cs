using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Input {
    public static class KeyState {

        #region fields
        private static Dictionary<VKey, bool> absoluteVKeytates;
        /// <summary>
        /// Returns the dictionary containig VKeytates. This dictionary should not be used for keybinds.
        /// </summary>
        public static Dictionary<VKey, bool> AbsoluteVKeytates => absoluteVKeytates;

        private static Dictionary<VKey, State> keyDown;
        private static Dictionary<VKey, State> keyUp;

        private static Dictionary<VKey, KeyCallback> keyCallbacks;

        private static int globalIndex;

        public delegate void KeyCallback(KeyEvent k);

        private struct State {
            public int localIndex;
            public int globalIndex;
        }

        public static long LastKeyPress { get; private set; }
        #endregion

        static KeyState() {
            Init();
        }

        public static void Init() {

            globalIndex = 0;

            absoluteVKeytates = new Dictionary<VKey, bool>();
            keyDown = new Dictionary<VKey, State>();
            keyUp = new Dictionary<VKey, State>();

            // RaspberryVKey = new RaspberryVKey();

            foreach (VKey key in Enum.GetValues(typeof(VKey))) {
                if (absoluteVKeytates.ContainsKey(key)) {
                    continue;
                }
                absoluteVKeytates.Add(key, false);
                keyDown.Add(key, new State());
                keyUp.Add(key, new State());
            }

            keyCallbacks = new Dictionary<VKey, KeyCallback>();
        }

        public static long TimeSinceLastKeyPress() {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - LastKeyPress;
        }

        /// <summary>
        /// Checks for an invalid VKey code e.g. a nonkey event sent by a Gamepad.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsInvalidKey(VKey key) {
            return !absoluteVKeytates.ContainsKey(key);
        }

        /// <summary>
        /// This method is used to keep track on which VKey are pressed down.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddAbsoluteEvent(KeyEvent k) {
            LastKeyPress = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            absoluteVKeytates[k.Key] = k.KeyState;
        }

        public static bool IsUniqueEvent(KeyEvent k) {
            if (k.KeyState) {
                if (PressingKey(k.Key)) {
                    return false;
                }
            }
            return true;
        }

        public static bool AddKeyEvent(KeyEvent k) {

            if (!IsUniqueEvent(k)) {
                return false;
            }

            Console.WriteLine("Key event: " + k);

            if (k.KeyState) {
                State state = keyDown[k.Key];
                SetState(ref state);
                keyDown[k.Key] = state;

                if (keyCallbacks.TryGetValue(k.Key, out KeyCallback cb)) {
                    cb?.Invoke(k);
                }
            } else {
                State state = keyUp[k.Key];
                SetState(ref state);
                keyUp[k.Key] = state;

                if (keyCallbacks.TryGetValue(k.Key, out KeyCallback cb)) {
                    cb?.Invoke(k);
                }
            }

            return true;
        }

        private static void SetState(ref State state) {
            globalIndex++;
            state.localIndex++;
            state.globalIndex = globalIndex;
        }

        #region bind checks
        public static bool PressingKeys(params VKey[] VKey) {
            if (VKey == null || VKey.Length == 0) {
                return false;
            }
            foreach (VKey key in VKey) {
                if (!PressingKey(key)) {
                    Console.WriteLine("Not pressing: " + key);
                    return false;
                }
            }

            //AddKeyEvent(new KeyEvent(MacroSettings.GeneralBindKey, false));
            return true;
        }
        public static bool PressingKeysInOrder(params VKey[] VKey) {
            if (VKey == null || VKey.Length == 0) {
                return false;
            }
            int prevIndex = 0;
            foreach (VKey k in VKey) {
                if (!PressingKey(k)) {
                    return false;
                }
                State state = keyDown[k];
                if (prevIndex > state.globalIndex) {
                    return false;
                }
                prevIndex = state.globalIndex;
            }

            //AddKeyEvent(new KeyEvent(MacroSettings.GeneralBindKey, false));
            return true;
        }
        public static bool PressingKey(VKey key) {
            return keyDown[key].globalIndex > keyUp[key].globalIndex;
        }
        #endregion

        public static void ResetKeys() {
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
        /// Calls the <paramref name="callback"/> function when a keyevent for <paramref name="key"/> happens.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public static void SubscribeToKeyEventOld(VKey key, KeyCallback callback) {
            if (keyCallbacks.ContainsKey(key)) {
                keyCallbacks[key] += callback;
            } else {
                keyCallbacks.Add(key, callback);
            }
        }
    }
}
