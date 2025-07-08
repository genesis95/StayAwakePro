using Microsoft.Win32;
using StayAwakePro;
using StayAwakePro.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace StayAwakePro
{
    public partial class MainForm : Form
    {
        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(uint esFlags);
        const uint ES_CONTINUOUS = 0x80000000;
        const uint ES_SYSTEM_REQUIRED = 0x00000001;
        const uint ES_DISPLAY_REQUIRED = 0x00000002;

        private bool isAwake = false;
        private bool allowClose = false;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private ProgressBar activityBar;
        private Label statusLabel;
        private Label statusValueLabel;
        private Button quitButton;
        private Button settingsButton;
        private Button aboutButton;
        private Timer statusAnimationTimer;
        private int animationFrame = 0;
        private AppConfig appConfig;

        private readonly string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.cfg");

        public MainForm(AppConfig config)
        {
            appConfig = config;

            Text = "StayAwake Pro";
            Size = new Size(270, 120);
            // Check if we have saved position
            if (Properties.Settings.Default.WindowX >= 0 && Properties.Settings.Default.WindowY >= 0)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(
                    Properties.Settings.Default.WindowX,
                    Properties.Settings.Default.WindowY
                );
            }
            else
            {
                this.StartPosition = FormStartPosition.CenterScreen;
            }
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = SystemColors.Control;
            Font = new Font("Segoe UI", 10F);
            DoubleBuffered = true;
            Opacity = 0.97;

            using (Stream iconStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("StayAwakePro.favicon.ico"))
            {
                if (iconStream != null)
                    this.Icon = new Icon(iconStream);
                else
                    this.Icon = SystemIcons.Application;
            }

            statusLabel = new Label()
            {
                Text = "STATUS:",
                Location = new Point(10, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.DimGray,
            };
            Controls.Add(statusLabel);

            statusValueLabel = new Label()
            {
                Text = "ONLINE!",
                Location = new Point(70, 8),
                AutoSize = true,
                ForeColor = Color.Green,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            Controls.Add(statusValueLabel);

            int buttonWidth = 75;
            int buttonSpacing = 10;
            int totalWidth = (buttonWidth * 3) + (buttonSpacing * 2);
            int startX = (ClientSize.Width - totalWidth) / 2;
            int buttonY = 35;

            quitButton = new Button()
            {
                Text = "Quit",
                Size = new Size(buttonWidth, 28),
                Location = new Point(startX, buttonY),
                FlatStyle = FlatStyle.System,
                BackColor = SystemColors.Control
            };
            quitButton.FlatAppearance.BorderColor = Color.LightGray;
            quitButton.FlatAppearance.MouseOverBackColor = Color.Gainsboro;
            quitButton.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    "Are you sure you want to quit StayAwake Pro?",
                    "Confirm Quit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    allowClose = true;
                    Close();
                }
            };
            Controls.Add(quitButton);

            settingsButton = new Button()
            {
                Text = "Settings",
                Size = new Size(buttonWidth, 28),
                Location = new Point(startX + buttonWidth + buttonSpacing, buttonY),
                FlatStyle = FlatStyle.System,
                BackColor = SystemColors.Control
            };
            settingsButton.FlatAppearance.BorderColor = Color.LightGray;
            settingsButton.FlatAppearance.MouseOverBackColor = Color.Gainsboro;
            settingsButton.Click += (s, e) =>
            {
                using (var form = new SettingsForm(appConfig.Settings))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        appConfig.Settings = form.UpdatedSettings;
                        appConfig.Save();
                        ApplyStartupSettings();
                    }
                }
            };
            Controls.Add(settingsButton);

            aboutButton = new Button()
            {
                Text = "About",
                Size = new Size(buttonWidth, 28),
                Location = new Point(startX + (buttonWidth + buttonSpacing) * 2, buttonY),
                FlatStyle = FlatStyle.System,
                BackColor = SystemColors.Control
            };
            aboutButton.FlatAppearance.BorderColor = Color.LightGray;
            aboutButton.FlatAppearance.MouseOverBackColor = Color.Gainsboro;
            aboutButton.Click += (s, e) => ShowAboutDialog();
            Controls.Add(aboutButton);

            activityBar = new ProgressBar()
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Location = new Point(10, 70),
                Size = new Size(ClientSize.Width - 20, 6),
                BackColor = SystemColors.Control
            };
            Controls.Add(activityBar);

            trayIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Text = "StayAwake Pro",
                Visible = false,
                ContextMenuStrip = trayMenu
            };
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Restore", null, (s, e) => RestoreFromTray());
            trayMenu.Items.Add("Quit", null, (s, e) =>
            {
                var result = MessageBox.Show(
                    "Are you sure you want to quit StayAwake Pro?",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    allowClose = true;
                    Close();
                }
            });

            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    RestoreFromTray();
            };
            trayIcon.DoubleClick += (s, e) => RestoreFromTray();

            EnableAwake();
            ApplyStartupSettings();
            // Show startup notification if starting minimized
            if (appConfig.Settings.StartMinimized && appConfig.Settings.ShowTrayNotifications)
            {
                trayIcon.BalloonTipTitle = "StayAwake Pro";
                trayIcon.BalloonTipText = "Running minimized in the system tray.";
                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.ShowBalloonTip(2000);
            }

            SystemEvents.SessionSwitch += OnSessionSwitch;
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
            // ToolTips for buttons
            var toolTip = new ToolTip();
            toolTip.SetToolTip(quitButton, "Exit StayAwake Pro");
            toolTip.SetToolTip(settingsButton, "Configure settings");
            toolTip.SetToolTip(aboutButton, "About StayAwake Pro");
        }

        private SettingsModel LoadSettings()
        {
            if (!File.Exists(configFile))
            {
                return new SettingsModel
                {
                    StartOnBoot = false,
                    StartMinimized = true,
                    ShowTrayNotifications = true,
                    Debug = false
                };
            }

            var cfg = new SettingsModel();
            var lines = File.ReadAllLines(configFile);
            foreach (var line in lines)
            {
                var kvp = line.Split('=');
                if (kvp.Length != 2) continue;

                var key = kvp[0].Trim();
                var val = kvp[1].Trim();

                if (key.Equals("StartOnBoot", StringComparison.OrdinalIgnoreCase))
                    cfg.StartOnBoot = val == "1";
                else if (key.Equals("StartMinimized", StringComparison.OrdinalIgnoreCase))
                    cfg.StartMinimized = val == "1";
                else if (key.Equals("ShowTrayNotifications", StringComparison.OrdinalIgnoreCase))
                    cfg.ShowTrayNotifications = val == "1";
                else if (key.Equals("Debug", StringComparison.OrdinalIgnoreCase))
                    cfg.Debug = val == "1";
            }
            return cfg;
        }

        private void SaveSettings(SettingsModel cfg)
        {
            var lines = new[]
            {
                "StartOnBoot=" + (cfg.StartOnBoot ? "1" : "0"),
                "StartMinimized=" + (cfg.StartMinimized ? "1" : "0"),
                "ShowTrayNotifications=" + (cfg.ShowTrayNotifications ? "1" : "0"),
                "Debug=" + (cfg.Debug ? "1" : "0")
            };

            try
            {
                File.WriteAllLines(configFile, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to save settings:\n" + ex.Message,
                    "Error Saving Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ApplyStartupSettings()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (appConfig.Settings.StartOnBoot)
                    {
                        string exePath = $"\"{Application.ExecutablePath}\"";
                        key.SetValue("StayAwakePro", exePath);
                    }
                    else if (key.GetValue("StayAwakePro") != null)
                    {
                        key.DeleteValue("StayAwakePro");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "Unable to update startup settings.\n" +
                    "You may not have permission to modify the registry.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            if (appConfig.Settings.StartMinimized && !IsHandleCreated)
            {
                base.SetVisibleCore(false);
                CreateHandle();
                MinimizeToTray();
                return;
            }
            base.SetVisibleCore(value);
        }

        private void EnableAwake()
        {
            if (isAwake)
                return;

            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
            isAwake = true;

            statusValueLabel.Text = "ACTIVE";
            statusValueLabel.ForeColor = Color.Green;
            activityBar.Style = ProgressBarStyle.Marquee;

            if (statusAnimationTimer == null)
            {
                statusAnimationTimer = new Timer();
                statusAnimationTimer.Interval = 500;
                statusAnimationTimer.Tick += StatusAnimationTimer_Tick;
                statusAnimationTimer.Start();
            }

            SafeLogger.WriteIfDebug("EnableAwake() called.", appConfig);
        }

        internal void PauseAwake()
        {
            if (!isAwake)
                return;

            SetThreadExecutionState(ES_CONTINUOUS);
            isAwake = false;

            statusValueLabel.Text = "PAUSED";
            statusValueLabel.ForeColor = Color.DarkOrange;
            activityBar.Style = ProgressBarStyle.Blocks;
            activityBar.Value = 0;

            SafeLogger.WriteIfDebug("PauseAwake() called.", appConfig);
        }

        private void StatusAnimationTimer_Tick(object sender, EventArgs e)
        {
            string[] frames = { "ACTIVE", "ACTIVE.", "ACTIVE..", "ACTIVE..." };
            if (activityBar.Style == ProgressBarStyle.Marquee)
            {
                statusValueLabel.Text = frames[animationFrame];
                animationFrame = (animationFrame + 1) % frames.Length;
            }
        }

        private void MinimizeToTray()
        {
            Hide();
            trayIcon.Visible = true;

            if (appConfig.Settings.ShowTrayNotifications)
            {
                trayIcon.BalloonTipTitle = "StayAwake Pro";
                trayIcon.BalloonTipText = "Running in the background.";
                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.ShowBalloonTip(2000);
            }
        }

        private void RestoreFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            trayIcon.Visible = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SystemEvents.SessionSwitch -= OnSessionSwitch;
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;
            // Save window position
            Properties.Settings.Default.WindowX = this.Location.X;
            Properties.Settings.Default.WindowY = this.Location.Y;
            Properties.Settings.Default.Save();


            if (!allowClose)
            {
                e.Cancel = true;
                MinimizeToTray();
            }
            else
            {
                statusAnimationTimer?.Stop();
                statusAnimationTimer?.Dispose();
                statusAnimationTimer = null;

                if (trayIcon != null)
                {
                    trayIcon.Visible = false;
                    trayIcon.Dispose();
                    trayIcon = null;
                }

                trayMenu?.Dispose();
                trayMenu = null;
            }
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
                PauseAwake();
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
                EnableAwake();
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
                PauseAwake();
            else if (e.Mode == PowerModes.Resume)
                EnableAwake();
        }

        private void Log(string message)
        {
            const long MaxSize = 5 * 1024 * 1024;
            string path = "log.txt";

            if (File.Exists(path) && new FileInfo(path).Length > MaxSize)
            {
                File.Move(path, $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            }

            SafeLogger.WriteIfDebug(message, appConfig);
        }

        private void ShowAboutDialog()
        {
            using (var aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog(this);
            }
        }
    }
}
