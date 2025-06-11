using System.Text.RegularExpressions;
using Serilog;

public static class DummyListHandler
{
    public static void Run(NotifyIcon? notifyIcon, int sleepTime, bool keepRunning)
    {
        while (keepRunning)
        {   
            Task.Delay(sleepTime).Wait();
            lock (Utils.ClipboardLock)
            {
                if (Clipboard.ContainsText())
                {
                    Log.Information("Clipboard contains text...");
                    string myText = Clipboard.GetText(); // Get the text from the clipboard
                    if (Regex.Matches(myText, @"\b[A-Z0-9]{32}\b").Count() > 1 && Regex.Matches(myText, @"\bDummy\b").Count() > 1)
                    {
                        foreach (Match match in Regex.Matches(myText, @"\b[A-Z0-9]{32}\b"))
                        {
                            Log.Information("Found UID: {UID}", match.Value);
                        }
                    }

                    Utils.ParsePipeSeparatedLines(myText);
                    Log.Information("Parsed text from clipboard: {Text}", myText);
                }
            }
        }
    }
}