# Windows Macro Framework

## Links

- [Nuget](https://www.nuget.org/packages/MacroFramework/)
- [Documentation](https://porrasm.github.io/windows-macro-framework-library/html/index.html) up to date with the most recent Nuget relase (0.1.4)
- [Getting started](https://porrasm.github.io/windows-macro-framework-library/html/md_markdown_getting_started.html)

## Quick setup

### 1 Install package via nuget: 

`Install-Package MacroFramework`

### 2 Setup library

You only need to setup 1 thing for this library. Inherit the `Setup` class and it's abstract methods.

```C#
using MacroFramework;

public class MySetup : Setup {
    protected override MacroSettings GetSettings() {
        MacroSettings settings = new MacroSettings();

        settings.GeneralBindKey = KKey.CapsLock;
        settings.CommandKey = KKey.LWin;

        settings.MainLoopTimestep = 15;

        return settings;
    }

    protected override Assembly GetMainAssembly() {
        return Assembly.GetExecutingAssembly();
    }
}
```

### 3 Create a macro

This command will open Notepad when you press 'left control' and 'n' in order and release the keys.

```C#
using MacroFramework.Commands;

public class NotepadCommand : Command {
    [BindActivator(KKey.LCtrl, KKey.N)]
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
public class QuickSetupCodeExample {
    [STAThread]
    static void Main(string[] args) {
        MacroFramework.Macros.Start(new MySetup());
    }
}
```

You have now started the application with a single macros. Ctrl+N will open the notepad for you!

### 5 Read the 'Geting started' guide

For more in depth guide read the [Getting started](https://porrasm.github.io/windows-macro-framework-library/html/md_markdown_getting_started.html) guide.
