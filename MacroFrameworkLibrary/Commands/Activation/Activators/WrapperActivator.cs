using System;
using System.Collections.Generic;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for wrapping multiple activators
    /// </summary>
    public class WrapperActivator : CommandActivator {
        #region fields
        /// <summary>
        /// The list of active <see cref="IActivator"/> instances
        /// </summary>
        public List<IActivator> ActiveActivators { get; private set; }

        public override Type UpdateGroup => typeof(TimerActivator);
        #endregion

        /// <summary>
        /// Creates a new <see cref="WrapperActivator"/> instance
        /// </summary>
        /// <param name="callback">The callback to use</param>
        /// <param name="activators">The <see cref="IActivator"/> instances to use</param>
        public WrapperActivator(Action callback, params IActivator[] activators) : base(callback) {
            foreach (IActivator act in activators) {
                this.ActiveActivators.Add(act);
            }
        }
        /// <summary>
        /// Creates a new <see cref="WrapperActivator"/> instance
        /// </summary>
        /// <param name="activators">The <see cref="IActivator"/> instances to use</param>
        public WrapperActivator(params IActivator[] activators) : base(null) {
            foreach (IActivator act in activators) {
                this.ActiveActivators.Add(act);
            }
        }

        /// <summary>
        /// Sets the callback for this activator
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public WrapperActivator SetCallback(Action cb) {
            CommandCallback = cb;
            return this;
        }

        protected override bool IsActivatorActive() {
            foreach (IActivator act in ActiveActivators) {
                if (act.IsActive()) {
                    return true;
                }
            }
            return false;
        }
    }
}
