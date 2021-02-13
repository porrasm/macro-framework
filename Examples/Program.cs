using MacroFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Examples {
    public class Program {
        static void Main(string[] args) {
            Macros.Start(new MySetup());
        }

        public class MySetup : Setup {
            protected override MacroSettings GetSettings() {
                MacroSettings settings = new MacroSettings();

                settings.GeneralBindKey = KKey.CapsLock;
                settings.CommandKey = KKey.LWin;

                settings.MainLoopTimestep = 15;

                return settings;
            }

            protected override Assembly GetMainAssembly() {
                return Assembly.GetExecutingAssembly();
            }
        }
    }
}
