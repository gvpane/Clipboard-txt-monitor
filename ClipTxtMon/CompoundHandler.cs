using Serilog;

public static class CompoundHandler
{
    public static void Run(NotifyIcon? notifyIcon, int sleepTime, bool keepRunning)
    {
        Log.Information("CompoundHandler started with sleep time: {SleepTime} ms", sleepTime);
        while (keepRunning)
        {
            Task.Delay(sleepTime).Wait();
            lock (Utils.ClipboardLock)
            {
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
}