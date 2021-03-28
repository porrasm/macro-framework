using MacroFramework.Commands;

/// https://www.nuget.org/packages/WinUtilities/
using WinUtilities;

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
        windowWasMaximized = window.IsMaximized;
        if (windowWasMaximized) {
            window.Restore();
        }

        // Keep window on top of everything while dragging
        window.SetAlwaysOnTop(true);

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

        window.SetAlwaysOnTop(false);
    }
    #endregion
}