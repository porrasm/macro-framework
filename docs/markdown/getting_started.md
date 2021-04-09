# Getting started

MacroFramework is a simple framework for the Windwos platform to create macros, commands or event based actions in a very simple manner. Here is an in depth guide on how to get started with it.

## Setup

The project is available on [Nuget](https://www.nuget.org/packages/MacroFramework/). Visit the [GitHub](https://github.com/porrasm/windows-macro-framework-library) page for setup instructions.

## Using the framework

The framework is designed in such a way that it is very simple and fast to use. You only need to call the ```MacroFramework.Macros.Start()``` in your main method (with an STAThread attribute). All of your defined command classes will be automatically added using reflection if you defined the command assembly in the setup class. This is very helpful because when you want to add new functionality to your application you only need to create a new command class and start writing the functionality code without any other setup. The only setup you need to do is inherit the main setup class and then you are done.

## How it works

The framework is started with a single method ```MacroFramework.Macros.Start(Setup)``` and it can be stopped with ```MacroFramework.Macros.Stop()```. When the application starts it will start a global keyboard hook which listens to keyboard input. These keypresses will be caught in desktop mode and in most applications (e.g. some fullscreen games might prevent the keyboard hook from receiving events). These keyvents will be used for activating macros relates to keyvents. 

The framework also has a main loop which runs every X milliseconds. You can set this option in the [Setup](../html/class_macro_framework_1_1_setup.html) class. This main loop is used for timer based events (so the smaller the main loop delay the more accurate the timers are). The keyevents are also queued up and handled in the main loop.

The framework also offers support for text commands.

### Exception handling

The framework calls every bit of custom functionality in a try clause, preventing crashed caused by macro functionality. It cannot, however, catch errors in async methods. You must implement your own error handling if you use async methods.

Async method should not be needed for simple macro functionality because the framework offers synchronous alternatives (see IDynamicActivator and Coroutines).

The framework is able to handle most uncaught errors that happen in your code using a global exception handler. It is recommended that you create your own exception handler or disable the exception handler all together. 

The framework does use a global exception handler for uncaught errors but it's not very efficient. The global exception handler can be disabled from the settings class and you can use your own exception handler instead.

### Keyboard and mouse input

The keyboard and mouse input are available using the `KKey` enum.

### The command class

The [Command](../html/class_macro_framework_1_1_commands_1_1_command.html) class is the base class which you should inherit whenever you create some custom functionality. I recommend grouping similar commends together in a single Command class and creating a new class for each time you want to create some new functionality.

It is possible to use this framework without a single Command class. The IDynamicActivator interface offers an alternative which is more similar to that of AutoHotKey for example.

## Keybinds and keyevents

Keybinds and keyvents are some events which are run when some keys or keys are pressed (the [BindActivator](class_macro_framework_1_1_commands_1_1_bind_activator.html) and [KeyActivator](class_macro_framework_1_1_commands_1_1_key_activator.html)). 

You can configure them in such a way that either a keypress or a release activates the bind. You can define the order that the keys should be pressed in or choose to ignore the order altogether. You can also define whether an exact keys match is required.

Below are some examples of bind and key activators with different syntaxes. Use either an attribute or the AddActivator method to register commands but not both.

~~~{.cs}
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

~~~

### The general bind key

The framework offers support for a general bind key. You can define this in the settings using the [Setup](../html/class_macro_framework_1_1_setup.html) class. The general bind key is a key which will always be intercepted i.e. other programs will not receive this input. It will also intercept all keys when this key is being held down. 

This allows you to create binds without having to fear you will accidentally do something in another application. I have set my general key bind to caps lock (because it is a useless key) and therefore can assign the bind [Caps lock, alt, F4] without exiting the current application. The general key bind is accessible from the `KKey` enum with the name `KKEy.GeneralBindKey`.

## TimerActivator

The [TimerActivator](class_macro_framework_1_1_commands_1_1_timer_activator.html) is an activator which calls events based on timers. Each TimerActivator is updated at every main loop iteration so your main loop timestep affects how accurate these timer actions are.

Below are some examples of timer activators with different syntaxes. Use either an attribute or the AddActivator method to register commands but not both.

~~~{.cs}
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
~~~

## Text commands

The framework offers support for text commands. Any string can be executed using the [TextCommands](../html/class_macro_framework_1_1_commands_1_1_text_commands.html) class. You can choose to execute a command immediately (use cautiously as infinite loops may occur with careless exectuion) or queue them up and execute at the next update loop iteration.

### Command mode

The command mode is an optional feature which you can enable by settings of the [Setup](../html/class_macro_framework_1_1_setup.html) class by assigning `CommandKey` and `CommandActivateKey`.

The command mode is activated when the `CommandKey` is pressed and it will remain active until an incorrect key (such as Escape) is pressed. Allowed keys are [A-Z, 0-9] and some special keys. In the future this class will become public and you will be allowed to define your own functionality. Assigning the `CommandKey` will disable it altogether so choose a useless key (like caps lock or the windows key).

When the command mode is enabled, all key events are intercepted such that no application receives them. This allows you to type an alphanumeric command from anywhere in Winddows without causing any unwanted actions in other applications. Then press the `CommandActivateKey` to execute the command.

Example: `CommandKey = Windows key, CommandActivateKey = Enter`. The key event chain [Windows key, T, E, S, T, Space, C, O, M, M, A, N, D, Enter] will execute the command "test command".

### TextActivator

Below are some examples of text activators with different syntaxes. Use either an attribute or the AddActivator method to register commands but not both.

~~~{.cs}
using MacroFramework.Commands;
using System;
using System.Text.RegularExpressions;

class TextActivatorExample : Command {

    // Alternate to using attributes
    protected override void InitializeActivators(ref ActivatorContainer acts) {
        new TextActivator("test command", OnTestCommand).AssignTo(acts);

        // Multiple matchers
        new TextActivator(new Matchers("stop", "exit", "quit"), ExitApplication).AssignTo(acts);

        // Regex
        new TextActivator(new Regex("print [A-Z]+"), PrintParameter).AssignTo(acts);
    }

    [TextActivator("test command", TextActivatorAttribute.MatchType.StringMatch)]
    private void OnTestCommand() {
        Console.WriteLine("Test command executed!");
    }


    private void ExitApplication(string receivedCommand) {
        Console.WriteLine("Exiting application with received command: " + receivedCommand);
        MacroFramework.Macros.Stop();
    }

    [TextActivator("print [A-Z]+", TextActivatorAttribute.MatchType.RegexPattern)]
    private void PrintParameter(string command) {
        string param = command.Split(' ')[1];
        Console.WriteLine(param);
    }

    public override void OnTextCommand(string command, bool commandWasAccepted) {
        if (commandWasAccepted) {
            Console.WriteLine("The command '" + command + "' was executed by at least 1 text activator");
        } else {
            Console.WriteLine("The command '" + command + "' was not executed by any text activator");
        }
    }

    private static void SomeOtherCode() {
        TextCommands.Execute("test command");
    }
}
...

// Execute from anywhere in code
TextCommands.Execute("test command");
~~~

## Bind hold activator

The bind hold activator can be used to get Start, Update and End events for some bind. 

~~~{.cs}
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

~~~

## Repeat activator

A repeat activator can be used for example for double or triple clicks. It will activate it its child activator has activated certain amount of times within some timespan.

It also has a delegate for OnEachActivate. This can be useful when you want to initialize the command before activation (because the activation happens after a timeout).

~~~{.cs}
using MacroFramework.Commands;
using System;

namespace Examples.ActivatorExamples {
    public class RepeatActivatorExample : Command{

        protected override void InitializeActivators(ref ActivatorContainer acts) {
            BindActivator clickActivator = new BindActivator(new Bind(KKey.MouseLeft));

            new RepeatActivator(clickActivator, SingleClick) { RepeatCount = 1 }.AssignTo(acts);
            new RepeatActivator(clickActivator, DoubleClick) { RepeatCount = 2 }.AssignTo(acts);
            new RepeatActivator(clickActivator, TripleClick) { RepeatCount = 3 }.AssignTo(acts);

            new RepeatActivator(clickActivator, DoubleAndTripleClick) { RepeatCount = 2, DisallowExtraRepeats = false, OnEachActivate = OnEachClick }.AssignTo(acts);
        }



        private void SingleClick() {
            Console.WriteLine("Single click!");
        }
        private void DoubleClick() {
            Console.WriteLine("Double click!");
        }
        private void TripleClick() {
            Console.WriteLine("Triple click!");
        }

        private void DoubleAndTripleClick() {
            Console.WriteLine("Double and triple click (and quadruple click and so on)!");
        }
        private void OnEachClick() {
            Console.WriteLine("I will be called on each individual click");
        }
    }
}

~~~

## Coroutines

The `Command` class has support for coroutines (very similar to Unity game engine coroutines). Coroutines can be used to execute some tasks over a period of time and it is a alternate to async/await. It also guarantees correct execution order.

~~~{.cs}
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

~~~

## Dynamic activators

All of the above ways of using activators require you to define them before starting up the framework. The activators can created and added during runtime as well. Each `CommandActivator` instance has methods `WaitForActivation` and `RegisterDynamicActivator` which can be used to for example create some kind of confirmation for a command.

~~~{.cs}
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
        CommandActivator onSpace = new KeyActivator(KKey.Space, OneTimeOnPressSpace);

        // One time bind for space, lambda expression indicates that the activator is discarded after execution
        IDynamicActivator dynamicOnSpace = onSpace.RegisterDynamicActivator(true);

        // Identical functionality, different syntax
        dynamicOnSpace = CommandContainer.AddDynamicActivator(new DynamicActivator(onSpace, true));

        // Can be cancelled manualyl
        CommandContainer.RemoveDynamicActivator(dynamicOnSpace);

        // Can use delegate to decide when to remove
        IDynamicActivator dynamic = new DynamicActivator(onSpace, RemoveActivatorAfterExecute);
    }
    private bool RemoveActivatorAfterExecute() {
        return true;
    }

    private void OneTimeOnPressSpace(IInputEvent e) {
        Console.WriteLine("Pressed space!");
    }
}

~~~