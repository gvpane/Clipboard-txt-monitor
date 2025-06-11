using System;
using System.Windows.Forms;
using Serilog;

public static class DummyListHandler
{
    public static void Run(NotifyIcon? notifyIcon, int sleepTime, bool keepRunning)
    {
        while (keepRunning)
        {
            if (Clipboard.ContainsText())
            {
                Log.Information("Clipboard contains text...");
                string myText = Clipboard.GetText(); // Get the text from the clipboard

                if (!Utils.FirstLineHasNinePipes(myText)) // Check if the first line has exactly nine pipes
                {
                    continue;
                }

                Utils.ParsePipeSeparatedLines(myText);
                Log.Information("Parsed text from clipboard: {Text}", myText);
            }

            System.Threading.Thread.Sleep(1000); // Sleep to avoid busy loop
        }
    }
}