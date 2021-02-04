using MacroFramework.Commands.Activation;
using MacroFramework.Commands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    public abstract class Command {

        #region fields
        public delegate void TextCommandCallback(string command);
        public delegate void CommandCallback();

        protected CommandActivatorGroup activator;
        public bool UsesActivator => activator.Activators.Count > 0;

        public virtual bool GetContext() => true;

        private struct MethodInfoAttributeCont {
            public MethodInfo Method;
            public ActivatorAttribute Attribute;
            public MethodInfoAttributeCont(MethodInfo method, ActivatorAttribute activator) {
                Method = method;
                Attribute = activator;
            }
        }
        #endregion

        #region initialization
        public Command() {
            InitializeActivators(out activator);
            InitializeAttributeActivators();
        }

        private void InitializeAttributeActivators() {
            MethodInfoAttributeCont[] methods = GetActivatorAttributeMethods();
            foreach (MethodInfoAttributeCont cont in methods) {
                activator.AddActivator(cont.Attribute.GetCommandActivator(cont.Method));
            }
        }

        private MethodInfoAttributeCont[] GetActivatorAttributeMethods() {
            return GetType().Assembly.GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(ICommandActivator), false).Length > 0)
                      .Select(m => {
                          if (m.GetParameters().Length > 0) {
                              throw new InvalidOperationException("A method with an ActivatorAttribute may not take any arguments.");
                          }
                          return new MethodInfoAttributeCont(m, (ActivatorAttribute)m.GetCustomAttribute(typeof(ActivatorAttribute)));
                      })
                      .ToArray();
        }

        /// <summary>
        /// Abstract method for initializing CommandActivators and class functionality. Use this like you would use a constructor. CommandActivators array mustn't be null and has to have at least 1 activator.
        /// </summary>
        protected virtual void InitializeActivators(out CommandActivatorGroup activator) {
            activator = new CommandActivatorGroup();
        }
        #endregion

        /// <summary>
        /// Called before the execution starts.
        /// </summary>
        /// <param name="command"></param>
        protected virtual void OnExecuteStart() { }

        /// <summary>
        /// Called after the execution is complete.
        /// </summary>
        /// <param name="command"></param>
        protected virtual void OnExecutionComplete() { }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        public virtual void OnStart() { }

        /// <summary>
        /// Called when the application quits.
        /// </summary>
        public virtual void OnClose() { }

        /// <summary>
        /// This method is called whenever a text command is executed, even if it doesn't match any of the activators.
        /// </summary>
        /// <param name="command"></param>
        public virtual void OnCommand(string command) { }

        public void ExecuteIfActive() {
            try {
                if (activator.IsActive(out CommandActivator a) && GetContext()) {
                    OnExecuteStart();
                    a.Execute();
                    OnExecutionComplete();
                }
            } catch (Exception e) {
                Console.WriteLine("Error executing command: " + e.Message);
            }
        }
    }
}
