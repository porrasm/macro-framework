# Windows Macro Framework

## Quick setup

The package is available on [Nuget](https://www.nuget.org/packages/MacroFramework/)

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

This command will open Notepad when 

```C#
using MacroFramework.Commands;

public class NotepadCommand : Command {
        [BindActivator(ActivationEventType.OnPress, true, VKey.LCONTROL, VKey.N)]
        private void OpenNotepad() {
            System.Diagnostics.Process.Start("Notepad.exe");
        }
    }
```

### 4 Start the Macro framework on a STA thread

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