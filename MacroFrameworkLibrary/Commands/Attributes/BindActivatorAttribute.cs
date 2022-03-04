using System;
using System.Reflection;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="Attribute"/> for easily creating a <see cref="BindActivator"/>
    /// </summary>
    public class BindActivatorAttribute : ActivatorAttribute {

        #region fields
        private bool useDefaults;
        private ActivationEventType activationType;
        private KeyMatchType matchType;
        private KeyPressOrder order;

        private KKey[] keys;
        #endregion

        /// <summary>
        /// Creates a new <see cref="BindActivator"/> instance at the start of the application from this method
        /// </summary>
        /// <param name="keys"></param>
        public BindActivatorAttribute(params KKey[] keys) {
            this.keys = keys;
            useDefaults = true;
        }

        /// <summary>
        /// Creates a new <see cref="BindActivator"/> instance at the start of the application from this method
        /// </summary>
        /// <param name="activationType"><see cref="BindActivator.ActivationType"/></param>
        /// <param name="matchType"><see cref="BindActivator.MatchType"/></param>
        /// <param name="order"><see cref="BindActivator.Order"/></param>
        /// <param name="keys"><see cref="BindActivator.Keys"/></param>
        public BindActivatorAttribute(ActivationEventType activationType, KeyMatchType matchType, KeyPressOrder order, params KKey[] keys) {
            this.activationType = activationType;
            this.matchType = matchType;
            this.order = order;
            this.keys = keys;
        }

        public override IActivator GetCommandActivator(CommandBase command, MethodInfo assignedMethod) {
            if (useDefaults) {
                return new BindActivator(new Bind(keys), () => assignedMethod.Invoke(command, null));
            }
            return new BindActivator(new Bind(new BindSettings(activationType: activationType, matchType: matchType, order: order), keys), () => assignedMethod.Invoke(command, null));
        }
    }
}
