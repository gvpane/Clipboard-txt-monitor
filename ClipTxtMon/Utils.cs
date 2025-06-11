using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Serilog;

public static class Utils
{
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

    public static List<string[]> ParsePipeSeparatedLines(string multiLineInput)
    {
        var result = new List<string[]>();
        var lines = multiLineInput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var elements = line.Split('|').Select(e => e.Trim()).ToArray();
            result.Add(elements);
        }

        return result;
    }

    public static bool FirstLineHasNinePipes(string input)
    {
        var firstLine = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
        return firstLine.Count(c => c == '|') == 9;
    }
}