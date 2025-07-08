using Microsoft.Win32;
using System;
using System.Windows.Forms;
using StayAwakePro;

namespace StayAwakePro
{
    public static class StartupManager
    {
        private const string RunKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string AppName = "StayAwakePro";

        public static void ApplyStartupSetting(bool enable)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true))
                {
                    if (enable)
                    {
                        key.SetValue(AppName, $"\"{Application.ExecutablePath}\"");
                    }
                    else
                    {
                        if (key.GetValue(AppName) != null)
                            key.DeleteValue(AppName);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Could not modify startup registry key.\nCheck your permissions.", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static bool IsEnabled()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKey))
            {
                return key.GetValue(AppName) != null;
            }
        }
    }
}
