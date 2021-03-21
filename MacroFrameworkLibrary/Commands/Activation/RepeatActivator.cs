﻿using System;
using System.Collections.Generic;
using System.Text;
using MacroFramework.Tools;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for repeating activators (e.g. double click). Requires that you set <see cref="MacroSettings.MainLoopTimestep"/> low enough.
    /// </summary>
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
        public Command.CommandCallback OnEachActivate { get; set; }

        public override Type UpdateGroup => typeof(TimerActivator);

        private DynamicActivator dynamicActivator;

        private bool isLocalActivatorActive;
        private int currentRepeatCount;
        private long lastActiveTimeStamp;
        #endregion

        public RepeatActivator(Command.CommandCallback callback, IActivator activator) : base(callback) {
            this.activator = activator;
            dynamicActivator = new DynamicActivator(activator, false);
            dynamicActivator.OnExecute = OnLocalActivatorActive;
            CommandContainer.AddDynamicActivator(dynamicActivator);
        }

        private void OnLocalActivatorActive() {
            isLocalActivatorActive = true;
            OnEachActivate?.Invoke();
        }

        protected override bool IsActivatorActive() {
            if (currentRepeatCount > 0 && Timer.PassedFrom(lastActiveTimeStamp) > RepeatInterval) {
                bool isActive = !ActivateImmediately && IsRepeatActivatorActive();
                Reset();
                if (isActive) {
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
            currentRepeatCount = 0;
            lastActiveTimeStamp = 0;
        }
        private bool IsRepeatActivatorActive() {
            return DisallowExtraRepeats ? currentRepeatCount == RepeatCount : currentRepeatCount >= RepeatCount;
        }
    }
}