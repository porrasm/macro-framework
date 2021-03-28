namespace MacroFramework.Commands {
    /// <summary>
    /// A settings class used to define the behaviour of a <see cref="Bind"/>
    /// </summary>
    public class BindSettings {
        #region fields
        /// <summary>
        /// The key event activation filter for this activator
        /// </summary>
        public ActivationEventType ActivationType { get; set; }
        /// <summary>
        /// The key match filter for this activator
        /// </summary>
        public KeyMatchType MatchType { get; set; }
        /// <summary>
        /// The key order filter for this activator
        /// </summary>
        public KeyPressOrder Order { get; set; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="Bind"/> instance
        /// </summary>
        /// <param name="activationType">The key event activation filter for this activator</param>
        /// <param name="matchType">The key match filter for this activator</param>
        /// <param name="order">The key order filter for this activator</param>
        public BindSettings(ActivationEventType activationType = ActivationEventType.OnAnyRelease, KeyMatchType matchType = KeyMatchType.ExactKeyMatch, KeyPressOrder order = KeyPressOrder.Ordered) {
            ActivationType = activationType;
            MatchType = matchType;
            Order = order;
        }

        #region presets

        /// <summary>
        /// The default settings for a bind. Sufficient for the majority of binds. Only the defined keys can be pressed and in order. Activated when the last key of the bind is released. 
        /// </summary>
        public static BindSettings Default = new BindSettings(ActivationEventType.OnFirstRelease, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered);

        /// <summary>
        /// Only the defined keys can be pressed in any order. Activated when the last key of the bind is released. 
        /// </summary>
        public static BindSettings OnFirstReleaseUnordered = new BindSettings(ActivationEventType.OnFirstRelease, KeyMatchType.ExactKeyMatch, KeyPressOrder.Unordered);

        /// <summary>
        /// Only the defined keys can be pressed and in order. Activated when the last key of the bind is pressed down.
        /// </summary>
        public static BindSettings OnPress = new BindSettings(ActivationEventType.OnPress, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered);

        /// <summary>
        /// Only the defined keys can be pressed and in order. Activated when the last key of the bind is pressed down.
        /// </summary>
        public static BindSettings OnPressUnordered = new BindSettings(ActivationEventType.OnPress, KeyMatchType.ExactKeyMatch, KeyPressOrder.Unordered);
        #endregion
    }
}
