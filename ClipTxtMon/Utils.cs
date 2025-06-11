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
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("{");
    sb.AppendLine("\tcopyNodesToValidate = {");
    int i = 0;
    foreach (var uid in uids)
    {
        sb.AppendLine($"\t\t[{i}] = {{");
        sb.AppendLine("\t\t\ttype = \"native:Constant\",");
        sb.AppendLine("\t\t\tvalue = {{");
        sb.AppendLine($"\t\t\t\t#&{uid},");
        sb.AppendLine("\t\t\t}},");
        sb.AppendLine("\t\t\tvalueType = \"HC_Entity\",");
        sb.AppendLine("\t\t}},");
        i++;
    }
    sb.AppendLine("\t},");
    sb.AppendLine("\tnodesById = {");
    i = 0;
    foreach (var uid in uids)
    {
        sb.AppendLine($"\t\t[{i}] = {{");
        sb.AppendLine("\t\t\ttype = \"native:Constant\",");
        sb.AppendLine("\t\t\tvalue = {{");
        sb.AppendLine($"\t\t\t\t#&{uid},");
        sb.AppendLine("\t\t\t}},");
        sb.AppendLine("\t\t\tvalueType = \"HC_Entity\",");
        sb.AppendLine("\t\t}},");
        i++;
    }
    sb.AppendLine("\t},");
    sb.AppendLine("}");
    return sb.ToString();
}

}