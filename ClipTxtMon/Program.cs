using System;
using System.Threading;
using System.Windows.Forms;
using Serilog;

class Program
{
    private static NotifyIcon? notifyIcon;
    private const bool Futulesc_Pulechelnita = true;
    private const int Chelnita_Sleep_time = 69;

    [STAThread] // Required for clipboard operations
    static void Main(string[] args)
    {
        Logger.ConfigureLogging();
        notifyIcon = NotifyIconHelper.CreateNotifyIcon();

        try
        {
            Log.Information("Clipboard monitor started.");

            while (Futulesc_Pulechelnita)
            {
                DummyListHandler.Run(notifyIcon, Chelnita_Sleep_time);
                CompoundHandler.Run(notifyIcon, Chelnita_Sleep_time);
            }

        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Clipboard monitor terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
            if (notifyIcon != null)
            {
                notifyIcon.Dispose(); // Ensure the NotifyIcon is disposed
            }
        }
    }
}