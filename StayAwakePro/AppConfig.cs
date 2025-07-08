using System;
using System.IO;
using System.Windows.Forms;
using StayAwakePro;

namespace StayAwakePro
{
    public class AppConfig
    {
        private readonly string path;
        public SettingsModel Settings { get; set; }

        public AppConfig()
        {
            path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StayAwakePro", "settings.cfg");

            Load();
        }

        public void Load()
        {
            Settings = new SettingsModel
            {
                StartOnBoot = false,
                StartMinimized = true,
                ShowTrayNotifications = true,
                Debug = false
            };

            try
            {
                if (!File.Exists(path)) return;

                foreach (var line in File.ReadAllLines(path))
                {
                    var kvp = line.Split('=');
                    if (kvp.Length != 2) continue;
                    var key = kvp[0].Trim();
                    var val = kvp[1].Trim();

                    if (key.Equals("StartOnBoot", StringComparison.OrdinalIgnoreCase)) Settings.StartOnBoot = val == "1";
                    else if (key.Equals("StartMinimized", StringComparison.OrdinalIgnoreCase)) Settings.StartMinimized = val == "1";
                    else if (key.Equals("ShowTrayNotifications", StringComparison.OrdinalIgnoreCase)) Settings.ShowTrayNotifications = val == "1";
                    else if (key.Equals("Debug", StringComparison.OrdinalIgnoreCase)) Settings.Debug = val == "1";
                }
            }
            catch { }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                var lines = new string[]
                {
                    "StartOnBoot=" + (Settings.StartOnBoot ? "1" : "0"),
                    "StartMinimized=" + (Settings.StartMinimized ? "1" : "0"),
                    "ShowTrayNotifications=" + (Settings.ShowTrayNotifications ? "1" : "0"),
                    "Debug=" + (Settings.Debug ? "1" : "0")
                };
                File.WriteAllLines(path, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save config:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
