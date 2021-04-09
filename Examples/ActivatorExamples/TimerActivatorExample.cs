using MacroFramework.Commands;
using System;

class TimerActivatorExample : Command {

    // Alternate to using attributes
    protected override void InitializeActivators(ref ActivatorContainer acts) {
        new TimerActivator(1, TimeUnit.Seconds, false, CalledEverySecond).AssignTo(acts);
        new TimerActivator(1, TimeUnit.Hours, true, CalledEveryHourAndAtApplicationStart).AssignTo(acts);
    }

    [TimerActivator(1, TimeUnit.Seconds)]
    private void CalledEverySecond() {
        Console.WriteLine("A second has passed!");
    }

    [TimerActivator(1, TimeUnit.Seconds, true)]
    private void CalledEveryHourAndAtApplicationStart() {
        Console.WriteLine("An hour has passed!");
    }
}
