using System.Reflection;

namespace MacroFramework.Commands {
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
