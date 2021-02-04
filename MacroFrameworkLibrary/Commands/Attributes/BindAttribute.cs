using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class ActivatorAttribute : Attribute {
        public ActivatorAttribute(CommandActivator act) {

        }
    }

    class Asd : Command {

        [Bind(ActivationEventType.OnPress, false, MacroSettings.GeneralBindKey, VKey.A)]
        private void BindTest() {

            Asd a = new Asd();

            
        }
    }
}
