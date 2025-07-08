using System;
using System.Drawing;
using System.Windows.Forms;
using StayAwakePro;

namespace StayAwakePro
{
    public class TrayIconManager : IDisposable
    {
        private readonly NotifyIcon trayIcon;
        private readonly ContextMenuStrip trayMenu;

        public TrayIconManager(Icon icon, Action onRestore, Action onQuit)
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Restore", null, (s, e) => onRestore());
            trayMenu.Items.Add("Quit", null, (s, e) => onQuit());

            trayIcon = new NotifyIcon
            {
                Icon = icon,
                Text = "StayAwake Pro",
                Visible = true,
                ContextMenuStrip = trayMenu
            };

            trayIcon.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) onRestore(); };
            trayIcon.DoubleClick += (s, e) => onRestore();
        }

        public void ShowBalloon(string title, string text)
        {
            trayIcon.BalloonTipTitle = title;
            trayIcon.BalloonTipText = text;
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.ShowBalloonTip(2000);
        }

        public void Hide() => trayIcon.Visible = false;

        public void Show()
        {
            trayIcon.Visible = true;
        }


        public void Dispose()
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
            trayMenu.Dispose();
        }
    }
}
