using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MacroFramework.Commands {
    /// <summaryICommandActivator
    /// The base class for all <see cref="CommandActivator"/> attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ActivatorAttribute : Attribute {
        /// <summary>
        /// Returns a corresponding <see cref="ICommandActivator"/> object
        /// </summary>
        /// <param name="c">Command which owns <paramref name="m"/></param>
        /// <param name="m">The method to call</param>
        /// <returns></returns>
        public abstract ICommandActivator GetCommandActivator(Command c, MethodInfo m);
    }
}
