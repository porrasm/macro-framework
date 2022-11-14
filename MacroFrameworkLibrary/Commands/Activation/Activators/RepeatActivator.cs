using MacroFramework.Tools;
using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for repeating activators (e.g. double click). Requires that you set <see cref="MacroSettings.MainLoopTimestep"/> low enough.
    /// </summary>

    // TODO NOT WORKING
    public class RepeatActivator : CommandActivator {
        #region fields
        private IActivator activator;

        /// <summary>
        /// The interval in milliseconds deciding how quickly the activator should be active again. Default = 400ms
        /// </summary>
        public int RepeatInterval { get; set; } = 400;

        /// <summary>
        /// The amount of times the activator should be activated. Default = 2
        /// </summary>
        public int RepeatCount { get; set; } = 2;

        /// <summary>
        /// If true, the repeat activator is activated if the repeat count is exactly <see cref="RepeatCount"/>. Default = true
        /// </summary>
        public bool DisallowExtraRepeats { get; set; } = true;

        /// <summary>
        /// Whether to activate the activator immediately or wait until <see cref="RepeatInterval"/> expires. Default = false
        /// </summary>
        public bool ActivateImmediately { get; set; } = false;

        /// <summary>
        /// This delegate is called each time the associated activator activates
        /// </summary>
        public Action OnEachActivate { get; set; }

        public override Type UpdateGroup => activator.UpdateGroup;

        private bool isLocalActivatorActive;
        private int currentRepeatCount;
        private long lastActiveTimeStamp;
        #endregion

        /// <summary>
        /// Creates a new <see cref="RepeatActivator"/> instance
        /// </summary>
        /// <param name="activator">The activator to use</param>
        /// <param name="callback">The callback to execute</param>
        public RepeatActivator(IActivator activator, Action callback = null) : base(callback) {
            this.activator = activator;
        }

        /// <summary>
        /// Sets the callback for this activator
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public RepeatActivator SetCallback(Action cb) {
            CommandCallback = cb;
            return this;
        }

        private void OnLocalActivatorActive() {
            isLocalActivatorActive = true;
            OnEachActivate?.Invoke();
        }

        protected override bool IsActivatorActive() {
            if (activator.IsActive()) {
                OnLocalActivatorActive();
            }

            if (currentRepeatCount > 0 && Timer.PassedFrom(lastActiveTimeStamp) > RepeatInterval) {
                bool isActive = !ActivateImmediately && IsRepeatActivatorActive();
                Reset();
                if (isActive) {
                    Console.WriteLine("Repeat active with count: " + currentRepeatCount);
                    return true;
                }
            }

            if (isLocalActivatorActive) {
                isLocalActivatorActive = false;
                currentRepeatCount++;
                lastActiveTimeStamp = Timer.Milliseconds;
            }

            if (ActivateImmediately) {
                return IsRepeatActivatorActive();
            }
            return false;
        }

        private void Reset() {
            Console.WriteLine("Reset repeat act");
            currentRepeatCount = 0;
            lastActiveTimeStamp = 0;
        }
        private bool IsRepeatActivatorActive() {
            return DisallowExtraRepeats ? currentRepeatCount == RepeatCount : currentRepeatCount >= RepeatCount;
        }
    }
}
