using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// Base class for all macro functionality
    /// </summary>
    public abstract class Command {

        #region fields
        /// <summary>
        /// Callback for command actions
        /// </summary>
        public delegate void CommandCallback();

        /// <summary>
        /// Container for the set of <see cref="CommandActivator"/> instances of this command
        /// </summary>
        protected CommandActivatorGroup activator;
        internal CommandActivatorGroup Activator => activator;

        /// <summary>
        /// Additional method for declaring contexts. Return false to disable all command functionality.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Initializes a new command instance
        /// </summary>
        public Command() {
            InitializeActivators(out activator);
            InitializeAttributeActivators();
        }

        private void InitializeAttributeActivators() {
            try {
                MethodInfoAttributeCont[] methods = GetAttributeMethods();
                foreach (MethodInfoAttributeCont cont in methods) {
                    activator.AddActivator(cont.Attribute.GetCommandActivator(this, cont.Method));
                }
            } catch (Exception e) {
                throw new Exception("Unable to load Attributes from Assembly on type " + GetType(), e);
            }
        }

        private MethodInfoAttributeCont[] GetAttributeMethods() {
            return GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttributes(typeof(ActivatorAttribute), false).Length > 0)
                .Select(m => new MethodInfoAttributeCont(m, m.GetCustomAttribute<ActivatorAttribute>())).ToArray();
        }


        /// <summary>
        /// Abstract method for initializing CommandActivators and class functionality. Use this like you would use a constructor. CommandActivators array mustn't be null and has to have at least 1 activator.
        /// </summary>
        protected virtual void InitializeActivators(out CommandActivatorGroup activator) {
            activator = new CommandActivatorGroup();
        }
        #endregion

        /// <summary>
        /// Called before the execution of any command starts.
        /// </summary>
        protected internal virtual void OnExecuteStart() { }

        /// <summary>
        /// Called after the execution of every command
        /// </summary>
        protected internal virtual void OnExecutionComplete() { }

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
        /// <param name="command">The text command which was executed</param>
        /// <param name="commandWasAccepted">True if any of the <see cref="Command"/> classes executed the text command. False if nonexistent text command.</param>
        public virtual void OnTextCommand(string command, bool commandWasAccepted) { }

        /// <summary>
        /// Syntax macro for creating array of <see cref="VKey"/> elements
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected VKey[] Keys(params VKey[] keys) {
            return keys;
        }
    }
}
