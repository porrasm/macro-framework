namespace MacroFramework.Commands.Coroutines {
    /// <summary>
    /// The different update groups that a coroutine has
    /// </summary>
    public enum CoroutineUpdateGroup {
        /// <summary>
        /// Handled at the start of each main loop iteration right after <see cref="Macros.OnMainLoop"/> delegate
        /// </summary>
        OnBeforeUpdate = 0,
        /// <summary>
        /// Handled at the end of each main loop iteration
        /// </summary>
        OnAfterUpdate = 1,
        /// <summary>
        /// Handled at each update loop
        /// </summary>
        OnTimerUpdate = 2,
        /// <summary>
        /// Handled whenever a received <see cref="IInputEvent"/> is handled, after corresponding <see cref="IActivator"/> instances
        /// </summary>
        OnInputEvent = 3
    }
}
