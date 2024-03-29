﻿using System;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace MacroFramework.Commands {
    public class Command : CommandBase {
        /// <summary>
        /// A static command which is automatically initialized at application start. Static commands are essentially singletons and only one can exist at a time.
        /// </summary>
        /// <returns></returns>
        internal override bool IsStatic() => true;

        public Command() {
            if (Macros.State != Macros.RunState.Initializing) {
                throw new Exception("You cannot create new instances of static commands. Use RuntimeCommand instead");
            }
        }
    }

    public class RuntimeCommand : CommandBase {
        /// <summary>
        /// A static command which is automatically initialized at application start. Static commands are essentially singletons and only one can exist at a time.
        /// </summary>
        /// <returns></returns>
        internal override bool IsStatic() => false;

        /// <summary>
        /// Adds this command instance to the application if it isn't already registered.
        /// </summary>
        /// <returns>this</returns>
        public RuntimeCommand Register() {
            if (!IsRegisteredToApp) {
                CommandContainer.AddCommand(this);
            }

            return this;
        }

        /// <summary>
        /// Removes the command at the end of the current update loop if it is registered.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Unregister() {
            if (IsRegisteredToApp) {
                CommandContainer.RemoveCommand(this);
            }
        }


    }

    /// <summary>
    /// Base class for all macro functionality
    /// </summary>
    public abstract partial class CommandBase {

        #region fields
        /// <summary>
        /// If true this <see cref="CommandBase"/> instance is currently registered in the framework. Always true for instances of <see cref="Command"/> (except in the very beginning after starting).
        /// </summary>
        public bool IsRegisteredToApp { get; internal set; }

        private static long IDCounter = 0;
        private readonly long id;

        /// <summary>
        /// The unique ID of this command
        /// </summary>
        public long UniqueID => id;

        internal abstract bool IsStatic();

        /// <summary>
        /// The list of <see cref="IActivator"/> instances this command owns
        /// </summary>
        internal IActivator[] Activators { get; }

        /// <summary>
        /// Override this property to create custom contexts for your command. If false is returned, none of the activators in <see cref="ActivatorGroup"/> are active eiher and this <see cref="CommandBase"/> instance is effectively disabled for the moment.
        /// </summary>
        public virtual Func<bool> IsActiveDelegate { private get; set; }

        /// <summary>
        /// The order in which the <see cref="CommandBase"/> instances are executed in
        /// </summary>
        public virtual int ExecutionOrderIndex => 0;
        #endregion

        #region initialization
        /// <summary>
        /// Creates a new <see cref="Command"/> instance
        /// </summary>
        internal CommandBase() {
            this.id = IDCounter++;
            ActivatorContainer activators = ActivatorContainer.New;
            InitializeActivators(ref activators);

            if (activators.Activators == null) {
                throw new Exception("Can't reassign ActivatorContainer");
            }

            InitializeAttributeActivators(ref activators);

            this.Activators = activators.Activators.ToArray();
        }

        private void InitializeAttributeActivators(ref ActivatorContainer activators) {
            try {
                MethodInfoAttributeCont[] methods = GetAttributeMethods();
                foreach (MethodInfoAttributeCont cont in methods) {
                    activators.Activators.Add(cont.Attribute.GetCommandActivator(this, cont.Method));
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
        /// Virtual method for registering activators of a <see cref="Command"/> instance. Activators can only be added to a command in this method. Use <see cref="IDynamicActivator"/>s instead if you wish to register activators on the fly.
        /// </summary>
        protected virtual void InitializeActivators(ref ActivatorContainer acts) { }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        internal void Dispose() {
            StopAllCoroutines();
            OnClose();
        }

        /// <summary>
        /// Returns true if the command is active. Identical to the result of <see cref="IsActiveDelegate"/> or true if <see cref="IsActiveDelegate"/> is null. Called even if the <see cref="CommandBase"/> is inactive.
        /// </summary>
        public bool IsActive() => IsActiveDelegate?.Invoke() ?? true;

        /// <summary>
        /// Called before the execution of any <see cref="IActivator"/> callback starts
        /// </summary>
        protected internal virtual void OnExecuteStart() { }

        /// <summary>
        /// Called after the execution of every <see cref="IActivator"/> callback
        /// </summary>
        protected internal virtual void OnExecutionComplete() { }

        /// <summary>
        /// Called after <see cref="Macros.Start(MacroSetup, bool, Action)"/>. Called even if the <see cref="Command"/> is inactive.
        /// </summary>
        public virtual void OnStart() { }

        /// <summary>
        /// Called on every iteration of <see cref="Macros.OnMainLoop"/> before any <see cref="IActivator"/> updates. Differs only in exectuion order with a <see cref="TimerActivator"/> with a 0 as a parameter.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Called after <see cref="Macros.Stop"/>. Called even if the <see cref="Command"/> is inactive.
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
        /// This method is called whenever a text command is executed but only if the command is active
        /// </summary>
        /// <param name="command">The text command which was executed</param>
        /// <param name="commandWasAccepted">True if any <see cref="TextActivator"/> instance executed the text command. False if command was not executed.</param>
        public virtual void OnTextCommand(string command, bool commandWasAccepted) { }

        public override bool Equals(object obj) {
            return obj is CommandBase @base &&
                   id == @base.id;
        }

        public override int GetHashCode() {
            return 1877310944 + id.GetHashCode();
        }
    }
}
