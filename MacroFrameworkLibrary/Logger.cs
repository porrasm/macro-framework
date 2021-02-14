using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework {
    internal class Logger : ILogger {
        internal static ILogger Instance { get; set; }
        public void LogMessage(string s) {
            Console.WriteLine(s);
        }
        public static void Log(string s) {
            Instance?.LogMessage(s);
        }
    }
}
