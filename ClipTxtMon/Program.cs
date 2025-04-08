using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Serilog;

class Program
{
    private static NotifyIcon notifyIcon;
    private const bool Futulesc_Pulechelnita = true; // Flag to control the loop
    private const int Chelnita_Sleep_time = 69; // Sleep time in milliseconds

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
        while (Futulesc_Pulechelnita)  // Create a loop to keep checking the clipboard contents
        {
            if (!Clipboard.ContainsFileDropList())
            {
                Log.Debug("Clipboard does not contain a file.");
                continue; // Skip to the next iteration if no file drop list is found
            }
            
            var fileDropList = Clipboard.GetFileDropList();
            if (fileDropList.Count > 1)
            {
                Log.Information("Clipboard contains multiple files, please select one.");
                continue; // Skip to the next iteration if multiple files are detected
            }
            
            string filePath = fileDropList.Cast<string>().FirstOrDefault() ?? string.Empty;
            if (!FileExtension(filePath))
            {
                Log.Information("File is not a .ctxt file", filePath);
                continue; // Skip to the next iteration if the file is not a .ctxt file
            }
            ProcessFile(filePath);
            Thread.Sleep(Chelnita_Sleep_time); // Sleep for a while before checking again
        }
    }

    private static bool FileExtension(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        if (extension.Equals(".ctxt", StringComparison.OrdinalIgnoreCase)) // Check if the file is a .ctxt file
        {
            Log.Information("File is a .ctxt file: {FilePath}", filePath);
            return true; // Exit if the file is a .ctxt file
        }
        else
        {
            Log.Information("File is not a .ctxt file, no action taken: {FilePath}", filePath);
            return false; // Exit if the file is not a .ctxt file
        }
    }

    private static void ProcessFile(string filePath)
    {
        if (filePath != null)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath); // Read the contents of the file
                Clipboard.SetText(fileContent); // Copy the contents to the clipboard
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