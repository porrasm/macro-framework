using MacroFramework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.ActivatorExamples {
    public class RepeatActivatorExample : Command{

        protected override void InitializeActivators(ref ActivatorContainer acts) {
            BindActivator clickActivator = new BindActivator(new Bind(KKey.MouseLeft));

            new RepeatActivator(clickActivator, SingleClick) { RepeatCount = 1 }.AssignTo(acts);
            new RepeatActivator(clickActivator, DoubleClick) { RepeatCount = 2 }.AssignTo(acts);
            new RepeatActivator(clickActivator, TripleClick) { RepeatCount = 3 }.AssignTo(acts);

            new RepeatActivator(clickActivator, DoubleAndTripleClick) { RepeatCount = 2, DisallowExtraRepeats = false, OnEachActivate = OnEachClick }.AssignTo(acts);
        }



        private void SingleClick() {
            Console.WriteLine("Single click!");
        }
        private void DoubleClick() {
            Console.WriteLine("Double click!");
        }
        private void TripleClick() {
            Console.WriteLine("Triple click!");
        }

        private void DoubleAndTripleClick() {
            Console.WriteLine("Double and triple click (and quadruple click and so on)!");
        }
        private void OnEachClick() {
            Console.WriteLine("I will be called on each individual click");
        }
    }
}
