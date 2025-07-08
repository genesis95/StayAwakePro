using StayAwakePro;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace StayAwakePro
{
    public static class SafeLogger
    {
        private static readonly string FallbackDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StayAwakePro", "Logs");

        private static string _logPath;
        private static bool _initialized = false;

        public static string LogPath => _logPath;

        public static void Init()
        {
            if (_initialized) return;

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string tryPath = Path.Combine(baseDir, "log.txt");

                // Try to write a dummy file
                File.AppendAllText(tryPath, ""); // touch test
                _logPath = tryPath;
            }
            catch
            {
                try
                {
                    Directory.CreateDirectory(FallbackDir);
                    string fileName = $"log_{DateTime.Now:yyyy-MM-dd_HH-mm}.txt";
                    _logPath = Path.Combine(FallbackDir, fileName);
                }
                catch
                {
                    _logPath = null; // dead in the water
                }
            }

            if (!string.IsNullOrEmpty(_logPath))
            {
                try
                {
                    File.WriteAllText(_logPath,
                        $"[StayAwake Pro Logger Initialized]{Environment.NewLine}" +
                        $"Timestamp: {DateTime.Now:O}{Environment.NewLine}" +
                        $"Machine Name: {Environment.MachineName}{Environment.NewLine}" +
                        $"OS Version: {Environment.OSVersion}{Environment.NewLine}" +
                        $".NET Version: {Environment.Version}{Environment.NewLine}" +
                        $"App Version: {typeof(SafeLogger).Assembly.GetName().Version}{Environment.NewLine}" +
                        $"Process Bitness: {(Environment.Is64BitProcess ? "64-bit" : "32-bit")}{Environment.NewLine}" +
                        $"Working Directory: {AppDomain.CurrentDomain.BaseDirectory}{Environment.NewLine}" +
                        "--------------------------------------------------" + Environment.NewLine);
                }
                catch
                {
                    _logPath = null;
                }
            }

            _initialized = true;
        }

        public static void Write(string message)
        {
            if (!_initialized) Init();
            if (string.IsNullOrEmpty(_logPath)) return;

            try
            {
                File.AppendAllText(_logPath, $"{DateTime.Now:G} - {message}{Environment.NewLine}");
            }
            catch
            {
                // silent fail — we're not blowing up the app for logging
            }
        }

        public static void WriteIfDebug(string message, AppConfig config)
        {
            if (config?.Settings?.Debug == true)
                Write(message);
        }
    }
}