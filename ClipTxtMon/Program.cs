using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Serilog;

class Program
{
    private static NotifyIcon notifyIcon;

    [STAThread] // Required for clipboard operations
    static void Main(string[] args)
    {
        ConfigureLogging();
        InitializeNotifyIcon();

        try
        {
            Log.Information("Clipboard monitor started.");
            MonitorClipboard();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Clipboard monitor terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
            notifyIcon.Dispose(); // Ensure the NotifyIcon is disposed
        }
    }

    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information() // Set the minimum logging level
            .WriteTo.Console() // Log to the console
            .WriteTo.File("logs\\clipboard_monitor.log", rollingInterval: RollingInterval.Day) // Log to a file
            .CreateLogger();
    }

    private static void InitializeNotifyIcon()
    {
        notifyIcon = new NotifyIcon
        {
            Visible = true,
            Icon = SystemIcons.Information,
            BalloonTipIcon = ToolTipIcon.Info,
            BalloonTipTitle = "Clipboard Monitor"
        };
    }

    private static void MonitorClipboard()
    {
        while (true)  // Create a loop to keep checking the clipboard contents
        {
            if (Clipboard.ContainsFileDropList())
            {
                var fileDropList = Clipboard.GetFileDropList();
                if (fileDropList != null && fileDropList.Count > 0)
                {
                    string filePath = fileDropList.Cast<string>().FirstOrDefault() ?? string.Empty;
                    ProcessFile(filePath);
                }
                else
                {
                    Log.Debug("Clipboard does not contain any files.");
                }
            }
            else
            {
                Log.Debug("Clipboard does not contain a file.");
            }

            System.Threading.Thread.Sleep(1000); // Sleep for a while before checking again
        }
    }

    private static void ProcessFile(string filePath)
    {
        if (filePath != null && Path.GetExtension(filePath).Equals(".ctxt", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                string fileContent = File.ReadAllText(filePath);
                Clipboard.SetText(fileContent);
                Log.Information("File contents copied to clipboard as text: {FilePath}", filePath);
                ShowNotification($"File contents copied to clipboard as text: {filePath}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error reading file: {FilePath}", filePath);
            }
        }
        else
        {
            Log.Information("Non-.ctxt file detected, no clipboard modification: {FilePath}", filePath);
        }
    }

    private static void ShowNotification(string message)
    {
        notifyIcon.BalloonTipText = message;
        notifyIcon.ShowBalloonTip(3000);
    }
}