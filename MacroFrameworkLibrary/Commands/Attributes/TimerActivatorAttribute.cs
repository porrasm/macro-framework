using System;
using System.Reflection;

namespace MacroFramework.Commands {
    /// <summary>
    /// <see cref="Attribute"/> for easily creating a <see cref="TimerActivator"/>
    /// </summary>
    public class TimerActivatorAttribute : ActivatorAttribute {

        private int delay;
        private TimeUnit unit;
        private bool callAtStart;

        /// <summary>
        /// Creates a new <see cref="TimerActivator"/> instance at the start of the application from this method.
        /// </summary>
        /// <param name="delay">Minimum delay between last execution and new execution start/param>
        /// <param name="callAtStart">If true the command is called on the first update loop</param>
        /// <param name="unit">The unit of time used</param>
        public TimerActivatorAttribute(int delay, TimeUnit unit = TimeUnit.Seconds, bool callAtStart = false) {
            this.delay = delay;
            this.unit = unit;
            this.callAtStart = callAtStart;
        }

        public override IActivator GetCommandActivator(Command c, MethodInfo m) {
            return new TimerActivator(() => m.Invoke(c, null), delay, unit, callAtStart);
        }
    }
}
