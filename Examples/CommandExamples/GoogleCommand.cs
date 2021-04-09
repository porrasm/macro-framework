using MacroFramework.Commands;
using MacroFramework.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Opens a Google search when a text command 'google search_param' is executed
/// </summary>
public class GoogleCommand : Command {

    private string googleURI = "https://www.google.com/search?q=";
    private RegexWrapper googleR = Regexes.StartsWith("google");
    private RegexWrapper googleS = Regexes.StartsWith("search");

    protected override void InitializeActivators(ref ActivatorContainer acts) {
        new TextActivator(new Matchers(googleR, googleS), GoogleSearch).AssignTo(acts);
    }

    private void GoogleSearch(string command) {

        Console.WriteLine("Google searching: " + command);

        string searchParam = Regexes.GetStringParameter(command);
        if (searchParam != null) {
            Uri uri = new Uri(googleURI + searchParam);
            Process.Start(uri.ToString());
        }
    }
}
