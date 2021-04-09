using MacroFramework;
using MacroFramework.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MacroFrameworkTests {
    [TestClass]
    public class ExceptionTests {

        static Command command;
        static Thread t;

        #region initialize and cleanup
        [ClassInitialize]
        public static void Initialize(TestContext c) {
            t = new Thread(ProgramMain);
            t.Start();
            // Give time for app to start
            Thread.Sleep(100);
        }

        [STAThread]
        private static void ProgramMain() {
            Macros.Start(new TestSetup());
        }

        class TestSetup : MacroSetup {
            protected override MacroSettings GetSettings() {
                MacroSettings settings = new MacroSettings();

                settings.AllowKeyboardHook = false;
                settings.AllowMouseHook = false;

                return settings;
            }

            protected override List<Command> GetActiveCommands() {
                var commands = new List<Command>();
                commands.Add(command = new ExceptionClass());
                return commands;
            }
        }

        [ClassCleanup]
        public static void Cleanup() {
            Macros.Stop();
            t.Abort();
        }
        #endregion

        [TestMethod]
        public void Test() {
            Thread.Sleep(1000);
        }

        public class ExceptionClass : Command {

            protected override void InitializeActivators(out CommandActivatorGroup activator) {
                base.InitializeActivators(out activator);
            }

            public override void OnStart() {
                A();
                throw new Exception("e");
            }

            private async Task A() {
                await Task.Delay(100);
                throw new Exception("e");
            }

            public override void OnTextCommand(string command, bool commandWasAccepted) {
                throw new Exception("e");
            }

            [TimerActivator(1, TimeUnit.Seconds, true)]
            private void Timer() {
                throw new Exception("e");
            }
        }
    }
}
