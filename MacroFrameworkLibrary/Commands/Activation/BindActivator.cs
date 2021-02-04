using MacroFramework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroFramework.Commands {
    public class BindActivator : CommandActivator {

        #region fields
        private ActivationEventType activationType;
        private bool ordered;

        public VKey[] Keys { get; }
        #endregion

        public BindActivator(Command.CommandCallback command, ActivationEventType activationType, bool ordered, VKey[] keys) : base(command) {
            this.activationType = activationType;
            this.ordered = ordered;
            this.Keys = keys;
        }

        public override bool IsActive() {
            ActivationEventType currentType = KeyEvents.CurrentKeyEvent.KeyState ? ActivationEventType.OnPress : ActivationEventType.OnRelease;
            if (activationType == ActivationEventType.Any || activationType == currentType) {
                Console.WriteLine("Current matching activation type: " + currentType);

                foreach (var k in Keys) {
                    Console.WriteLine("    Current Keys: " + k);
                }

                if (ordered) {
                    return KeyState.PressingKeysInOrder(Keys);
                } else {
                    return KeyState.PressingKeys(Keys);
                }
            }
            return false;
        }

        protected override void ExecuteCallback() {
            command?.Invoke(null);
        }
    }
}
