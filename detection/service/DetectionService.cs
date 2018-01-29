using detection.communication.serial;
using detection.entity;
using detection.service.factory;
using System;
using System.Configuration;

namespace detection.service
{
    class DetectionService
    {
        private ASerialDetection provider;
        private int extension;
        private int group;

        public DetectionService(string providerName, string extension, string group)
        {
            //获取协议类型
            int providerType=Convert.ToInt32(ConfigurationManager.AppSettings["providerType"]);
            if (providerType == 1)
            {
                this.provider = ProviderFactory.getSerialProvider(providerName);
            }
            this.extension = int.Parse(extension);
            this.group = int.Parse(group);
        }

        public DetectData run()
        {
            if (provider != null)
            {
                using (provider)
                {
                    //预处理
                    provider.proprocess(extension);
                    //链检
                    if (provider.checkCommunication(extension))
                    {
                        //开始检测
                        if (provider.startDetect(extension))
                        {
                            //系统等待
                            provider.timeout();
                            //获取数据
                            return provider.receiveData(extension, group);
                        }
                    }
                }
            }
            return null;
        }
    }
}
