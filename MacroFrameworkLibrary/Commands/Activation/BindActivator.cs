using MacroFramework.Commands.Activation;
using MacroFramework.Commands.Attributes;
using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for keybind callbacks
    /// </summary>
    public class BindActivator : CommandActivator {

        #region fields
        private ActivationEventType activationType;
        private KeyPressOrder order;

        public VKey[] Keys { get; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="BindActivator"/> instance
        /// </summary>
        /// <param name="command">The callback which is executed when this becomes active</param>
        /// <param name="activationType">The eventy type filter</param>
        /// <param name="order">Determines whether keys should be pressed in the given parameter order</param>
        /// <param name="keys"></param>
        public BindActivator(Command.CommandCallback command, ActivationEventType activationType, KeyPressOrder order, VKey[] keys) : base((s) => command()) {
            this.activationType = activationType;
            this.order = order;
            this.Keys = keys;
        }

        public override bool IsActive() {
            ActivationEventType currentType = KeyEvents.CurrentKeyEvent.KeyState ? ActivationEventType.OnPress : ActivationEventType.OnRelease;
            if (activationType == ActivationEventType.Any || activationType == currentType) {
                if (order == KeyPressOrder.Ordered) {
                    return KeyState.PressingKeysInOrder(Keys);
                } else {
                    return KeyState.PressingKeys(Keys);
                }
            }
            return false;
        }

        protected override void ExecuteCallback() {
            command?.Invoke(null);
        }
    }

    /// <summary>
    /// Activator attribute for <see cref="BindActivator"/>
    /// </summary>
    public class BindActivatorAttribute : ActivatorAttribute {

        #region fields
        private ActivationEventType activationType;
        private KeyPressOrder order;

        public VKey[] Keys { get; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="BindActivator"/> instance
        /// </summary>
        /// <param name="activationType">The eventy type filter</param>
        /// <param name="order">Determines whether keys should be pressed in the given parameter order</param>
        /// <param name="keys"></param>
        public BindActivatorAttribute(ActivationEventType activationType, KeyPressOrder order, params VKey[] keys) {
            this.activationType = activationType;
            this.order = order;
            this.Keys = keys;
        }

        public override ICommandActivator GetCommandActivator(Command c, MethodInfo m) {
            return new BindActivator(() => m.Invoke(c, null), activationType, order, Keys);
        }
    }
}