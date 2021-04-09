# Windows Macro Framework

## Links

- [Nuget](https://www.nuget.org/packages/MacroFramework/)
- [Documentation](https://porrasm.github.io/windows-macro-framework-library/html/index.html) up to date with the most recent Nuget relase (0.1.8)
- [Getting started](https://porrasm.github.io/windows-macro-framework-library/html/md_markdown_getting_started.html)

## Quick setup

### 1 Install package via nuget: 

`Install-Package MacroFramework`


### 2 Setup settigns and preferences

Setting up is simple. Use the `MacroSetup.GetDefaultSetup()` as your template (or don't change it at all).

```C#
MacroSetup setup = MacroSetup.GetDefaultSetup();

setup.Settings.GeneralBindKey = KKey.CapsLock;
setup.Settings.CommandKey = KKey.LWin;
setup.Settings.MainLoopTimestep = 15;

setup.Settings.AllowKeyboardHook = true;
setup.Settings.AllowMouseHook = true;
```

#### 2.1 Reflection

The framework uses reflection to automatically parse classes you create in a given assembly definition. If you want to disable this feature, simply set `setup.CommandAssembly = null;`.

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

If you disabled loading via reflection you need to activate the command manually using:

```C#
setup.CommandToUse = new HashSet<Type>();
setup.CommandsToUse.Add(typeof(NotepadCommand));
```

### 4 Start the Macro framework on a STA thread

Start the Macro framework with your custom setup class as a paremeter.

```C#
public class Program {
    [STAThread]
    static void Main(string[] args) {
        Macros.Start(MacroSetup.GetDefaultSetup());
    }
}
```

You have now started the application with a single macro. Ctrl+N will open the notepad for you!

### 5 Read the 'Geting started' guide

For more in depth guide read the [Getting started](https://porrasm.github.io/windows-macro-framework-library/html/md_markdown_getting_started.html) guide. Check out the [code examples](https://github.com/porrasm/macro-framework/tree/main/Examples/CommandExamples) for more more complex use cases.
