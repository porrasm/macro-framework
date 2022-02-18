#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Text;

namespace MacroFramework {
    /// <summary>
    /// A class for executing <see cref="Action"/> and <see cref="Func{T, TResult}"/> objects in a try clause
    /// </summary>
    public static class Callbacks {

        private static void OnCallbackFail(Exception e, string errorMessage, Action failCallback) {
            Logger.Exception(e, errorMessage);
            failCallback?.Invoke();
        }

        #region action
        public static void ExecuteAction(Action action, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke();
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1>(Action<T1> action, T1 p1, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2>(Action<T1, T2> action, T1 p1, T2 p2, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3>(Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 p1, T2 p2, T3 p3, T4 p4, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6, p7);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6, p7, p8);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6, p7, p8, p9);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }

        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, string errorMessage = "", Action onFail = null) {
            try {
                action?.Invoke(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
            }
        }
        #endregion

        #region func
        public static Result ExecuteFunc<Result>(Func<Result> action, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action();
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, Result>(Func<T1, Result> action, T1 p1, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, Result>(Func<T1, T2, Result> action, T1 p1, T2 p2, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, Result>(Func<T1, T2, T3, Result> action, T1 p1, T2 p2, T3 p3, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, Result>(Func<T1, T2, T3, T4, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, Result>(Func<T1, T2, T3, T4, T5, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, Result>(Func<T1, T2, T3, T4, T5, T6, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, T7, Result>(Func<T1, T2, T3, T4, T5, T6, T7, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6, p7);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, T7, T8, Result>(Func<T1, T2, T3, T4, T5, T6, T7, T8, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6, p7, p8);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, Result>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6, p7, p8, p9);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Result>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Result>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Result>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Result>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Result>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Result>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }

        public static Result ExecuteFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Result>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Result> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, Result resultDefault = default, string errorMessage = "", Action onFail = null) {
            try {
                return action == null ? resultDefault : action(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16);
            } catch (Exception e) {
                OnCallbackFail(e, errorMessage, onFail);
                return resultDefault;
            }
        }
        #endregion
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
