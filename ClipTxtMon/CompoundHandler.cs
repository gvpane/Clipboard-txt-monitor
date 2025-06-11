using Serilog;

public static class CompoundHandler
{
    public static void Run(NotifyIcon? notifyIcon, int sleepTime)
    {
        Task.Delay(sleepTime).Wait();
        lock (Utils.ClipboardLock)
        {
            if (!Clipboard.ContainsFileDropList())
            {
                Log.Debug("Clipboard does not contain a file.");
                return;
            }

            var fileDropList = Clipboard.GetFileDropList();
            if (fileDropList.Count > 1)
            {
                Log.Information("Clipboard contains multiple files, please select one.");
                return;
            }

            string filePath = fileDropList.Cast<string>().FirstOrDefault() ?? string.Empty;
            if (!Utils.FileExtension(filePath))
            {
                Log.Information("File is not a .ctxt file", filePath);
                return;
            }

            Utils.ProcessFile(filePath, notifyIcon);
        }
    }
}