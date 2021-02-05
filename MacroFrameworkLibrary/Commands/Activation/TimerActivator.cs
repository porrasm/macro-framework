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
        /// <param name="delay">Minimum delay in milliseconds between the end of last executionm and the beginning of a new one. If 0 the callback is called at at every update loop, see <see cref="MacroSettings.MainLoopTimestep"/></param>
        /// <param name="callAtStart">If true the command is called on the first update loop</param>
        public TimerActivator(Command.CommandCallback command, long delay, bool callAtStart = false) : base((s) => command()) {
            this.delay = delay;
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
    }

    /// <summary>
    /// Attribute activator for <see cref="TimerActivator"/>
    /// </summary>
    public class TimerActivatorAttribute : ActivatorAttribute {

        private long delay;
        private bool callAtStart;

        public TimerActivatorAttribute(long delay, bool callAtStart= false) {
            this.delay = delay;
            this.callAtStart = callAtStart;
        }

        public override ICommandActivator GetCommandActivator(Command c, MethodInfo m) {
            return new TimerActivator(() => m.Invoke(c, null), delay, callAtStart);
        }
    }
}
