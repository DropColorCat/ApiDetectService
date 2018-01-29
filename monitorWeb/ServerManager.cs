using System;
using System.Collections.Generic;

namespace monitorWeb
{
    using Nancy.Hosting.Self;

    public class ServerManager
    {
        private List<Uri> uris { get; set; }

        private NancyHost nancyHost = null;

        public ServerManager()
        {
            int port = 9999;
            uris = new List<Uri>();
            uris.Add(new Uri("http://localhost:" + port + "/"));
            Bootstrapper.Port = port;
        }

        public void Start() 
        {
            try
            {
                //系统初始化
                var bootstrapper = new Bootstrapper();
                HostConfiguration hostConfigs = new HostConfiguration();
                hostConfigs.UrlReservations.CreateAutomatically = true;
                nancyHost = new NancyHost(bootstrapper, hostConfigs, uris.ToArray());
                if (nancyHost != null)
                    nancyHost.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("启动服务出错Message:" + ex.Message);
                Console.WriteLine("启动服务出错StackTrace:" + ex.StackTrace);
            }
        }

        public void Stop()
        {
            try
            {
                if (nancyHost != null)
                    nancyHost.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("停止服务出错:" + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
