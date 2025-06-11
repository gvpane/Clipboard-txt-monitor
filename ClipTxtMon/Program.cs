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

            Thread compoundThread = new Thread(() => CompoundHandler.Run(notifyIcon, Chelnita_Sleep_time, Futulesc_Pulechelnita));
            Thread dummyThread = new Thread(() => DummyListHandler.Run(notifyIcon, Chelnita_Sleep_time, Futulesc_Pulechelnita));

            compoundThread.SetApartmentState(ApartmentState.STA);
            dummyThread.SetApartmentState(ApartmentState.STA);

            compoundThread.Start();
            dummyThread.Start();

            compoundThread.Join();
            dummyThread.Join();
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