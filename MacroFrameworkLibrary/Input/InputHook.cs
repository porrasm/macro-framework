using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MacroFramework.Input {
    public static class InputHook {

        #region variables
        public static bool Enabled { get; private set; }

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;

        private static IntPtr KeyboardHookID = IntPtr.Zero;
        private static IntPtr MouseHookID = IntPtr.Zero;

        private static LowLevelKeyboardProc keyboardProc = KeyboardCallback;
        private static LowLevelKeyboardProc mouseProc = MouseCallback;

        private static IntPtr BlockCode { get => new IntPtr(-1); }

        private static IntPtr WM_KEYDOWN, WM_KEYUP;
        #endregion

        static InputHook() {
            WM_KEYDOWN = (IntPtr)0x0100;
            WM_KEYUP = (IntPtr)0x0101;
        }

        #region imports
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterShellHookWindow(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        #endregion

        public static void StartHooks() {
            KeyboardHookID = SetKeyboardHook(keyboardProc);
            //MouseHookID = SetMouseHook(mouseProc);
            Enabled = true;
            Console.WriteLine("Starting hooks");
        }

        public static void StopHooks() {
            UnhookWindowsHookEx(KeyboardHookID);
            //UnhookWindowsHookEx(MouseHookID);
            Enabled = false;
            Console.WriteLine("Stopped device hooks");
        }

        private static IntPtr KeyboardCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0) {
                VKey key = (VKey)Marshal.ReadInt32(lParam);

                if (wParam == WM_KEYDOWN) {
                    if (KeyEvents.OnHookKeyPress(new KeyEvent(key, true))) {
                        return BlockCode;
                    }
                } else if (wParam == WM_KEYUP) {
                    if (KeyEvents.OnHookKeyPress(new KeyEvent(key, false))) {
                        return BlockCode;
                    }
                }
            }

            return CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);
        }

        private static IntPtr MouseCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0) {
                int mouseInput = Marshal.ReadInt32(lParam); ;

                Console.WriteLine("Mouse input: " + mouseInput);

                // Handle mouse input
            }

            return CallNextHookEx(MouseHookID, nCode, wParam, lParam);
        }

        private static IntPtr SetKeyboardHook(LowLevelKeyboardProc proc) {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, curModule.BaseAddress, 0);
            }
        }

        private static IntPtr SetMouseHook(LowLevelKeyboardProc proc) {
            using (Process process = Process.GetCurrentProcess())
            using (ProcessModule module = process.MainModule) {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, module.BaseAddress, 0);
            }
        }
    }
}
