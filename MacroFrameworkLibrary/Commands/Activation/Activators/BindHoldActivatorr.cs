using MacroFramework.Tools;
using System;

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
                Bind fixedBind = new Bind(new BindSettings(ActivationEventType.OnPress, value.Settings.MatchType, value.Settings.Order), value.Keys);
                this.bindActivator = new BindActivator(fixedBind);
            }
        }
        private BindActivator bindActivator;
        private DynamicActivator timerActivator;

        /// <summary>
        /// Called when the <see cref="Bind"/> becomes active. Called before <see cref="OnUpdate"/> and <see cref="OnDeactivate"/>.
        /// </summary>
        public Action OnActivate { get; set; }

        /// <summary>
        /// Called on each main loop iteration for as long as the <see cref="Bind"/> stays active. Called after <see cref="OnActivate"/> and before <see cref="OnDeactivate"/>. Gets the current hold time (ms) as a paremeter.
        /// </summary>
        public Action<int> OnUpdate { get; set; }

        /// <summary>
        /// Called when the <see cref="Bind"/> becomes inactive. Called after <see cref="OnActivate"/> and <see cref="OnUpdate"/>. Gets the total hold time (ms) as a paremeter.
        /// </summary>
        public Action<int> OnDeactivate { get; set; }

        public override Type UpdateGroup => typeof(BindActivator);

        private long startTime;
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
        public BindHoldActivator(Bind bind, Action onActivate, Action<int> onUpdate, Action<int> onDeactivate) : base(null) {
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
        public BindHoldActivator SetOnActivate(Action cb) {
            this.OnActivate = cb;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OnUpdate"/>
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public BindHoldActivator SetOnUpdate(Action<int> cb) {
            this.OnUpdate = cb;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OnDeactivate"/>
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public BindHoldActivator SetOnDeactivate(Action<int> cb) {
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
            startTime = Timer.Milliseconds;
            OnActivate?.Invoke();
        }

        private void Update() {
            if (!bindActivator.IsActive()) {
                End();
                return;
            }
            OnUpdate?.Invoke((int)Timer.PassedFrom(startTime));
        }
        private void End() {
            if (timerActivator != null) {
                CommandContainer.RemoveDynamicActivator(timerActivator);
                timerActivator = null;
                OnDeactivate?.Invoke((int)Timer.PassedFrom(startTime));
            }
        }
    }
}
