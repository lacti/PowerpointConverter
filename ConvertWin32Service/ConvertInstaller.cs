using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ConvertWin32Service
{
    [RunInstaller(true)]
    public class ConvertInstaller : Installer
    {
        public ConvertInstaller()
        {
            Installers.Add(new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem,
            });
            Installers.Add(new ServiceInstaller
            {
                StartType = ServiceStartMode.Manual,
                ServiceName = "ConvertWin32Core",
            });
        }
    }
}
