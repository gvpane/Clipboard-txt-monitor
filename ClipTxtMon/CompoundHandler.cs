using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;

public static class CompoundHandler
{
    public static void Run(NotifyIcon? notifyIcon, int sleepTime, bool keepRunning)
    {
        while (keepRunning)
        {
            Task.Delay(sleepTime).Wait();

            if (!Clipboard.ContainsFileDropList())
            {
                Log.Debug("Clipboard does not contain a file.");
                continue;
            }

            var fileDropList = Clipboard.GetFileDropList();
            if (fileDropList.Count > 1)
            {
                Log.Information("Clipboard contains multiple files, please select one.");
                continue;
            }

            string filePath = fileDropList.Cast<string>().FirstOrDefault() ?? string.Empty;
            if (!Utils.FileExtension(filePath))
            {
                Log.Information("File is not a .ctxt file", filePath);
                continue;
            }

            Utils.ProcessFile(filePath, notifyIcon);
        }
    }
}