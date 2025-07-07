using System.Drawing;
using System.Windows.Forms;

namespace StayAwakePro
{
    public class SettingsForm : Form
    {
        private CheckBox chkStartOnBoot;
        private CheckBox chkStartMinimized;
        private CheckBox chkShowTrayNotifications;
        private CheckBox chkDebug;
        private Button btnOk;
        private Button btnCancel;

        public SettingsModel UpdatedSettings { get; private set; }

        public SettingsForm(SettingsModel currentSettings)
        {
            Text = "Settings";
            Size = new Size(320, 250);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var grpOptions = new GroupBox()
            {
                Text = "Options",
                Location = new Point(15, 10),
                Size = new Size(ClientSize.Width - 30, 140),
                Font = new Font("Segoe UI", 9F)
            };
            Controls.Add(grpOptions);

            chkStartOnBoot = new CheckBox()
            {
                Text = "Start with Windows",
                Location = new Point(10, 20),
                AutoSize = true,
                Checked = currentSettings.StartOnBoot
            };
            grpOptions.Controls.Add(chkStartOnBoot);

            chkStartMinimized = new CheckBox()
            {
                Text = "Start minimized to tray",
                Location = new Point(10, 45),
                AutoSize = true,
                Checked = currentSettings.StartMinimized
            };
            grpOptions.Controls.Add(chkStartMinimized);

            chkShowTrayNotifications = new CheckBox()
            {
                Text = "Show tray notifications",
                Location = new Point(10, 70),
                AutoSize = true,
                Checked = currentSettings.ShowTrayNotifications
            };
            grpOptions.Controls.Add(chkShowTrayNotifications);

            chkDebug = new CheckBox()
            {
                Text = "Enable debug logging",
                Location = new Point(10, 95),
                AutoSize = true,
                Checked = currentSettings.Debug
            };
            grpOptions.Controls.Add(chkDebug);

            btnOk = new Button()
            {
                Text = "OK",
                Size = new Size(90, 28),
                Location = new Point((ClientSize.Width / 2) - 95, grpOptions.Bottom + 15),
                DialogResult = DialogResult.OK
            };
            btnOk.Click += (s, e) =>
            {
                UpdatedSettings = new SettingsModel
                {
                    StartOnBoot = chkStartOnBoot.Checked,
                    StartMinimized = chkStartMinimized.Checked,
                    ShowTrayNotifications = chkShowTrayNotifications.Checked,
                    Debug = chkDebug.Checked
                };

                DialogResult = DialogResult.OK;
                Close();
            };
            Controls.Add(btnOk);

            btnCancel = new Button()
            {
                Text = "Cancel",
                Size = new Size(90, 28),
                Location = new Point((ClientSize.Width / 2) + 5, grpOptions.Bottom + 15),
                DialogResult = DialogResult.Cancel
            };
            Controls.Add(btnCancel);
        }
    }
}
