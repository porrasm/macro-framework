using MacroFramework.Commands;
using System;

class TimerActivatorExample : Command {

    // Alternate to using attributes
    protected override void InitializeActivators(out CommandActivatorGroup activator) {
        activator = new CommandActivatorGroup(this);

        activator.Add(new TimerActivator(1, TimeUnit.Seconds, false, CalledEverySecond));
        activator.Add(new TimerActivator(1, TimeUnit.Hours, true, CalledEveryHourAndAtApplicationStart));
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
