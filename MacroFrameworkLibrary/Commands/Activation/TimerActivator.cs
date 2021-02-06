using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="CommandActivator"/> instance for timer based actions. Callback is called at least
    /// </summary>
    public class TimerActivator : CommandActivator {

        private long lastExecute;
        private long delay;

        /// <summary>
        /// Initializes a new timer activator instance
        /// </summary>
        /// <param name="command">Callback</param>
        /// <param name="delay">Minimum delay between the end of last executionm and the beginning of a new one. If 0 the callback is called at at every update loop, see <see cref="MacroSettings.MainLoopTimestep"/></param>
        /// <param name="callAtStart">If true the command is called on the first update loop</param>
        /// <param name="unit">The unit of time used</param>
        public TimerActivator(Command.CommandCallback command, int delay, TimeUnit unit = TimeUnit.Seconds, bool callAtStart = false) : base(command) {
            this.delay = ToMilliseconds(delay, unit);
            if (!callAtStart) {
                lastExecute = Timer.Milliseconds;
            }
        }

        public override bool IsActive() {
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

    /// <summary>
    /// Attribute activator for <see cref="TimerActivator"/>
    /// </summary>
    public class TimerActivatorAttribute : ActivatorAttribute {

        private int delay;
        private TimeUnit unit;
        private bool callAtStart;

        /// <summary>
        /// Initializes a new timer activator instance
        /// </summary>
        /// <param name="command">Callback</param>
        /// <param name="delay">Minimum delay between the end of last executionm and the beginning of a new one. If 0 the callback is called at at every update loop, see <see cref="MacroSettings.MainLoopTimestep"/></param>
        /// <param name="callAtStart">If true the command is called on the first update loop</param>
        /// <param name="unit">The unit of time used</param>
        public TimerActivatorAttribute(int delay, TimeUnit unit = TimeUnit.Seconds, bool callAtStart= false) {
            this.delay = delay;
            this.unit = unit;
            this.callAtStart = callAtStart;
        }

        public override ICommandActivator GetCommandActivator(Command c, MethodInfo m) {
            return new TimerActivator(() => m.Invoke(c, null), delay, unit, callAtStart);
        }
    }
}
