using MacroFramework.Commands.Activation;
using MacroFramework.Input;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    /// <summary>
    /// A wrapper class for multiple <see cref="CommandActivator"/> instances
    /// </summary>
    public class CommandActivatorGroup {

        #region fields
        public List<ICommandActivator> Activators { get; }
        #endregion

        public CommandActivatorGroup() {
            Activators = new List<ICommandActivator>();
        }

        #region new activators
        #region regular bind
        /// <summary>
        /// Adds a <see cref="BindActivator"/>
        /// </summary>
        public CommandActivatorGroup Bind(Command.CommandCallback cb, params VKey[] keys) {
            return Bind(cb, ActivationEventType.OnRelease, KeyPressOrder.Ordered, keys);
        }

        /// <summary>
        /// Adds a <see cref="BindActivator"/>
        /// </summary>
        public CommandActivatorGroup Bind(Command.CommandCallback cb, ActivationEventType activationType, KeyPressOrder order, params VKey[] keys) {
            Activators.Add(new BindActivator(cb, activationType, order, keys));
            return this;
        }
        #endregion

        #region keybind
        /// <summary>
        /// Adds a <see cref="KeyActivator"/>
        /// </summary>
        public CommandActivatorGroup KeyBind(KeyActivator.KeyCallback cb, params VKey[] keys) {
            foreach (VKey k in keys) {
                Activators.Add(new KeyActivator(cb, k));
            }
            return this;
        }
        #endregion

        #region text command
        /// <summary>
        /// Adds a <see cref="TextActivator"/>
        /// </summary>
        public CommandActivatorGroup TextCommand(Command.CommandCallback cb, params RegexWrapper[] matchers) {
            return TextCommand((cmd) => cb?.Invoke(), matchers);
        }

        /// <summary>
        /// Adds a <see cref="TextActivator"/>
        /// </summary>
        public CommandActivatorGroup TextCommand(Command.TextCommandCallback cb, params RegexWrapper[] matchers) {
            Activators.Add(new TextActivator(cb, matchers));
            return this;
        }
        #endregion

        /// <summary>
        /// Adds an activator to the group
        /// </summary>
        /// <param name="activator"></param>
        public void AddActivator(ICommandActivator activator) {
            Activators.Add(activator);
        }
        #endregion

        /// <summary>
        /// Returns true if any of the <see cref="CommandActivator"/> instances is active
        /// </summary>
        /// <param name="activeActivator"></param>
        /// <returns></returns>
        public bool IsActive(out CommandActivator activeActivator) {
            foreach (CommandActivator act in Activators) {
                if (act.IsActive()) {
                    activeActivator = act;
                    return true;
                }
            }
            activeActivator = null;
            return false;
        }
    }
}