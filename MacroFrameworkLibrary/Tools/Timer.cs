using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Tools {
    /// <summary>
    /// Provides simple timer class
    /// </summary>
    public static class Timer {
        /// <summary>
        /// System time in milliseconds
        /// </summary>
        public static long Milliseconds => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        /// <summary>
        /// Elapes system time in milliseconds
        /// </summary>
        /// <param name="timestampMillis">Elapsed millis from</param>
        /// <returns></returns>
        public static long PassedFrom(long timestampMillis) {
            return Milliseconds - timestampMillis;
        }
    }
}
