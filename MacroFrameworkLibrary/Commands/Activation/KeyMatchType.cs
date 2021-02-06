using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework.Commands {
    /// <summary>
    /// Determines how keypress combinations are checked for matches
    /// </summary>
    public enum KeyMatchType {
        /// <summary>
        /// Match occurs if all of the selected keys and no other keys are pressed
        /// </summary>
        ExactKeyMatch,
        /// <summary>
        /// Match occurs if all of the selected keys are pressed
        /// </summary>
        PartialMatch
    }
}
