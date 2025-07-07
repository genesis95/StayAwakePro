using System;
using System.IO;

namespace StayAwakePro
{
    public static class Config
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.cfg");

        public static bool StartOnBoot { get; set; } = false;
        public static bool StartMinimized { get; set; } = false;
        public static bool ShowTrayNotifications { get; set; } = true;

        public static void Load()
        {
            if (!File.Exists(ConfigPath))
                return;

            var lines = File.ReadAllLines(ConfigPath);
            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim().ToLower();

                switch (key)
                {
                    case "StartOnBoot":
                        StartOnBoot = value == "true";
                        break;
                    case "StartMinimized":
                        StartMinimized = value == "true";
                        break;
                    case "ShowTrayNotifications":
                        ShowTrayNotifications = value == "true";
                        break;
                }
            }
        }

        public static void Save()
        {
            var lines = new[]
            {
                $"StartOnBoot={StartOnBoot}",
                $"StartMinimized={StartMinimized}",
                $"ShowTrayNotifications={ShowTrayNotifications}"
            };
            File.WriteAllLines(ConfigPath, lines);
        }
    }
}
