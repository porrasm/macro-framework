using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for keybind hold callbacks. Provides more detailed events than a regular <see cref="BindActivator"/>
    /// </summary>
    public class BindHoldActivator : CommandActivator {

        #region fields
        /// <summary>
        /// The bind to use
        /// </summary>
        public Bind Bind {
            get => bindActivator.Bind;
            set {
                this.bindActivator = new BindActivator(value);
            }
        }
        private BindActivator bindActivator;
        private DynamicActivator timerActivator;

        /// <summary>
        /// Called when the <see cref="Bind"/> becomes active. Called before <see cref="OnUpdate"/> and <see cref="OnDeactivate"/>.
        /// </summary>
        public Command.CommandCallback OnActivate { get; set; }

        /// <summary>
        /// Called on each main loop iteration for as long as the <see cref="Bind"/> stays active. Called after <see cref="OnActivate"/> and before <see cref="OnDeactivate"/>.
        /// </summary>
        public Command.CommandCallback OnUpdate { get; set; }

        /// <summary>
        /// Called when the <see cref="Bind"/> becomes inactive. Called after <see cref="OnActivate"/> and <see cref="OnUpdate"/>.
        /// </summary>
        public Command.CommandCallback OnDeactivate { get; set; }

        public override Type UpdateGroup => typeof(BindActivator);
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new <see cref="BindHoldActivator"/> instance
        /// </summary>
        /// <param name="bind">The bind to use</param>
        public BindHoldActivator(Bind bind) : base(null) {
            this.Bind = bind;
        }

        /// <summary>
        /// Creates a new <see cref="BindHoldActivator"/> instance
        /// </summary>
        /// <param name="bind">The bind to use</param>
        /// <param name="onActivate"><see cref="OnActivate"/></param>
        /// <param name="onUpdate"><see cref="OnUpdate"/></param>
        /// <param name="onDeactivate"><see cref="OnDeactivate"/></param>
        public BindHoldActivator(Bind bind, Command.CommandCallback onActivate, Command.CommandCallback onUpdate, Command.CommandCallback onDeactivate) : base(null) {
            this.Bind = bind;
            OnActivate = onActivate;
            OnUpdate = onUpdate;
            OnDeactivate = onDeactivate;
        }
        #endregion

        #region callbacks
        /// <summary>
        /// Sets the <see cref="OnActivate"/>
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public BindHoldActivator SetOnActivate(Command.CommandCallback cb) {
            this.OnActivate = cb;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OnUpdate"/>
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public BindHoldActivator SetOnUpdate(Command.CommandCallback cb) {
            this.OnUpdate = cb;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OnDeactivate"/>
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public BindHoldActivator SetOnDeactivate(Command.CommandCallback cb) {
            this.OnDeactivate = cb;
            return this;
        }
        #endregion

        protected override bool IsActivatorActive() {
            return bindActivator.IsActive();
        }

        public override void Execute() {
            End();
            timerActivator = new TimerActivator(0).SetCallback(Update).RegisterDynamicActivator(false);
            OnActivate?.Invoke();
        }

        private void Update() {
            if (!bindActivator.IsActive()) {
                End();
                return;
            }
            OnUpdate?.Invoke();
        }
        private void End() {
            if (timerActivator != null) {
                CommandContainer.RemoveDynamicActivator(timerActivator.ID);
                timerActivator = null;
                OnDeactivate?.Invoke();
            }
        }
    }
}
