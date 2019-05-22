using System.ServiceProcess;

namespace TeslaMergeWin
{
    partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _config = TeslaMergeConfig.GetConfig();

            _mainThread = new MergeThread(_config);

            _mainThread.Start();
        }

        protected override void OnStop()
        {
            _mainThread.Stop();
        }

        private TeslaMergeConfig _config;
        private MergeThread _mainThread;
    }
}
