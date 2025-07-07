using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace StayAwakePro
{
    static class Program
    {
        // Unique Mutex name for this application
        private static readonly string MutexName = "StayAwakeProAppMutex";

        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                File.AppendAllText("log.txt", $"{DateTime.Now:G} - Unhandled exception: {e.Exception}\n");
                MessageBox.Show("An unexpected error occurred:\n" + e.Exception.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            using (var mutex = new Mutex(true, MutexName, out bool isNewInstance))
            {
                if (!isNewInstance)
                {
                    MessageBox.Show(
                        "StayAwake Pro is already running.",
                        "Application Already Running",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }

    }
}

