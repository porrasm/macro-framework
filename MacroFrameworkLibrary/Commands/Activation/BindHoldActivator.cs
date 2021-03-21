using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for keybind hold callbacks. Provides more detailed events than a regular <see cref="BindActivator"/>
    /// </summary>
    public class BindHoldActivator : CommandActivator {

        #region fields
        private BindActivator bindActivator;
        private DynamicActivator timerActivator;
        private Command.CommandCallback onActivate, onUpdate, onDeactivate;

        public override Type UpdateGroup => typeof(BindActivator);
        #endregion

        /// <summary>
        /// Creates a new <see cref="BindHoldActivator"/> instance
        /// </summary>
        /// <param name="bindActivator">The tem</param>
        /// <param name="onUpdate"></param>
        /// <param name="onActivate"></param>
        /// <param name="onDeactivate"></param>
        public BindHoldActivator(BindActivator bindActivator, Command.CommandCallback onUpdate, Command.CommandCallback onActivate = null, Command.CommandCallback onDeactivate = null) : base(null) {
            if (bindActivator.CommandCallback != null) {
                throw new Exception("The activator should not have a command callback");
            }
            if (bindActivator.Owner != null) {
                throw new Exception("The activator should not have an owner");
            }
            this.bindActivator = bindActivator;
            this.onActivate = onActivate;
            this.onUpdate = onUpdate;
            this.onDeactivate = onDeactivate;
        }

        protected override bool IsActivatorActive() {
            return bindActivator.IsActive();
        }

        public override void Execute() {
            End();
            timerActivator = new TimerActivator(Update, 0).RegisterDynamicActivator(false);
            onActivate?.Invoke();
        }

        private void Update() {
            if (!bindActivator.IsActive()) {
                End();
                return;
            }
            onUpdate?.Invoke();
        }
        private void End() {
            if (timerActivator != null) {
                CommandContainer.RemoveDynamicActivator(timerActivator.ID);
                timerActivator = null;
                onDeactivate?.Invoke();
            }
        }
    }
}
