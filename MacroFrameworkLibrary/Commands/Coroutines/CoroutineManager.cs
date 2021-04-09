using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroFramework.Commands.Coroutines {
    /// <summary>
    /// Class for managing coroutines
    /// </summary>
    internal class CoroutineManager {
        #region fields
        private HashSet<Coroutine> coroutines;
        private HashSet<YieldInstruction>[] groups;
        #endregion

        public CoroutineManager() {
            coroutines = new HashSet<Coroutine>();
            groups = new HashSet<YieldInstruction>[4];
            for (int i = 0; i < 4; i++) {
                groups[i] = new HashSet<YieldInstruction>();
            }
        }

        #region management
        internal void StartCoroutine(Coroutine coroutine) {
            if (coroutine.IsRunning) {
                throw new Exception("Can't add an already running coroutine. Create a new instance instad.");
            }
            if (!coroutines.Add(coroutine)) {
                throw new Exception("Coroutine already running");
            }

            coroutine.IsRunning = true;
            coroutine.MoveNext();
            int groupIndex = (int)coroutine.CurrentInstruction.UpdateGroup;
            groups[groupIndex].Add(coroutine.CurrentInstruction);
        }

        internal bool StopCoroutine(Coroutine coroutine, bool finished) {
            if (!coroutines.Remove(coroutine)) {
                return false;
            }

            coroutine.Finish();
            coroutine.IsRunning = false;

            if (!finished) {
                groups[(int)coroutine.CurrentInstruction.UpdateGroup].Remove(coroutine.CurrentInstruction);
            }

            return true;
        }

        internal void StopAllCoroutines() {
            Coroutine[] allCoroutines = coroutines.ToArray();
            Array.ForEach(allCoroutines, c => StopCoroutine(c, false));
        }

        internal void UpdateCoroutines(CoroutineUpdateGroup group) {
            int groupIndex = (int)group;
            foreach (YieldInstruction instruction in groups[groupIndex].ToArray()) {
                UpdateCoroutine(groupIndex, instruction);
            }
        }

        private void UpdateCoroutine(int groupIndex, YieldInstruction instruction) {

            if (instruction.Owner.IsPaused) {
                return;
            }

            if (instruction.TimeoutExceeded()) {
                if (!StopCoroutine(instruction.Owner, false)) {
                    throw new Exception("Error removing coroutine after finishing it");
                }
                return;
            }

            // Check if current instruction is finished
            if (!instruction.MoveNext()) {

                groups[groupIndex].Remove(instruction);

                // Check if coroutine is finished
                if (!instruction.Owner.MoveNext()) {
                    if (!StopCoroutine(instruction.Owner, true)) {
                        throw new Exception("Error removing coroutine after finishing it");
                    }
                    return;
                }

                YieldInstruction newInstruction = instruction.Owner.CurrentInstruction;
                groups[(int)newInstruction.UpdateGroup].Add(newInstruction);
            }
        }
        #endregion
    }
}
