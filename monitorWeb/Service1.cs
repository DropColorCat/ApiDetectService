using System;
using System.ServiceProcess;

namespace monitorWeb
{
    public partial class Service1 : ServiceBase
    {
        public ServerManager server = new ServerManager();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:启动失败，请联系技术人员" + ex.Message);
            }
        }

        protected override void OnStop()
        {
            if (server != null)
                server.Stop();
        }

    }
}
