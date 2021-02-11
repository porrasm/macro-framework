using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Input {
    /// <summary>Extension methods for the enhanced Key enum</summary>
    public static class EnhancedKey {

        static EnhancedKey() {
            KeyMap = MapNormalKeys();
            KeyExtendedMap = MapExtendedKeys();
            StringToKey = MapStrings();
        }

        #region mapping
        /// <summary>Map virtual key codes to the Key enum</summary>
        public static Dictionary<VKey, KKey> KeyMap { get; }
        /// <summary>Map extended versions of virtual key codes to the Key enum</summary>
        public static Dictionary<VKey, KKey> KeyExtendedMap { get; }
        /// <summary>Map strings to the Key enum</summary>
        public static Dictionary<string, KKey> StringToKey { get; }

        private static Dictionary<VKey, KKey> MapNormalKeys() {
            var dict = new Dictionary<VKey, KKey>();

            foreach (KKey key in Enum.GetValues(typeof(KKey))) {
                if (!key.IsCustom() && key.IsKey() && !key.IsExtended()) {
                    var vkey = key.AsVirtualKey();
                    if (!dict.ContainsKey(vkey)) {
                        dict.Add(vkey, key);
                    }
                }
            }

            return dict;
        }

        private static Dictionary<VKey, KKey> MapExtendedKeys() {
            var dict = new Dictionary<VKey, KKey>();

            foreach (KKey key in Enum.GetValues(typeof(KKey))) {
                if (key.IsExtended()) {
                    var vkey = key.AsVirtualKey();
                    if (!dict.ContainsKey(vkey)) {
                        dict.Add(vkey, key);
                    }
                }
            }

            return dict;
        }

        private static Dictionary<string, KKey> MapStrings() {
            var toKey = new Dictionary<string, KKey>();

            foreach (KKey key in Enum.GetValues(typeof(KKey))) {
                var s = key.ToString().ToLower();
                if (!toKey.ContainsKey(s)) {
                    toKey.Add(s, key);
                }
            }

            return toKey;
        }
        #endregion

        #region flags
        /// <summary>Check if key has any of the flags given</summary>
        public static bool HasAny(this KKey key, KKey flags) => (key & flags) != 0;
        /// <summary>Check if key has all of the flags given</summary>
        public static bool HasAll(this KKey key, KKey flags) => (key & flags) == flags;

        /// <summary>Check if the Key value is a key instead of a flag etc.</summary>
        public static bool IsKey(this KKey key) => key.HasAny(KKey.M_KeyMask) && key != KKey.M_KeyMask;
        /// <summary>Check if the Key value is a flag</summary>
        public static bool IsFlag(this KKey key) => (key & ~KKey.M_FlagMask) == 0 && key != KKey.M_FlagMask;
        /// <summary>Check if the Key value is a mask</summary>
        public static bool IsMask(this KKey key) => key == KKey.M_KeyMask || key == KKey.M_FlagMask;
        /// <summary>Check if the Key value is a custom entry</summary>
        public static bool IsCustom(this KKey key) => key.HasFlag(KKey.F_Custom);
        /// <summary>Check if the key is a modifier</summary>
        public static bool IsModifier(this KKey key) => key.HasFlag(KKey.F_Modifier);
        /// <summary>Check if the key is a mouse key</summary>
        public static bool IsMouse(this KKey key) => key.HasFlag(KKey.F_Mouse);
        /// <summary>Check if the key is a numpad key</summary>
        public static bool IsNumpad(this KKey key) => key.HasFlag(KKey.F_Numpad);
        /// <summary>Check if the key represents a scroll event</summary>
        public static bool IsScroll(this KKey key) => key.HasFlag(KKey.F_Scroll);
        /// <summary>Check if the Key value represents mouse movement</summary>
        public static bool IsMouseMove(this KKey key) => key == KKey.MouseMove;
        /// <summary>Check if the key is a number</summary>
        public static bool IsNumber(this KKey key) => key.HasFlag(KKey.F_Number);
        /// <summary>Check if the key is has the extended property</summary>
        public static bool IsExtended(this KKey key) => key.HasFlag(KKey.F_Extended) && key.HasAny(KKey.M_KeyMask);
        /// <summary>Check if the key is a media key</summary>
        public static bool IsMedia(this KKey key) => key.HasFlag(KKey.F_Media);
        /// <summary>Check if the key produces a character when typed</summary>
        public static bool IsChar(this KKey key) => key.HasFlag(KKey.F_Char);
        /// <summary>Check if the key is stateless. Stateless keys have no up event.</summary>
        public static bool IsStateless(this KKey key) => key.HasFlag(KKey.F_Stateless);
        /// <summary>Check if the key is a toggleable key</summary>
        public static bool IsToggle(this KKey key) => key.HasFlag(KKey.F_Toggle);

        /// <summary>Check if the key is a keyboard key instead of a mouse or a custom key</summary>
        public static bool IsKeyboard(this KKey key) {
            return key.IsKey() && !key.HasAny(KKey.F_Mouse | KKey.F_Custom);
        }

        /// <summary>Check if the Key value is a modifier flag</summary>
        public static bool IsModifierFlag(this KKey key) => key.HasFlag(KKey.F_Modifier) && !key.HasAny(KKey.M_KeyMask);
        /// <summary>Check if the key is a left or right shift key</summary>
        public static bool IsShift(this KKey key) => key.HasFlag(KKey.Shift);
        /// <summary>Check if the key is a left or right control key</summary>
        public static bool IsCtrl(this KKey key) => key.HasFlag(KKey.Ctrl);
        /// <summary>Check if the key is a left or right win key</summary>
        public static bool IsWin(this KKey key) => key.HasFlag(KKey.Win);
        /// <summary>Check if the key is a left or right alt key</summary>
        public static bool IsAlt(this KKey key) => key.HasFlag(KKey.Alt);
        #endregion

        #region key casting
        /// <summary>Get the <see cref="VKey"/> equivalent of this <see cref="KKey"/></summary>
        public static VKey AsVirtualKey(this KKey key) {
            if (key.IsCustom())
                throw new Exception("Custom keys do not have virtual codes");
            if (!key.IsKey())
                throw new Exception("This enum member is not a key");
            return (VKey)(key & KKey.M_KeyMask);
        }

        /// <summary>Get the <see cref="KKey"/> equivalent of this <see cref="VKey"/>. If an extended key is not found, a non-extended version is returned if possible.</summary>
        /// <param name="key">The <see cref="VKey"/> to cast into a <see cref="ScanCode"/></param>
        /// <param name="extended">Set true to prioritize the extended version of the key (Example: Enter vs NumpadEnter). Returns non-extended version as fallback if not found and vice versa.</param>
        public static KKey AsCustom(this VKey key, bool extended) {
            if (extended) {
                if (KeyExtendedMap.ContainsKey(key))
                    return KeyExtendedMap[key];
                else if (KeyMap.ContainsKey(key))
                    return KeyMap[key];
            } else {
                if (KeyMap.ContainsKey(key))
                    return KeyMap[key];
                else if (KeyExtendedMap.ContainsKey(key))
                    return KeyExtendedMap[key];
            }

            throw new Exception("This virtual key is not defined in the Key enum.");
        }
        #endregion

        private static ushort scMask = 0xFF00;
        private static ushort scValue = 0xE000;

        /// <summary>Check if the <see cref="ScanCode"/> is an extended key</summary>
        public static bool IsExtended(this ScanCode sc) => ((ushort)sc & scMask) == scValue;
    }
}
