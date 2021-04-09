using MacroFramework.Commands;
using MacroFramework.Input;
using System;
using System.Collections;
using System.Diagnostics;

public class CoroutineExample : Command {

    private Coroutine coroutine;

    public override void OnStart() {
        coroutine = StartCoroutine(ShutDownPC, OnCancel);
    }

    private IEnumerator ShutDownPC() {
        Console.WriteLine("Confirm shutdown by pressing 'Y'");

        // Wait for next input event
        yield return new WaitForInputEvent();
        if (InputEvents.CurrentInputEvent.Key != KKey.Y) {
            yield break;
        }

        Console.WriteLine("Are you sure?");

        // Wait for Y again, cancel coroutine if Y not pressed within 5 seconds
        // NOTE: The cancel function will only be called the next time a keyevent is captured.
        //       This is because coroutines are updated by their UpdateGroup which happens to be a keyvent in this case.
        yield return new WaitForBind(new Bind(KKey.Y)).SetTimeout(5);

        Console.WriteLine("Shutting down pc in 30 seconds");

        // wait 15 seconds
        yield return new WaitFor(15, TimeUnit.Seconds);
        // wait another 15 seconds,this syntax defaults to milliseconds
        yield return 15000;

        // Wait for 1 update cycle, useful if you wish to execute heavy calculations, you can pause the computation to allow for other functionality
        yield return null;

        Process.Start(new ProcessStartInfo("shutdown", "/s /t 0") {
            CreateNoWindow = true,
            UseShellExecute = false
        });
    }
    private void OnCancel(Coroutine coroutine) {
        Console.WriteLine("Coroutine cancelled");
    }


    [BindActivator(KKey.Backspace)]
    private void RestartCoroutine() {
        Console.WriteLine("Restaring entire sequene");
        coroutine.Restart();
    }

    [BindActivator(KKey.Delete)]
    private void CancelCoroutine() {
        Console.WriteLine("Stopping entire sequence");
        coroutine.Stop();
    }
}
