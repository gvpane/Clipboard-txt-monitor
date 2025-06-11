using System.Text.RegularExpressions;
using Serilog;

public static class DummyListHandler
{
    public static void Run(NotifyIcon? notifyIcon, int sleepTime, bool keepRunning)
    {   
        Log.Information("DummyListHandler started with sleep time: {SleepTime} ms", sleepTime);
        while (keepRunning)
        {
            Task.Delay(sleepTime).Wait();
            lock (Utils.ClipboardLock)
            {
                if (Clipboard.ContainsText())
                {
                    string myText = Clipboard.GetText(); // Get the text from the clipboard
                    if (Regex.Matches(myText, @"\b[A-Z0-9]{32}\b").Count() > 1 && Regex.Matches(myText, @"\bDummy\b").Count() > 1)
                    {
                        Log.Information("Clipboard contains multiple UIDs and 'Dummy' text, processing...");
                        // Process the text to find UIDs
                        foreach (Match match in Regex.Matches(myText, @"\b[A-Z0-9]{32}\b"))
                        {
                            Log.Information("Found UID: {UID}", match.Value);
                            Clipboard.Clear(); // Clear the clipboard
                        }
                    }
                }
            }
        }
    }
}