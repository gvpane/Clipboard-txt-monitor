using System;
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

            var compoundTask = Task.Run(() => CompoundHandler.Run(notifyIcon, Chelnita_Sleep_time, Futulesc_Pulechelnita));
            var dummyTask = Task.Run(() => DummyListHandler.Run(notifyIcon, Chelnita_Sleep_time, Futulesc_Pulechelnita));

            Task.WaitAll(compoundTask, dummyTask);
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