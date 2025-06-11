using System.Text.RegularExpressions;
using Serilog;

public static class DummyListHandler
{
    public static void Run(NotifyIcon? notifyIcon, int sleepTime)
    {   
        string? lastResult = null;
        Task.Delay(sleepTime).Wait();
        lock (Utils.ClipboardLock)
        {
            if (Clipboard.ContainsText())
            {
                string myText = Clipboard.GetText();
                var matches = Regex.Matches(myText, @"\b[A-Z0-9]{32}\b");
                if (matches.Count > 1 && Regex.Matches(myText, @"\bDummy\b").Count > 1)
                {
                    var uids = matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();
                    string result = Utils.GenerateTemplateForUids(uids);

                    // Skip if clipboard already contains the generated result
                    if (myText == result || result == lastResult)
                        {
                        Log.Information("Clipboard already contains the generated template, skipping update.");
                        return;
                        }
                    Clipboard.SetText(result); // Copy the generated template to clipboard
                    lastResult = result;
                    Log.Information("Generated template copied to clipboard:\n{Result}", result);
                }
            }
        }
    }
}