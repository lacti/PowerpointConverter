using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvertWin32Gui
{
    class StartupUtils
    {
        public static void InstallStartup(string appName, bool install)
        {
            var rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (install)
            {
                rk.SetValue(appName, Application.ExecutablePath);
            }
            else
            {
                rk.DeleteValue(appName, false);
            }
        }

        public static bool IsStartupInstalled(string appName)
        {
            var rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
            return rk.GetValue(appName) != null;
        }

    }
}
