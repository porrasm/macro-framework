using System;
using System.Reflection;

namespace MacroFramework.Commands {
    /// <summary>
    /// The base class for all <see cref="IActivator"/> attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ActivatorAttribute : Attribute {
        /// <summary>
        /// Returns a corresponding <see cref="IActivator"/> object
        /// </summary>
        /// <param name="owner">Command which owns <paramref name="assignedMethod"/></param>
        /// <param name="assignedMethod">The method to call</param>
        /// <returns></returns>
        public abstract IActivator GetCommandActivator(Command owner, MethodInfo assignedMethod);
    }
}
