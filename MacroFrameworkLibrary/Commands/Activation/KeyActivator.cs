﻿using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for key event callbacks
    /// </summary>
    public class KeyActivator : CommandActivator {

        #region fields
        /// <summary>
        /// The delegate for a key event callback
        /// </summary>
        /// <param name="k"></param>
        public delegate void KeyEventCallback(KeyEvent k);
        private VKey key;
        private KeyEventCallback cb;
        #endregion

        /// <summary>
        /// Creates a <see cref="KeyActivator"/> instance
        /// </summary>
        /// <param name="callback">The key event callback</param>
        /// <param name="key">The key for which you wish to receive callbacks on</param>
        public KeyActivator(KeyEventCallback callback, VKey key) : base(null) {
            this.key = key;
            this.cb = callback;
        }

        protected override bool IsActivatorActive() {
            return KeyEvents.CurrentKeyEvent.Key == key;
        }

        public override void Execute() {
            try {
                cb?.Invoke(KeyEvents.CurrentKeyEvent);
            } catch (Exception e) {
                Console.WriteLine("Error executing KeyCallback: " + e.Message);
            }
        }
    }
}