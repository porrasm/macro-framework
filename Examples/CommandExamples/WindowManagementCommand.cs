using MacroFramework.Commands;

/// https://www.nuget.org/packages/WinUtilities/
using WinUtilities;

/// <summary>
/// Maximizes, minimizes or closes a window based on an activator event
/// </summary>
public class WindomManagementCommands : Command {

    private Window currentWindow = Window.FromMouse;

    protected override void InitializeActivators(out CommandActivatorGroup activator) {
        base.InitializeActivators(out activator);

        // Maximize window on Ctrl + double click
        activator.AddActivator(new RepeatActivator(new BindActivator(Keys(KKey.LCtrl, KKey.MouseLeft)), MaximizeWindow) { RepeatCount = 2, OnEachActivate = SetCurrentWindow });

        // Minimize window on Ctrl + click
        activator.AddActivator(new RepeatActivator(new BindActivator(Keys(KKey.LCtrl, KKey.MouseRight)), MinimizeWindow) { RepeatCount = 1, OnEachActivate = SetCurrentWindow });

        // Close window on Ctrl + double right click
        activator.AddActivator(new RepeatActivator(new BindActivator(Keys(KKey.LCtrl, KKey.MouseMiddle)), CloseWindow) { RepeatCount = 2, OnEachActivate = SetCurrentWindow });
    }

    /// <summary>
    /// RepeatActivator is activated with slight delay so we need to set the current window at the right time i.e. when Ctrl+Click happens not after the repeat activator timeout. 
    /// </summary>
    private void SetCurrentWindow() {
        currentWindow = Window.FromMouse;
    }

    /// <summary>
    /// Check that the current window is a valid window and that the current window is not the Windows desktop
    /// </summary>
    private bool IsInvalidWindow() {
        return !currentWindow.IsValid || WinGroup.Desktop.Match(currentWindow);
    }

    private void MaximizeWindow() {
        if (IsInvalidWindow()) {
            return;
        }

        if (currentWindow.IsMaximized) {
            currentWindow.Restore();
        } else {
            currentWindow.Maximize();
        }
    }

    private void MinimizeWindow() {
        if (IsInvalidWindow()) {
            return;
        }

        if (currentWindow.IsMinimized) {
            currentWindow.Restore();
        } else {
            currentWindow.Minimize();
        }
    }

    private void CloseWindow() {
        if (IsInvalidWindow()) {
            return;
        }

        currentWindow.Close();
    }
}
