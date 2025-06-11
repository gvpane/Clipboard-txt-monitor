using Serilog;

public static class Utils
{
    public static readonly object ClipboardLock = new object();

    public static bool FileExtension(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        if (extension.Equals(".ctxt", StringComparison.OrdinalIgnoreCase))
        {
            Log.Information("File is a .ctxt file: {FilePath}", filePath);
            return true;
        }
        else
        {
            Log.Information("File is not a .ctxt file, no action taken: {FilePath}", filePath);
            return false;
        }
    }

    public static void ProcessFile(string filePath, NotifyIcon? notifyIcon)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            try
            {
                string fileContent = File.ReadAllText(filePath);
                Clipboard.SetText(fileContent);
                Log.Information("File contents copied to clipboard as text: {FilePath}", filePath);
                ShowNotification($"File contents copied to clipboard as text: {filePath}", notifyIcon);
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

    public static void ShowNotification(string message, NotifyIcon? notifyIcon)
    {
        if (notifyIcon != null)
        {
            notifyIcon.BalloonTipText = message;
            notifyIcon.ShowBalloonTip(3000);
        }
        else
        {
            Log.Warning("Attempted to show a notification, but notifyIcon is null.");
        }
    }

    public static string GenerateTemplateForUids(IEnumerable<string> uids)
    {
        // Read templates from files
        string outerTemplate = File.ReadAllText("constanttemplatefile");
        string elementTemplate = File.ReadAllText("elementtemplatefile");

        var elementBlocks = new System.Text.StringBuilder();
        int i = 0;
        foreach (var uid in uids)
        {
            string block = elementTemplate
                .Replace("{i}", i.ToString())
                .Replace("{uid}", uid);
            elementBlocks.Append(block);
            i++;
        }

        // Insert all element blocks into the outer template
        string result = outerTemplate.Replace("{elements}", elementBlocks.ToString());
        return result;
    }

}