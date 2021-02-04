using MacroFramework.Commands.Activation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MacroFramework.Commands.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ActivatorAttribute : Attribute {
        public abstract ICommandActivator GetCommandActivator(MethodInfo m);
    }
}
