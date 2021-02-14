using MacroFramework.Input;
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
        /// <summary>
        /// The key event activation filter for this activator
        /// </summary>
        public ActivationEventType ActivationType { get; set; }
        /// <summary>
        /// The key match filter for this activator
        /// </summary>
        public KeyMatchType MatchType { get; set; }
        /// <summary>
        /// The key order filter for this activator
        /// </summary>
        public KeyPressOrder Order { get; set; }
        /// <summary>
        /// The keys which need to be pressed
        /// </summary>
        public KKey[] Keys { get; set; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="BindActivator"/> instance
        /// </summary>
        /// <param name="command">The callback to be called when this activator becomes active</param>
        /// <param name="keys"><see cref="Keys"/></param>
        /// <param name="activationType">see<see cref="ActivationType"/></param>
        /// <param name="matchType"><see cref="MatchType"/></param>
        /// <param name="order"><see cref="Order"/></param>
        public BindActivator(Command.CommandCallback command, KKey[] keys, ActivationEventType activationType = ActivationEventType.OnFirstRelease, KeyMatchType matchType = KeyMatchType.ExactKeyMatch, KeyPressOrder order = KeyPressOrder.Ordered) : base(command) {
            VerifyKeys(keys);
            this.ActivationType = activationType;
            this.MatchType = matchType;
            this.Order = order;
            this.Keys = keys;
        }

        private static void VerifyKeys(KKey[] keys) {
            foreach (KKey k in keys) {
                if (k.IsStateless()) {
                    throw new Exception("BindActivators cannot use stateless keys: " + k);
                }
            }
        }

        protected override bool IsActivatorActive() {
            IInputEvent k = InputEvents.CurrentInputEvent;
            if (IsMatchingActivationEventType(k.ActivationType)) {
                return KeyStates.IsMatchingKeyState(MatchType, Order, Keys);
            }
            return false;
        }

        private bool IsMatchingActivationEventType(ActivationEventType type) {
            if (ActivationType == ActivationEventType.Any) {
                return true;
            }
            if (ActivationType == ActivationEventType.OnAnyRelease && (type == ActivationEventType.OnFirstRelease || type == ActivationEventType.OnAnyRelease)) {
                return true;
            }
            return ActivationType == type;
        }
    }
}