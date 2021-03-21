using MacroFramework.Commands;
using MacroFramework.Tools;
using System;

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
        activator.AddActivator(new RepeatActivator(MaximizeWindow, new BindActivator(Keys(KKey.LCtrl, KKey.MouseLeft))) { RepeatCount = 2, OnEachActivate = SetCurrentWindow });

        // Minimize window on Ctrl + click
        activator.AddActivator(new RepeatActivator(MinimizeWindow, new BindActivator(Keys(KKey.LCtrl, KKey.MouseRight))) { RepeatCount = 1, OnEachActivate = SetCurrentWindow });

        // Close window on Ctrl + double right click
        activator.AddActivator(new RepeatActivator(CloseWindow, new BindActivator(Keys(KKey.LCtrl, KKey.MouseMiddle))) { RepeatCount = 2, OnEachActivate = SetCurrentWindow });
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



/// <summary>
/// Allows the user to drag the window without using the top border bar
/// </summary>
public class WindowDraggerCommand : Command {
    #region fields
    private Window window;

    private bool windowWasMaximized;

    private Coord mousePos;
    private Coord mouseOffset;
    #endregion

    protected override void InitializeActivators(out CommandActivatorGroup activator) {
        activator = new CommandActivatorGroup(this);

        // Drags a window on Ctrl+click and hold
        activator.AddActivator(new BindHoldActivator(new BindActivator(Keys(KKey.Ctrl, KKey.MouseLeft), ActivationEventType.OnPress), OnDragUpdate, OnDragStart, OnDragEnd));
    }

    /// <summary>
    /// Check that window is valid and not the Windows desktop
    /// </summary>
    private bool InvalidWindow => !window.IsValid || WinGroup.Desktop.Match(window);

    #region drag
    /// <summary>
    /// Initialize the drag
    /// </summary>
    private void OnDragStart() {
        window = Window.FromMouse;
        if (InvalidWindow) {
            return;
        }

        // Disable maximization
        if (window.IsMaximized) {
            windowWasMaximized = true;
            window.Restore();
        } else {
            windowWasMaximized = false;
        }

        // Update mouse offset
        mousePos = Mouse.Position;
        mouseOffset = window.Area.Center - Mouse.Position;
    }

    /// <summary>
    /// Perfrom drag
    /// </summary>
    private void OnDragUpdate() {
        if (InvalidWindow) {
            return;
        }
        mousePos = Mouse.Position;
        Area area = window.Area;
        area.Center = mousePos + mouseOffset;
        window.Move(area);
    }

    /// <summary>
    /// Cleanup
    /// </summary>
    private void OnDragEnd() {
        if (InvalidWindow) {
            return;
        }
        // Restore maximized state
        if (windowWasMaximized) {
            window.Maximize();
        }
    }

    
    #endregion
}
