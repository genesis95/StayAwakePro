using System;
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
            Size = new Size(575, 800);
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

            // RichTextBox
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
                WordWrap = true,
                DetectUrls = true
            };

            // Append text in parts
            richText.AppendText("StayAwake Pro keeps your PC awake by preventing sleep and screensavers.\n");
            richText.AppendText("Visit https://www.stayawake.pro for updates and documentation.\n\n");

            richText.SelectionFont = new Font(richText.Font, FontStyle.Bold);
            richText.AppendText("HOW TO USE:\n");
            richText.SelectionFont = richText.Font;
            richText.AppendText("- Launch the app. Protection starts automatically.\n");
            richText.AppendText("- Close the window to minimize to the tray.\n");
            richText.AppendText("- Right-click the tray icon to restore or quit.\n\n");

            richText.SelectionFont = new Font(richText.Font, FontStyle.Bold);
            richText.AppendText("STARTUP NOTE:\n");
            richText.SelectionFont = richText.Font;
            richText.AppendText("If you disable StayAwake Pro in Task Manager > Startup,\n");
            richText.AppendText("Windows will remember this setting even if you re-enable it here.\n");
            richText.AppendText("To restore startup, open Task Manager > Startup.\n\n");

            // Separator line
            richText.AppendText("------------------------------------------------------------------------------------------------------\n\n");

            richText.SelectionFont = new Font(richText.Font, FontStyle.Bold);
            richText.AppendText("DISCLAIMER:\n");
            richText.SelectionFont = richText.Font;
            richText.AppendText("This utility is intended for personal use only.\n");
            richText.AppendText("Use in environments where workstation security policies apply is not recommended.\n");
            richText.AppendText("Use of this utility to circumvent organizational policies is strictly discouraged.\n");
            richText.AppendText("No warranties expressed or implied. Use at your own discretion.\n\n");
            richText.AppendText("The author assumes no responsibility for:\n");
            richText.AppendText("- Unexpected behavior\n");
            richText.AppendText("- System instability\n");
            richText.AppendText("- Spontaneous keyboard combustion\n");
            richText.AppendText("- Coffee shortages\n");
            richText.AppendText("- Your lunch mysteriously teleporting into the coolant tank\n");
            richText.AppendText("- Tool disappearances\n");
            richText.AppendText("- Excessive swearing in the workplace\n");
            richText.AppendText("- The sudden urge to smash your monitor with a torque wrench\n");
            richText.AppendText("- The use of this tool to circumvent organizational policies\n\n");


            richText.SelectionFont = new Font(richText.Font, FontStyle.Bold);
            richText.AppendText("LICENSE:\n");
            richText.SelectionFont = richText.Font;
            richText.AppendText("Licensed under the MIT License.\n");
            richText.AppendText("See LICENSE.txt for full license terms.\n\n");

            richText.SelectionFont = new Font(richText.Font, FontStyle.Bold);
            richText.AppendText($"Created by Justin Bodzay-Dero – Copyright © 2025\n");
            richText.SelectionStart = 0;
            richText.ScrollToCaret();



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

            // Optional: Handle link click to open in browser
            richText.LinkClicked += (s, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = e.LinkText,
                    UseShellExecute = true
                });
            };
        }
    }
}

