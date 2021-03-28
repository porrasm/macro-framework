using System;

namespace MacroFramework {
    /// <summary>
    /// Interface for log messages
    /// </summary>
    public interface ILogger {
        void LogMessage(string s);
        void LogException(Exception e, string message);
    }
}
