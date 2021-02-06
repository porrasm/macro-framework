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
        private KeyMatchType matchType;
        private KeyPressOrder order;

        public VKey[] Keys { get; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="BindActivator"/> instance
        /// </summary>
        /// <param name="command">The callback which is executed when this becomes active</param>
        /// <param name="activationType">The eventy type filter</param>
        /// <param name="order">Determines whether keys should be pressed in the given parameter order</param>
        /// <param name="matchType">Match type corresponding to how the match is determined</param>
        /// <param name="keys"></param>
        public BindActivator(Command.CommandCallback command, VKey[] keys, ActivationEventType activationType = ActivationEventType.OnFirstRelease, KeyMatchType matchType = KeyMatchType.ExactKeyMatch, KeyPressOrder order = KeyPressOrder.Ordered) : base((s) => command()) {
            this.activationType = activationType;
            this.matchType = matchType;
            this.order = order;
            this.Keys = keys;
        }

        public override bool IsActive() {
            KeyEvent k = KeyEvents.CurrentKeyEvent;
            if (IsMatchingActivationEventType(k.ActivationType)) {
                return KeyState.IsMatchingKeyState(matchType, order, Keys);
            }
            return false;
        }

        private bool IsMatchingActivationEventType(ActivationEventType type) {
            if (activationType == ActivationEventType.Any) {
                return true;
            }
            if (activationType == ActivationEventType.OnAnyRelease && (type == ActivationEventType.OnFirstRelease || type == ActivationEventType.OnAnyRelease)) {
                return true;
            }
            return activationType == type;
        }
    }

    /// <summary>
    /// Activator attribute for <see cref="BindActivator"/>
    /// </summary>
    public class BindActivatorAttribute : ActivatorAttribute {

        #region fields
        private bool useDefaults;
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
        public BindActivatorAttribute(params VKey[] keys) {
            this.Keys = keys;
            useDefaults = true;
        }

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
            if (useDefaults) {
                return new BindActivator(() => m.Invoke(c, null), Keys);
            }
            return new BindActivator(() => m.Invoke(c, null), Keys, activationType: activationType, order: order);
        }
    }
}