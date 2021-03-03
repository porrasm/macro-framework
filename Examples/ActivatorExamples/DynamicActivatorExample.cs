using MacroFramework.Commands;
using System;
using System.Diagnostics;

public class DynamicActivatorExample : Command {

    [BindActivator(KKey.LAlt, KKey.F4, KKey.Delete)]
    private async void Shutdown() {
        Console.WriteLine("Confirm shutdown by pressing 'Y'");

        if (await new KeyActivator(KKey.Y).WaitForActivation(5000)) {
            Process.Start(new ProcessStartInfo("shutdown", "/s /t 0") {
                CreateNoWindow = true,
                UseShellExecute = false
            });
        } else {
            Console.WriteLine("Shutdown canceled");
        }
    }

    public override void OnStart() {
        CommandActivator onSpace = new KeyActivator(OneTimeOnPressSpace, KKey.Space);

        // One time bind for space, lambda expression indicates that the activator is discarded after execution
        onSpace.RegisterDynamicActivator(() => true);
    }

    private void OneTimeOnPressSpace(IInputEvent e) {
        Console.WriteLine("Pressed space!");
    }
}
