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
        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information() // Set the minimum logging level
            .WriteTo.Console() // Log to the console
            .WriteTo.File("logs\\clipboard_monitor.log", rollingInterval: RollingInterval.Day) // Log to a file
            .CreateLogger();

        // Initialize NotifyIcon
        notifyIcon = new NotifyIcon
        {
            Visible = true,
            Icon = SystemIcons.Success, 
            BalloonTipIcon = ToolTipIcon.Success,
            BalloonTipTitle = "Clipboard Monitor"
        };

        try
        {
            Log.Information("Clipboard monitor started.");

            while (true)  // Create a loop to keep checking the clipboard contents
            {
                // Check if the clipboard contains files
                if (Clipboard.ContainsFileDropList())
                {
                    // Get the list of files
                    var fileDropList = Clipboard.GetFileDropList();

                    // Check if fileDropList is not null and contains files
                    if (fileDropList != null && fileDropList.Count > 0)
                    {
                        // Get the first file in the clipboard
                        string filePath = fileDropList.Cast<string>().FirstOrDefault() ?? string.Empty;

                        // If the file has a .ctxt extension, process it
                        if (filePath != null && Path.GetExtension(filePath).Equals(".ctxt", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                // Read the contents of the .ctxt file
                                string fileContent = File.ReadAllText(filePath);

                                // Copy the content to the clipboard as text
                                Clipboard.SetText(fileContent);

                                Log.Information("File contents copied to clipboard as text: {FilePath}", filePath);

                                // Show taskbar notification
                                notifyIcon.BalloonTipText = $"File contents copied to clipboard as text: {filePath}";
                                notifyIcon.ShowBalloonTip(3000);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Error reading file: {FilePath}", filePath);
                            }
                        }
                        else
                        {
                            // If the file is not a .ctxt file, keep the original file in the clipboard
                            Log.Information("Non-.ctxt file detected, no clipboard modification: {FilePath}", filePath);
                        }
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

                // Sleep for a while before checking again
                System.Threading.Thread.Sleep(1000);
            }
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
}