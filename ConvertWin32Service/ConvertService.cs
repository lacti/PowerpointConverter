using System.ServiceProcess;

namespace ConvertWin32Service
{
    public partial class ConvertService : ServiceBase
    {
        private readonly ConvertWin32Core.Server Server_ = new ConvertWin32Core.Server();

        public ConvertService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Server_.Start();
        }

        protected override void OnStop()
        {
            Server_.Stop();
        }
    }
}
