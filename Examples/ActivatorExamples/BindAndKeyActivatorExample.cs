using MacroFramework.Commands;
using System;

public class BindAndKeyActivatorExample : Command {

    // Alternate to using attributes
    protected override void InitializeActivators(out CommandActivatorGroup activator) {
        activator = new CommandActivatorGroup(this);

        activator.AddActivator(new KeyActivator(OnPressSpacebar, KKey.Space));

        // Defaults to ordered press of [A, B, C] and then releasing any key.
        activator.AddActivator(new BindActivator(OnReleaseABC, Keys(KKey.A, KKey.B, KKey.C)));

        // Activated when A is followed by B is followed by C and when no other keys are pressed
        activator.AddActivator(new BindActivator(OnPressABC, Keys(KKey.A, KKey.B, KKey.C), ActivationEventType.OnPress, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered));
    }

    [KeyActivator(KKey.Space)]
    private void OnPressSpacebar(IInputEvent i) {
        Console.WriteLine("Pressed space " + i.State);
    }

    // Defaults to ordered press of [A, B, C] and then releasing any key.
    [BindActivator(KKey.A, KKey.B, KKey.C)]
    private void OnReleaseABC() {
        Console.WriteLine("Pressed ABC in order and released!");
    }

    // Activated when A is followed by B is followed by C and when no other keys are pressed
    [BindActivator(ActivationEventType.OnPress, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered, KKey.A, KKey.B, KKey.C)]
    private void OnPressABC() {
        Console.WriteLine("Pressed ABC in order!");
    }
}
