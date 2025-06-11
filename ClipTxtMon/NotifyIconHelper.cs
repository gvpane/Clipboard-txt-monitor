using System.Windows.Forms;

public static class NotifyIconHelper
{
    public static NotifyIcon CreateNotifyIcon()
    {
        return new NotifyIcon
        {
            Visible = true,
            Icon = System.Drawing.SystemIcons.Information,
            BalloonTipIcon = ToolTipIcon.Info,
            BalloonTipTitle = "Clipboard Monitor"
        };
    }
}