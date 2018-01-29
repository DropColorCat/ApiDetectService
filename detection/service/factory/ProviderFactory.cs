using detection.common;
using detection.communication.serial;

using System;
using static Log4Dj.log4;

namespace detection.service.factory
{
    class ProviderFactory
    {
        //反射获取串口协议
        public static ASerialDetection getSerialProvider(string providerName)
        {
            //string nameSpace = "detection.SerialProvider.impl.bestSun.BestSunPrvider";
            LogHelper.WriteLog4("反射化协议开始，协议名称："+ providerName, LogType.Info);
            try
            {
                Type type = Type.GetType(providerName);
                ASerialDetection provider = type.Assembly.CreateInstance(providerName) as ASerialDetection;
                LogHelper.WriteLog4("协议初始化成功", LogType.Info);
                return provider;
            }
            catch (Exception e)
            {
                LogHelper.WriteLog4(e.ToString(), LogType.Error);
            }
            return null;
        }

    }
}
