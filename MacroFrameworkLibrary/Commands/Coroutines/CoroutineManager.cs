using MacroFramework.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if (coroutine.Consumed) {
                throw new Exception("Can't add an already consumed coroutine. Create a new instance instad.");
            }
            if (!coroutines.Add(coroutine)) {
                throw new Exception("Coroutine already running");
            }

            coroutine.Consumed = true;
            coroutine.IsRunning = true;
            coroutine.MoveNext();
            int groupIndex = (int)coroutine.CurrentInstruction.UpdateGroup;
            groups[groupIndex].Add(coroutine.CurrentInstruction);
        }

        internal bool StopCoroutine(Coroutine coroutine) {
            if (!coroutines.Contains(coroutine)) {
                return false;
            }

            coroutines.Remove(coroutine);
            coroutine.Finish();
            coroutine.IsRunning = false;

            return true;
        }

        internal void StopAllCoroutines() {
            Coroutine[] allCoroutines = coroutines.ToArray();
            Array.ForEach(allCoroutines, c => StopCoroutine(c));
        }

        internal void UpdateCoroutines(CoroutineUpdateGroup group) {
            int groupIndex = (int)group;
            foreach (YieldInstruction instruction in groups[groupIndex].ToArray()) {
                UpdateCoroutine(groupIndex, instruction);
            }
        }

        private void UpdateCoroutine(int groupIndex, YieldInstruction instruction) {

            // Check if current instruction is finished
            if (!instruction.MoveNext()) {

                groups[groupIndex].Remove(instruction);

                // Check if coroutine is finished
                if (!instruction.Owner.MoveNext()) {
                    if (!StopCoroutine(instruction.Owner)) {
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
