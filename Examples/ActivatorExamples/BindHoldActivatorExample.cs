using System;
using MacroFramework.Commands;

public class BindHoldActivatorExample : Command {

    protected override void InitializeActivators(ref ActivatorContainer acts) {
        Bind space = new Bind(KKey.Space);
        new BindHoldActivator(space)
            .SetOnActivate(PressSpaceDown)
            .SetOnUpdate(HoldingSpaceDown)
            .SetOnDeactivate(ReleaseSpace)
            .AssignTo(acts);
    }

    private void PressSpaceDown() {
        Console.WriteLine("Press space down");
    }

    private void HoldingSpaceDown() {
        Console.WriteLine("Holding space down");
    }

    private void ReleaseSpace() {
        Console.WriteLine("Release space");
    }
}
