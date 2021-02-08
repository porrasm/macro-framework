﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MacroFramework.Commands.Attributes {
    /// <summary>
    /// <see cref="Attribute"/> for easily creating a <see cref="BindActivator"/>
    /// </summary>
    public class BindActivatorAttribute : ActivatorAttribute {

        #region fields
        private bool useDefaults;
        private ActivationEventType activationType;
        private KeyMatchType matchType;
        private KeyPressOrder order;

        private VKey[] keys;
        #endregion

        /// <summary>
        /// Creates a new <see cref="BindActivator"/> instance at the start of the application from this method
        /// </summary>
        /// <param name="keys"></param>
        public BindActivatorAttribute(params VKey[] keys) {
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
        public BindActivatorAttribute(ActivationEventType activationType, KeyMatchType matchType, KeyPressOrder order, params VKey[] keys) {
            this.activationType = activationType;
            this.matchType = matchType;
            this.order = order;
            this.keys = keys;
        }

        public override IActivator GetCommandActivator(Command command, MethodInfo assignedMethod) {
            if (useDefaults) {
                return new BindActivator(() => assignedMethod.Invoke(command, null), keys);
            }
            return new BindActivator(() => assignedMethod.Invoke(command, null), keys, activationType: activationType, matchType: matchType, order: order);
        }
    }
}
