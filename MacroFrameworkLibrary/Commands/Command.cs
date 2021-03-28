using System;
using System.Linq;
using System.Reflection;
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
        protected CommandActivatorGroup activatorGroup;
        internal CommandActivatorGroup ActivatorGroup => activatorGroup;

        /// <summary>
        /// The deleget bool used to determine whether a <see cref="Command"/> instance is active
        /// </summary>
        /// <returns></returns>
        public delegate bool CommandContext();

        /// <summary>
        /// The default context used in all <see cref="Command"/> instances. Returns true on default but can be changed.
        /// </summary>
        public static CommandContext DefaultContext = () => true;
        #endregion

        #region initialization
        /// <summary>
        /// Creates a new <see cref="Command"/> instance
        /// </summary>
        public Command() {
            InitializeActivators(out activatorGroup);
            InitializeAttributeActivators();
        }

        private void InitializeAttributeActivators() {
            try {
                MethodInfoAttributeCont[] methods = GetAttributeMethods();
                foreach (MethodInfoAttributeCont cont in methods) {
                    activatorGroup.Add(cont.Attribute.GetCommandActivator(this, cont.Method));
                }
            } catch (Exception e) {
                throw new Exception("Unable to load Attributes from Assembly on type " + GetType() + ", message: " + e.Message, e);
            }
        }

        private MethodInfoAttributeCont[] GetAttributeMethods() {
            return GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttributes(typeof(ActivatorAttribute), false).Length > 0)
                .Select(m => new MethodInfoAttributeCont(m, m.GetCustomAttribute<ActivatorAttribute>())).ToArray();
        }


        /// <summary>
        /// Abstract method for initializing <see cref="Commands.IActivator"/> and class functionality. Use this like you would use a constructor. CommandActivators array mustn't be null and has to have at least 1 activator.
        /// </summary>
        protected virtual void InitializeActivators(out CommandActivatorGroup group) {
            group = new CommandActivatorGroup(this);
        }
        #endregion
        /// <summary>
        /// Override this method to create custom contexts for your command. If false is returned, none of the activators in <see cref="ActivatorGroup"/> are active eiher and this <see cref="Command"/> instance is effectively disabled for the moment.
        /// </summary>
        /// <returns></returns>
        public CommandContext IsActive = () => true;

        /// <summary>
        /// Called before the execution of any <see cref="IActivator"/> callback starts
        /// </summary>
        protected internal virtual void OnExecuteStart() { }

        /// <summary>
        /// Called after the execution of every <see cref="IActivator"/> callback
        /// </summary>
        protected internal virtual void OnExecutionComplete() { }

        /// <summary>
        /// Called after <see cref="Macros.Start(Setup)"/>
        /// </summary>
        public virtual void OnStart() { }

        /// <summary>
        /// Called on every iteration of <see cref="Macros.OnMainLoop"/> before any <see cref="IActivator"/> updates. Differs only in exectuion order with a <see cref="TimerActivator"/> with a 0 as a parameter.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Called after <see cref="Macros.Stop"/>
        /// </summary>
        public virtual void OnClose() { }

        /// <summary>
        /// Called when the framrwork is paused
        /// </summary>
        public virtual void OnPause() { }

        /// <summary>
        /// Called when the framrwork is unpaused
        /// </summary>
        public virtual void OnResume() { }

        /// <summary>
        /// This method is called whenever a text command is executed
        /// </summary>
        /// <param name="command">The text command which was executed</param>
        /// <param name="commandWasAccepted">True if any <see cref="TextActivator"/> instance executed the text command. False if command was not executed.</param>
        public virtual void OnTextCommand(string command, bool commandWasAccepted) { }
    }
}
