using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MacroFramework.Commands.Attributes {
    /// <summary>
    /// A container object for method and their <see cref="ActivatorAttribute"/>
    /// </summary>
    internal struct MethodInfoAttributeCont {
        public MethodInfo Method;
        public ActivatorAttribute Attribute;
        public MethodInfoAttributeCont(MethodInfo method, ActivatorAttribute activator) {
            Method = method;
            Attribute = activator;
        }
    }
}
