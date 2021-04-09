using MacroFramework;
using MacroFramework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public class Program {
    [STAThread]
    static void Main(string[] args) {
        Macros.Start(MacroSetup.GetDefaultSetup());
    }

    public static MacroSetup MySetup() {
        MacroSetup setup = MacroSetup.GetDefaultSetup();

        setup.Settings.GeneralBindKey = KKey.CapsLock;
        setup.Settings.CommandKey = KKey.LWin;
        setup.Settings.MainLoopTimestep = 15;

        setup.Settings.AllowKeyboardHook = true;
        setup.Settings.AllowMouseHook = true;

        return setup;
    }
}

public class NotepadCommand : Command {
    [BindActivator(KKey.LCtrl, KKey.N)]
    private void OpenNotepad() {
        System.Diagnostics.Process.Start("Notepad.exe");
    }
}