﻿using MacroFramework.Tools;
using System;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for timer based actions
    /// </summary>
    public class TimerActivator : CommandActivator {

        #region fields
        private long lastExecute;
        private long delay;
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new <see cref="TimerActivator"/> instance
        /// </summary>
        /// <param name="command">Callback</param>
        /// <param name="delay">Minimum delay between the end of last execution and the beginning of a new one. If 0 the callback is called at at every update loop, see <see cref="MacroSettings.MainLoopTimestep"/></param>
        /// <param name="callAtStart">If true the command is called on the first update loop</param>
        /// <param name="unit">The unit of time used</param>
        public TimerActivator(int delay, TimeUnit unit = TimeUnit.Seconds, bool callAtStart = false, Action command = null) : base(command) {
            this.delay = ToMilliseconds(delay, unit);
            if (!callAtStart) {
                lastExecute = Timer.Milliseconds;
            }
        }
        #endregion

        /// <summary>
        /// Sets the callback for this activator
        /// </summary>
        /// <param name="cb">The callback to use</param>
        public TimerActivator SetCallback(Action cb) {
            CommandCallback = cb;
            return this;
        }

        protected override bool IsActivatorActive() {
            return Timer.PassedFrom(lastExecute) >= delay;
        }

        public override void Execute() {
            base.Execute();
            lastExecute = Timer.Milliseconds;
        }

        /// <summary>
        /// Converts given time unit to milliseconds
        /// </summary>
        /// <param name="time"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static long ToMilliseconds(long time, TimeUnit unit) {
            if (time < 0) {
                return 0;
            }
            switch (unit) {
                case TimeUnit.Seconds:
                    return time * 1000;
                case TimeUnit.Minutes:
                    return time * 60000;
                case TimeUnit.Hours:
                    return time * 216000000;
                default:
                    return time;
            }
        }
    }
}
