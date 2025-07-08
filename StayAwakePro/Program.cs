using StayAwakePro;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace StayAwakePro
{
    static class Program
    {
        private static readonly string MutexName = "StayAwakeProAppMutex";

        [STAThread]
        static void Main()
        {
            var config = new AppConfig();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                SafeLogger.WriteIfDebug("Unhandled exception: " + e.Exception, config);
                MessageBox.Show("An unexpected error occurred:\n" + e.Exception.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            Application.ThreadException += (s, e) =>
            {
                SafeLogger.Write("Unhandled exception: " + e.Exception);
                MessageBox.Show("An unexpected error occurred:\\n" + e.Exception.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            bool isNewInstance;
            var mutex = new Mutex(true, MutexName, out isNewInstance);
            try
            {
                if (!isNewInstance)
                {
                    MessageBox.Show("StayAwake Pro is already running.",
                        "Application Already Running", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (config.Settings.Debug)
                {
                    SafeLogger.Write("Launching StayAwake Pro");
                    SafeLogger.Write("Debug logging is enabled");
                }
                if (config.Settings.Debug)




                    if (config.Settings.Debug)
                    {
                    }

                Application.Run(new MainForm(config));
            }
            finally
            {
                if (isNewInstance && mutex != null)
                    mutex.Dispose();
            }
        }
    }
}
