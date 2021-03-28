using System;
using System.Reflection;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="Attribute"/> for easily creating a <see cref="KeyActivator"/>. Attribute methods are parameterless, use <see cref="Input.InputEvents.CurrentInputEvent"/> to get the current key event.
    /// </summary>
    public class KeyActivatorAttribute : ActivatorAttribute {

        private KKey key;

        /// <summary>
        /// Creates a new <see cref="KeyActivator"/> instance at the start of the application from this method
        /// </summary>
        /// <param name="key">Key</param>
        public KeyActivatorAttribute(KKey key) {
            this.key = key;
        }

        public override IActivator GetCommandActivator(Command command, MethodInfo assignedMethod) {
            return new KeyActivator(key, (IInputEvent k) => assignedMethod.Invoke(command, null));
        }
    }
}
