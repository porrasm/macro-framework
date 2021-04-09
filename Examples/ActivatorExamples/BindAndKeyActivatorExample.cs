using MacroFramework.Commands;
using System;

public class BindAndKeyActivatorExample : Command {

    // Alternate to using attributes
    protected override void InitializeActivators(ref ActivatorContainer acts) {

        new KeyActivator(KKey.Space, OnPressSpacebar).AssignTo(acts);

        // Defaults to ordered press of [A, B, C] and then releasing any key.
        new BindActivator(new Bind(KKey.A, KKey.B, KKey.C), OnReleaseABC).AssignTo(acts);

        // Activated when A is followed by B is followed by C and when no other keys are pressed
        new BindActivator(new Bind(new BindSettings(ActivationEventType.OnPress, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered), KKey.A, KKey.B, KKey.C), OnPressABC).AssignTo(acts);

        // Activated when A-Z or 0-9 is pressed
        new KeyActivator((e) => (e.Key >= KKey.A && e.Key <= KKey.Z) || (e.Key >= KKey.D0 && e.Key <= KKey.D9), OnPressAlphanumeric).AssignTo(acts);
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

    private void OnPressAlphanumeric(IInputEvent e) {
        Console.WriteLine($"Press alphanumeric key {e.Key}!");
    }
}
