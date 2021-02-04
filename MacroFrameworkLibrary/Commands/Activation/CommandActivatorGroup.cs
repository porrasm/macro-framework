using MacroFramework.Input;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    public class CommandActivatorGroup {

        #region fields
        public List<CommandActivator> Activators { get; }
        #endregion

        public CommandActivatorGroup() {
            Activators = new List<CommandActivator>();
        }

        #region new activators
        #region regular bind
        public CommandActivatorGroup Bind(Command.CommandCallbackParameterless cb, params VKey[] keys) {
            return Bind((cmd) => cb.Invoke(), ActivationEventType.OnRelease, false, keys);
        }
        public CommandActivatorGroup Bind(Command.CommandCallback cb, params VKey[] keys) {
            return Bind(cb, ActivationEventType.OnRelease, false, keys);
        }

        public CommandActivatorGroup Bind(Command.CommandCallbackParameterless cb, ActivationEventType activationType, bool ordered, params VKey[] keys) {
            return Bind((cmd) => cb.Invoke(), activationType, ordered, keys);
        }
        public CommandActivatorGroup Bind(Command.CommandCallback cb, ActivationEventType activationType, bool ordered, params VKey[] keys) {
            Activators.Add(new BindActivator(cb, activationType, ordered, keys));
            return this;
        }
        #endregion

        #region keybind
        public CommandActivatorGroup KeyBind(KeyActivator.KeyCallback cb, params VKey[] keys) {
            foreach (VKey k in keys) {
                Activators.Add(new KeyActivator(cb, k));
            }
            return this;
        }
        #endregion

        #region text command
        public CommandActivatorGroup TextCommand(Command.CommandCallbackParameterless cb, params RegexWrapper[] matchers) {
            return TextCommand((cmd) => cb?.Invoke(), matchers);
        }
        public CommandActivatorGroup TextCommand(Command.CommandCallback cb, params RegexWrapper[] matchers) {
            Activators.Add(new TextActivator(cb, matchers));
            return this;
        }
        #endregion

        public void AddActivator(CommandActivator activator) {
            Activators.Add(activator);
        }
        #endregion

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
