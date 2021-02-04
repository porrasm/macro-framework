using MacroFramework.Commands.Attributes;
using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// Single key activator
    /// </summary>
    public class KeyActivator : CommandActivator {

        #region fields
        public delegate void KeyCallback(KeyEvent k);
        private VKey key;
        private KeyCallback cb;
        #endregion

        /// <summary>
        /// Creates a key activator instance
        /// </summary>
        /// <param name="callback">The callback method</param>
        /// <param name="key"></param>
        public KeyActivator(KeyCallback callback, VKey key) : base(null) {
            this.key = key;
            this.cb = callback;
        }

        public override bool IsActive() {
            return KeyEvents.CurrentKeyEvent.Key == key;
        }

        protected override void ExecuteCallback() {
            try {
                cb?.Invoke(KeyEvents.CurrentKeyEvent);
            } catch (Exception e) {
                Console.WriteLine("Error executing KeyCallback: " + e.Message);
            }
        }
    }
}