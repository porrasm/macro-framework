using MacroFramework.Commands;
using System;
using System.Text.RegularExpressions;

class TextActivatorExample : Command {

    // Alternate to using attributes
    protected override void InitializeActivators(out CommandActivatorGroup activator) {
        activator = new CommandActivatorGroup(this);

        activator.Add(new TextActivator(OnTestCommand, "test command"));

        // Multiple matchers
        activator.Add(new TextActivator(ExitApplication, "stop", "exit", "quit"));

        // Regex
        activator.Add(new TextActivator(PrintParameter, new Regex("print [A-Z]+")));
    }

    [TextActivator("test command", TextActivatorAttribute.MatchType.StringMatch)]
    private void OnTestCommand() {
        Console.WriteLine("Test command executed!");
    }


    private void ExitApplication(string receivedCommand) {
        Console.WriteLine("Exiting application with received command: " + receivedCommand);
        MacroFramework.Macros.Stop();
    }

    [TextActivator("print [A-Z]+", TextActivatorAttribute.MatchType.RegexPattern)]
    private void PrintParameter(string command) {
        string param = command.Split(' ')[1];
        Console.WriteLine(param);
    }

    public override void OnTextCommand(string command, bool commandWasAccepted) {
        if (commandWasAccepted) {
            Console.WriteLine("The command '" + command + "' was executed by at least 1 text activator");
        } else {
            Console.WriteLine("The command '" + command + "' was not executed by any text activator");
        }
    }

    private static void SomeOtherCode() {
        TextCommands.Execute("test command");
    }
}
