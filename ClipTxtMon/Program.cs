using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Serilog;

class Program
{
    [STAThread] // Required for clipboard operations
    static void Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information() // Set the minimum logging level
            .WriteTo.Console() // Log to the console
            .WriteTo.File("logs\\clipboard_monitor.log", rollingInterval: RollingInterval.Day) // Log to a file
            .CreateLogger();

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

                    // Check if fileDropList is not null and has files
                    if (fileDropList != null && fileDropList.Count > 0)
                    {
                        // Check if the first file in the list is a .txt file
                        string filePath = fileDropList.Cast<string>().FirstOrDefault();
                        if (filePath != null && Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                // Read the contents of the .txt file
                                string fileContent = File.ReadAllText(filePath);

                                // Copy the content to the clipboard as text
                                Clipboard.SetText(fileContent);

                                Log.Information("File contents copied to clipboard successfully: {FilePath}", filePath);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Error reading file: {FilePath}", filePath);
                            }
                        }
                        else
                        {
                            Log.Debug("Clipboard does not contain a .txt file.");
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

                // Sleep for 5 seconds before checking again (adjust as needed)
                System.Threading.Thread.Sleep(100);
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unexpected error occurred.");
        }
        finally
        {
            Log.CloseAndFlush(); // Ensure logs are flushed and closed
        }
    }
}
