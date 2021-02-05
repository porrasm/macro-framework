using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public override void Execute() {
            try {
                cb?.Invoke(KeyEvents.CurrentKeyEvent);
            } catch (Exception e) {
                Console.WriteLine("Error executing KeyCallback: " + e.Message);
            }
        }
    }

    /// <summary>
    /// Attribute activator for <see cref="KeyActivator"/>. Use <see cref="KeyEvents.CurrentKeyEvent"/> to get current keyevent without a parameter.
    /// </summary>
    public class KeyActivatorAttribute : ActivatorAttribute {

        private VKey key;

        /// <summary>
        /// Creates a key activator instance from this method
        /// </summary>
        /// <param name="key">Key</param>
        public KeyActivatorAttribute(VKey key) {
            this.key = key;
        }

        public override ICommandActivator GetCommandActivator(Command c, MethodInfo m) {
            return new KeyActivator((k) => m.Invoke(c, null), key);
        }
    }
}