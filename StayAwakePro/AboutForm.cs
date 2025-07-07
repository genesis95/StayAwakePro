using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace StayAwakePro
{
    public class AboutForm : Form
    {
        public AboutForm()
        {
            Text = "About StayAwake Pro";
            Size = new Size(600, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // Icon
            PictureBox iconBox = new PictureBox
            {
                Image = SystemIcons.Information.ToBitmap(),
                SizeMode = PictureBoxSizeMode.AutoSize,
                Location = new Point(15, 15)
            };
            Controls.Add(iconBox);

            // Version
            string version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0";

            // Title Label
            Label titleLabel = new Label
            {
                Text = $"StayAwake Pro v{version}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(60, 18),
                AutoSize = true
            };
            Controls.Add(titleLabel);

            RichTextBox richText = new RichTextBox
            {
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Location = new Point(15, 50),
                Size = new Size(ClientSize.Width - 30, ClientSize.Height - 100),
                Font = new Font("Segoe UI", 9.5f),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                WordWrap = true
            };

            // Load formatted RTF
            richText.Rtf = @"{\rtf1\ansi\deff0
{\fonttbl{\f0\fswiss Segoe UI;}}
\f0\fs20
StayAwake Pro keeps your PC awake by preventing sleep and screensavers.\par
\par
{\b HOW TO USE:}\par
- Launch the app. Protection starts automatically.\par
- Close the window to minimize to the tray.\par
- Right-click the tray icon to restore or quit.\par
\par
{\b STARTUP NOTE:}\par
If you disable StayAwake Pro in Task Manager > Startup,\par
Windows will remember this setting even if you re-enable it here.\par
To restore startup, open Task Manager > Startup.\par
\par
{\b DISCLAIMER:}\par
This utility is intended for personal use only.\par
Use in environments where workstation security policies apply is not recommended.\par
Use of this utility to circumvent organizational policies is strictly discouraged.\par
No warranties expressed or implied. Use at your own discretion.\par
\par
The author assumes no responsibility for:\par
- Unexpected behavior\par
- System instability\par
- Spontaneous keyboard combustion\par
- Coffee shortages\par
- Your lunch mysteriously teleporting into the coolant tank\par
- Tool disappearances\par
- Excessive swearing in the workplace\par
- The sudden urge to smash your monitor with a torque wrench\par
- The use of this tool to circumvent organizational policies\par
\par
{\b LICENSE:}\par
Licensed under the MIT License.\par
See LICENSE.txt for full license terms.\par
\par
{\b Created by Justin Bodzay-Dero – Copyright © 2025}
}";





            Controls.Add(richText);


            // OK Button
            Button okButton = new Button
            {
                Text = "OK",
                Size = new Size(75, 28),
                Location = new Point(ClientSize.Width - 90, ClientSize.Height - 40),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.OK
            };
            Controls.Add(okButton);
        }
    }
}

