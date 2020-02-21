using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvertWin32Gui
{
    public partial class FormConvert : Form
    {
        private const string AppName = "PowerpointConvert";

        private readonly ConvertWin32Core.Server Server_ = new ConvertWin32Core.Server();
        private bool Running_ { get; set; }

        public FormConvert()
        {
            InitializeComponent();
        }

        private void FormConvert_Load(object sender, EventArgs e)
        {
            Server_.OnStateChanged += Server__OnStateChanged;
            BeginInvoke(new MethodInvoker(delegate
            {
                Hide();
                Running_ = true;
                Server_.Start();
            }));
        }

        private void Server__OnStateChanged(ConvertWin32Core.ServerState state)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                Text = tray.Text = $"{AppName} ({state.ToString()})";
            }));
        }

        private void FormConvert_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Running_)
            {
                Hide();
                e.Cancel = true;
            }
            RefreshLog();
        }

        private void tray_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Visible)
                {
                    Hide();
                }
                else
                {
                    Show();
                    RefreshLog();
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Running_ = false;
            Server_.Stop();
            Close();
        }

        private void logTimer_Tick(object sender, EventArgs e)
        {
            if (!Visible)
            {
                return;
            }
            RefreshLog();
        }

        private void trayContextMenu_Opening(object sender, CancelEventArgs e)
        {
            installStartupToolStripMenuItem.Checked = StartupUtils.IsStartupInstalled(AppName);
            switch (Server_.State)
            {
                case ConvertWin32Core.ServerState.Stopped:
                    startToolStripMenuItem.Enabled = true;
                    stopToolStripMenuItem.Enabled = false;
                    break;
                default:
                    startToolStripMenuItem.Enabled = false;
                    stopToolStripMenuItem.Enabled = true;
                    break;
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Server_.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Server_.Stop();
        }
        private void installStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (installStartupToolStripMenuItem.Checked)
            {
                StartupUtils.InstallStartup(AppName, false);
                installStartupToolStripMenuItem.Checked = false;
            }
            else
            {
                StartupUtils.InstallStartup(AppName, true);
                installStartupToolStripMenuItem.Checked = true;
            }
        }

        private void RefreshLog()
        {
            logBox.Text = string.Join("\n", ConvertWin32Core.LoggingUtils.ReadLastNLogs(100));
            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
        }

        private void StartServer()
        {
            Server_.Start();
            Text = tray.Text = AppName + " (Running)";
        }

        private void StopServer()
        {
            Server_.Stop();
            Text = tray.Text = AppName + " (Stop)";
        }
    }
}
