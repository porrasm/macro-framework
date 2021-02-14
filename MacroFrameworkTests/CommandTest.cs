using MacroFramework;
using MacroFramework.Commands;
using MacroFramework.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace MacroFrameworkTests {
    [TestClass]
    public class CommandTest {

        private static Thread t;
        private static CommandTestCommandClass command;

        private static KKey bindKey = KKey.CapsLock;
        private static KKey commandKey = KKey.LWin;
        private static KKey commandActivateKey = KKey.Enter;
        private static int textCommandTimeout = 100;
        private static int mainLoopTimeStep = 15;

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

        class TestSetup : Setup {
            protected override MacroSettings GetSettings() {
                MacroSettings settings = new MacroSettings();

                settings.AutoEnableKeyboardHook = false;
                settings.AutoEnableMouseHook = false;
                settings.GeneralBindKey = bindKey;
                settings.CommandKey = commandKey;
                settings.CommandActivateKey = commandActivateKey;
                settings.TextCommandTimeout = textCommandTimeout;
                settings.MainLoopTimestep = mainLoopTimeStep;

                return settings;
            }

            protected override List<Command> GetActiveCommands() {
                var commands = new List<Command>();
                commands.Add(command = new CommandTestCommandClass());
                return commands;
            }
        }

        [ClassCleanup]
        public static void Cleanup() {
            Macros.Stop();
            t.Abort();
        }
        #endregion

        #region tests
        [TestMethod]
        public void A_TimerTest() {

            Assert.AreEqual(1, command.timerActivatorOnStartCount);
            Assert.AreEqual(1, command.timerActivatorOnStartCountAttr);
            Assert.AreEqual(0, command.timerActivatorNotOnStartCount);
            Assert.AreEqual(0, command.timerActivatorNotOnStartCountAttr);

            Thread.Sleep(600);

            Assert.AreEqual(2, command.timerActivatorOnStartCount);
            Assert.AreEqual(2, command.timerActivatorOnStartCountAttr);
            Assert.AreEqual(1, command.timerActivatorNotOnStartCount);
            Assert.AreEqual(1, command.timerActivatorNotOnStartCountAttr);
        }

        [TestMethod]
        public void B_TextCommandtest() {
            Assert.AreEqual(0, command.commandASDRan);
            Assert.AreEqual(0, command.commandASDRan2);
            Assert.AreEqual(0, command.commandASDRanAttr);
            Assert.AreEqual(0, command.commandASDRan2Attr);

            TextCommands.Execute("asd");
            MainLoopTimeout();

            Assert.AreEqual(1, command.commandASDRan);
            Assert.AreEqual(0, command.commandASDRan2);
            Assert.AreEqual(1, command.commandASDRanAttr);
            Assert.AreEqual(0, command.commandASDRan2Attr);

            TextCommands.Execute("asd2");
            MainLoopTimeout();

            Assert.AreEqual(1, command.commandASDRan);
            Assert.AreEqual(1, command.commandASDRan2);
            Assert.AreEqual(1, command.commandASDRanAttr);
            Assert.AreEqual(1, command.commandASDRan2Attr);

            TextCommands.Execute("asd", true);
            TextCommands.Execute("asd2", true);

            Assert.AreEqual(2, command.commandASDRan);
            Assert.AreEqual(2, command.commandASDRan2);
            Assert.AreEqual(2, command.commandASDRanAttr);
            Assert.AreEqual(2, command.commandASDRan2Attr);

            KeyPress(commandKey);
            KeyPress(KKey.A);
            KeyPress(KKey.S);
            KeyPress(KKey.D);
            KeyPress(commandActivateKey);
            MainLoopTimeout();

            Assert.AreEqual(3, command.commandASDRan);
            Assert.AreEqual(2, command.commandASDRan2);
            Assert.AreEqual(3, command.commandASDRanAttr);
            Assert.AreEqual(2, command.commandASDRan2Attr);

            KeyPress(commandKey);
            KeyPress(KKey.A);
            KeyPress(KKey.S);
            KeyPress(KKey.D);
            KeyPress(KKey.D2);
            KeyPress(commandActivateKey);
            MainLoopTimeout();

            Assert.AreEqual(3, command.commandASDRan);
            Assert.AreEqual(3, command.commandASDRan2);
            Assert.AreEqual(3, command.commandASDRanAttr);
            Assert.AreEqual(3, command.commandASDRan2Attr);

            KeyPress(commandKey);
            KeyPress(KKey.A);
            KeyPress(KKey.S);
            Thread.Sleep(textCommandTimeout * 2);
            KeyPress(KKey.D);
            KeyPress(commandActivateKey);
            MainLoopTimeout();

            Assert.AreEqual(3, command.commandASDRan);
            Assert.AreEqual(3, command.commandASDRan2);
            Assert.AreEqual(3, command.commandASDRanAttr);
            Assert.AreEqual(3, command.commandASDRan2Attr);


            KeyPress(commandKey);
            KeyPress(KKey.A);
            KeyPress(KKey.S);
            KeyPress(KKey.D);
            KeyPress(KKey.Escape);
            KeyPress(commandActivateKey);
            MainLoopTimeout();

            Assert.AreEqual(3, command.commandASDRan);
            Assert.AreEqual(3, command.commandASDRan2);
            Assert.AreEqual(3, command.commandASDRanAttr);
            Assert.AreEqual(3, command.commandASDRan2Attr);
        }

        [TestMethod]
        public void C_TextCommandRegexTest() {

            Assert.AreEqual(0, command.regex1Val);
            Assert.AreEqual(0, command.regex2Val);

            TextCommands.Execute("regex", true);

            Assert.AreEqual(0, command.regex1Val);
            Assert.AreEqual(0, command.regex2Val);

            TextCommands.Execute("regex ", true);

            Assert.AreEqual(0, command.regex1Val);
            Assert.AreEqual(0, command.regex2Val);

            TextCommands.Execute("regex asd", true);

            Assert.AreEqual(1, command.regex1Val);
            Assert.AreEqual(1, command.regex2Val);

            TextCommands.Execute("regex asd asd", true);

            Assert.AreEqual(1, command.regex1Val);
            Assert.AreEqual(1, command.regex2Val);
        }

        [TestMethod]
        public void D_KeyActivatorTest() {
            Assert.AreEqual(0, command.keyKDownValue);
            Assert.AreEqual(0, command.keyKDownValueAttr);
            Assert.AreEqual(0, command.keyKUpValue);
            Assert.AreEqual(0, command.keyKUpValueAttr);

            KeyPress(KKey.A);
            MainLoopTimeout();

            Assert.AreEqual(0, command.keyKDownValue);
            Assert.AreEqual(0, command.keyKDownValueAttr);
            Assert.AreEqual(0, command.keyKUpValue);
            Assert.AreEqual(0, command.keyKUpValueAttr);

            KeyPress(KKey.K);
            MainLoopTimeout();

            Assert.AreEqual(1, command.keyKDownValue);
            Assert.AreEqual(1, command.keyKDownValueAttr);
            Assert.AreEqual(1, command.keyKUpValue);
            Assert.AreEqual(1, command.keyKUpValueAttr);

            KeyDown(KKey.K);
            MainLoopTimeout();

            Assert.AreEqual(2, command.keyKDownValue);
            Assert.AreEqual(2, command.keyKDownValueAttr);
            Assert.AreEqual(1, command.keyKUpValue);
            Assert.AreEqual(1, command.keyKUpValueAttr);

            KeyUp(KKey.K);
            MainLoopTimeout();

            Assert.AreEqual(2, command.keyKDownValue);
            Assert.AreEqual(2, command.keyKDownValueAttr);
            Assert.AreEqual(2, command.keyKUpValue);
            Assert.AreEqual(2, command.keyKUpValueAttr);
        }

        [TestMethod]
        public void E_BindActivatorAttributeTest() {
            Assert.AreEqual(0, command.bindAttributeActivateTest1);
            Assert.AreEqual(0, command.bindAttributeActivateTest2);

            KeyDown(KKey.GeneralBindKey);
            KeyDown(KKey.D1);
            MainLoopTimeout();

            Assert.AreEqual(1, command.bindAttributeActivateTest1);
            Assert.AreEqual(0, command.bindAttributeActivateTest2);

            KeyUp(KKey.GeneralBindKey);
            KeyUp(KKey.D1);
            MainLoopTimeout();

            Assert.AreEqual(1, command.bindAttributeActivateTest1);
            Assert.AreEqual(1, command.bindAttributeActivateTest2);


            KeyDown(KKey.D1);
            KeyDown(KKey.GeneralBindKey);
            MainLoopTimeout();

            Assert.AreEqual(1, command.bindAttributeActivateTest1);
            Assert.AreEqual(1, command.bindAttributeActivateTest2);

            KeyUp(KKey.GeneralBindKey);
            KeyUp(KKey.D1);
            MainLoopTimeout();

            Assert.AreEqual(1, command.bindAttributeActivateTest1);
            Assert.AreEqual(1, command.bindAttributeActivateTest2);
        }

        [TestMethod]
        public void F_BindActivatorExtensiveTest() {

            // ActivationEvenType any
            SetBindActivator(ActivationEventType.Any, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);

            ExpectBind(1);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);

            ExpectBind(2);

            // ActivationEventType on press
            SetBindActivator(ActivationEventType.OnPress, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);

            ExpectBind(1);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);

            ExpectBind(1);

            // ActivationEventType on first release
            SetBindActivator(ActivationEventType.OnFirstRelease, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);

            ExpectBind(0);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);

            ExpectBind(1);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);
            KeyDown(KKey.F4);

            ExpectBind(1);

            KeyUp(KKey.F4);
            KeyUp(KKey.F3);
            KeyUp(KKey.F2);
            KeyUp(KKey.F1);


            ExpectBind(1);

            // ActivatrionEventType on any release
            SetBindActivator(ActivationEventType.OnAnyRelease, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);

            ExpectBind(0);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);

            ExpectBind(1);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);
            KeyDown(KKey.F4);

            ExpectBind(1);

            KeyUp(KKey.F4);
            KeyUp(KKey.F3);
            KeyUp(KKey.F2);
            KeyUp(KKey.F1);


            ExpectBind(2);

            // MatchType exact
            SetBindActivator(ActivationEventType.OnFirstRelease, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);

            ExpectBind(0);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);

            ExpectBind(1);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);
            KeyDown(KKey.F4);

            ExpectBind(1);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);
            KeyUp(KKey.F4);

            ExpectBind(1);

            // MatchType partial
            SetBindActivator(ActivationEventType.OnFirstRelease, KeyMatchType.PartialMatch, KeyPressOrder.Ordered);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);

            ExpectBind(0);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);

            ExpectBind(1);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);
            KeyDown(KKey.F4);

            ExpectBind(1);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);
            KeyUp(KKey.F4);

            ExpectBind(2);

            // Order ordered
            SetBindActivator(ActivationEventType.OnFirstRelease, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);

            ExpectBind(0);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);

            ExpectBind(1);

            KeyDown(KKey.F3);
            KeyDown(KKey.F2);
            KeyDown(KKey.F1);

            ExpectBind(1);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);

            ExpectBind(1);

            // Order unordered
            SetBindActivator(ActivationEventType.OnFirstRelease, KeyMatchType.ExactKeyMatch, KeyPressOrder.Unordered);

            KeyDown(KKey.F1);
            KeyDown(KKey.F2);
            KeyDown(KKey.F3);

            ExpectBind(0);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);

            ExpectBind(1);

            KeyDown(KKey.F3);
            KeyDown(KKey.F2);
            KeyDown(KKey.F1);

            ExpectBind(1);

            KeyUp(KKey.F1);
            KeyUp(KKey.F2);
            KeyUp(KKey.F3);

            ExpectBind(2);
        }
        private void SetBindActivator(ActivationEventType e, KeyMatchType m, KeyPressOrder o) {
            command.bindActivatorValue = 0;
            command.bindActivator.ActivationType = e;
            command.bindActivator.MatchType = m;
            command.bindActivator.Order = o;
        }
        private void ExpectBind(int val) {
            MainLoopTimeout();
            Assert.AreEqual(val, command.bindActivatorValue);
        }
        #endregion

        private static IInputEvent DummyInput(KKey k, bool state) {
            KeyEvent e = new KeyEvent();
            e.ActivationType = KeyStates.GetCurrentActivationEventType(state);
            e.Key = k;
            e.State = state;
            e.ReceiveTimestamp = MacroFramework.Tools.Timer.Milliseconds;
            e.Unique = KeyStates.IsUniqueEvent(k, state);
            return e;
        }

        private static void KeyPress(KKey k) {
            KeyDown(k);
            KeyUp(k);
        }
        private static void KeyDown(KKey k) {
            InputEvents.RegisterHookKeyEvent(DummyInput(k, true));
        }
        private static void KeyUp(KKey k) {
            InputEvents.RegisterHookKeyEvent(DummyInput(k, false));
        }

        private static void MainLoopTimeout() {
            Thread.Sleep(mainLoopTimeStep * 3);
        }
    }

    public class CommandTestCommandClass : Command {

        public bool OnStartRan, OnCloseRan, OnExecuteStartRan, OnExecutionCompleteRan, InitializeRan;

        public int timerActivatorOnStartCount, timerActivatorNotOnStartCount;
        public int timerActivatorOnStartCountAttr, timerActivatorNotOnStartCountAttr;

        public override void OnStart() {
            OnStartRan = true;
        }
        protected override void OnExecuteStart() {
            OnExecuteStartRan = true;
        }
        protected override void OnExecutionComplete() {
            OnExecutionCompleteRan = true;
        }
        public override void OnClose() {
            OnCloseRan = true;
        }

        protected override void InitializeActivators(out CommandActivatorGroup activator) {
            activator = new CommandActivatorGroup(this);
            InitializeRan = true;
            activator.AddActivator(new TimerActivator(TimerActivatorTestOnStartCount, 500, TimeUnit.Milliseconds, true));
            activator.AddActivator(new TimerActivator(TimerActivatorTestNotOnStartCount, 500, TimeUnit.Milliseconds, false));


            activator.AddActivator(new TextActivator(TextCommandASDRan, "asd"));
            activator.AddActivator(new TextActivator(TextCommandASDRan2, "asd2"));

            activator.AddActivator(new TextActivator(RegexTest1, new Regex("regex [a-z]+$")));

            activator.AddActivator(new KeyActivator(KeyEventForK, KKey.K));

            bindActivator = new BindActivator(BindActivatorTest, Keys(KKey.F1, KKey.F2, KKey.F3));
            activator.AddActivator(bindActivator);
        }

        #region tests
        private void TimerActivatorTestOnStartCount() {
            timerActivatorOnStartCount++;
        }
        private void TimerActivatorTestNotOnStartCount() {
            timerActivatorNotOnStartCount++;
        }

        [TimerActivator(500, TimeUnit.Milliseconds, true)]
        private void TimerActivatorTestOnStartCountAttr() {
            timerActivatorOnStartCountAttr++;
        }
        [TimerActivator(500, TimeUnit.Milliseconds, false)]
        private void TimerActivatorTestNotOnStartCountAttr() {
            timerActivatorNotOnStartCountAttr++;
        }


        public int commandASDRan;
        private void TextCommandASDRan() {
            commandASDRan++;
        }

        public int commandASDRan2;
        private void TextCommandASDRan2(string c) {
            commandASDRan2++;
        }

        public int commandASDRanAttr;
        [TextActivator("asd", TextActivatorAttribute.MatchType.StringMatch)]
        private void TextCommandASDRanAttr() {
            commandASDRanAttr++;
        }

        public int commandASDRan2Attr;
        [TextActivator("asd2", TextActivatorAttribute.MatchType.StringMatch)]
        private void TextCommandASDRan2Attr() {
            commandASDRan2Attr++;
        }

        public int regex1Val;
        private void RegexTest1() {
            regex1Val++;
        }

        public int regex2Val;
        [TextActivator("regex [a-z]+$", TextActivatorAttribute.MatchType.RegexPattern)]
        private void RegexTest2() {
            regex2Val++;
        }

        public int keyKDownValue, keyKUpValue;
        private void KeyEventForK(IInputEvent e) {
            if (e.Key != KKey.K) {
                throw new Exception("Invalid key");
            }
            if (e.State) {
                keyKDownValue++;
            } else {
                keyKUpValue++;
            }
        }

        public int keyKDownValueAttr, keyKUpValueAttr;
        [KeyActivator(KKey.K)]
        private void KeyEventForKAttr() {
            IInputEvent e = InputEvents.CurrentInputEvent;
            if (e.Key != KKey.K) {
                throw new Exception("Invalid key");
            }
            if (e.State) {
                keyKDownValueAttr++;
            } else {
                keyKUpValueAttr++;
            }
        }

        #region binds

        public int bindAttributeActivateTest1, bindAttributeActivateTest2;
        [BindActivator(ActivationEventType.OnPress, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered, KKey.GeneralBindKey, KKey.D1)]
        private void BindAttr1() {
            bindAttributeActivateTest1++;
        }
        [BindActivator(ActivationEventType.OnFirstRelease, KeyMatchType.ExactKeyMatch, KeyPressOrder.Ordered, KKey.GeneralBindKey, KKey.D1)]
        private void BindAttr2() {
            bindAttributeActivateTest2++;
        }

        public BindActivator bindActivator;
        public int bindActivatorValue;
        private void BindActivatorTest() {
            bindActivatorValue++;
        }
        #endregion
        #endregion
    }
}
