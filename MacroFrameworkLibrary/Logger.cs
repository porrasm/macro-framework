using System;

namespace MacroFramework {
    /// <summary>
    /// Default logger implementation
    /// </summary>
    public class Logger : ILogger {
        internal static ILogger Instance { get; set; }
        public void LogMessage(string s) {
            Console.WriteLine(s);
        }
        public void LogException(Exception e, string additionalMessage = "") {
            Console.WriteLine($"{additionalMessage}\nError message: {e.Message}\nError: {e}");
        }

        public static void Log(string s) {
            Instance?.LogMessage(s);
        }
        public static void Exception(Exception e, string additionalMessage = "") {
            Instance?.LogException(e, additionalMessage);
        }
    }
}
