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
        private Command owner;
        public List<ICommandActivator> Activators { get; }
        #endregion

        public CommandActivatorGroup(Command owner = null) {
            this.owner = owner;
            Activators = new List<ICommandActivator>();
        }

        /// <summary>
        /// Adds an activator to the group
        /// </summary>
        /// <param name="activator"></param>
        public void AddActivator(ICommandActivator activator) {
            activator.Owner = owner;
            Activators.Add(activator);
        }

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