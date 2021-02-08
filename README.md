# Windows Macro Framework

## Links

- [Documentation](https://porrasm.github.io/windows-macro-framework-library/html/index.html)
- [Nuget](https://www.nuget.org/packages/MacroFramework/)

## Warning

Project is in very early stage. Many things will change. There are bugs and prepare to refactor things after updating.

## Quick setup

### 1 Install package via nuget: 

`Install-Package MacroFramework`

### 2 Setup library

You only need to setup 1 thing for this library. Inherit the `Setup` class and it's abstract methods.

```C#
using MacroFramework;

public class MySetup : Setup {

    // Return the executing Assembly for automatic Command activation 
    // or null if you don't want this feature.
    protected override Assembly GetMainAssembly() {
        return Assembly.GetExecutingAssembly();
    }

    protected override MacroSettings GetSettings() {
        MacroSettings settings = new MacroSettings();
        // Set settings here
        return settings;
    }
}
```

### 3 Create a macro

This command will open Notepad when the 'left control' and 'n' are pressed in order.

```C#
using MacroFramework.Commands;

public class NotepadCommand : Command {
    [BindActivator(ActivationEventType.OnPress, KeyPressOrder.Ordered, VKey.LCONTROL, VKey.N)]
    private void OpenNotepad() {
        System.Diagnostics.Process.Start("Notepad.exe");
    }
}
```

If you did not setup the executing assembly you need to activate the command manually using:

```C#
using MacroFramework.Commands;
...
CommandContainer.AddCommand(new NotepadCommand());
```

### 4 Start the Macro framework on a STA thread

Start the Macro framework with your custom setup class as a paremeter.

```C#
public static class Program {
    [STAThread]
    static void Main(string[] args) {
        MacroFramework.Macros.Start(new MySetup());
    }
}
```

### 5 Read the 'Geting started' guide

--link-to-guide
