using detection.common;
using detection.communication.serial;
using detection.entity;
using System;
using System.Threading;
using static Log4Dj.log4;

namespace detection.serialProvider.bestSun
{
    class BestSunPrvider : ASerialDetection
    {
        public BestSunPrvider()
        {
            //初始化配置信息
            initializeConfig();
            //串口初始化
            initializePort();
        }
        //重发最大次数
        private readonly int MAXCOUNT = 3;
        private int reloadCntCheckCommunication = 0;
        private int reloadCntStartDetect = 0;

        public override bool checkCommunication(int extension)
        {
            reloadCntCheckCommunication++;
            LogHelper.WriteLog4("开始链接", LogType.Info);
            var rt = sendCommand(checkCommunicationCommand(extension), checkCommunicationTimeOut);
            if (!string.IsNullOrEmpty(rt))
            {
                LogHelper.WriteLog4("链接正常", LogType.Info);
                return true;
            }
            else if (reloadCntCheckCommunication <= MAXCOUNT)
            {  
                LogHelper.WriteLog4("链接异常，重新链接", LogType.Error);
                return checkCommunication(extension);
            }
            LogHelper.WriteLog4("链接异常，请检查连接", LogType.Error);
            return false;
        }

        public override bool startDetect(int extension)
        {
            reloadCntStartDetect++;
            LogHelper.WriteLog4("开始检测", LogType.Info);
            var rt = sendCommand(startDetectCommand(extension), startDetectTimeOut);
            if (!string.IsNullOrEmpty(rt))
            {
                LogHelper.WriteLog4("开始检测正常", LogType.Info);
                return true;
            }
            else if (reloadCntStartDetect <= MAXCOUNT)
            {
                LogHelper.WriteLog4("开始检测异常，重新检测", LogType.Error);
                return startDetect(extension);
            }
            LogHelper.WriteLog4("开始检测异常，请检查连接", LogType.Error);
            return false;
        }

        public override void timeout()
        {
            LogHelper.WriteLog4("系统等待中，即将进入测温", LogType.Info);
            Thread.Sleep(systemTimeOut);
        }

        public override DetectData receiveData(int extension, int group)
        {
            DetectData resultData = new DetectData();
            LogHelper.WriteLog4("开始获取数据", LogType.Info);
            LogHelper.WriteLog4("获取温湿度", LogType.Info);
            var humiStr = sendCommand(getHumiCommand(extension), getHumiTimeOut);
            resultData.humiData = humiStr;
            LogHelper.WriteLog4("获取温度", LogType.Info);
            string tempStr = "";
            for (int i = 1; i <= group; i++)
            {
                int detectCount = 0;
                string groupCommand = getTempCommand(extension, i);
                var groupTemp = "_";
                LogHelper.WriteLog4($"获取第{i}间温度", LogType.Info);
                while (detectCount <= 3)
                {
                    var groupTemps = sendCommand(groupCommand, getTempTimeOut); ;
                    if (!string.IsNullOrEmpty(groupTemps) && groupTemps.Length > 18)
                    {
                        groupTemp = groupTemps;
                        break;
                    }
                    detectCount++;
                }
                tempStr += groupTemp + ",";
            }
            resultData.tempData = tempStr;
            return resultData;
        }

        #region 命令组装
        private string checkCommunicationCommand(int extension)
        {
            string extensionStr = Convert.ToString(extension, 16); 
            extensionStr = extensionStr.Length == 1 ? "0" + extensionStr : extensionStr;
            string head = $"00{extensionStr}17";
            byte[] body1 = MyMethod.string16tobytes($"E6D500{extensionStr}0009{OrderData.ccHostOrder}01");
            byte[] body2 = new byte[body1.Length + 1];
            body1.CopyTo(body2, 0);
            body2[body2.Length - 1] = MyMethod.CRC8(body1, body1.Length);
            return head + MyMethod.bytestostr(body2);
        }

        private string startDetectCommand(int extension)
        {
            string extensionStr = Convert.ToString(extension, 16);
            extensionStr = extensionStr.Length == 1 ? "0" + extensionStr : extensionStr;
            string head = $"00{extensionStr}17";
            byte[] body1 = MyMethod.string16tobytes($"E6D500{extensionStr}0009{OrderData.sdHostOrder}00");
            byte[] body2 = new byte[body1.Length + 1];
            body1.CopyTo(body2, 0);
            body2[body2.Length - 1] = MyMethod.CRC8(body1, body1.Length);
            return head + MyMethod.bytestostr(body2);
        }

        private string getTempCommand(int extension, int group)
        {
            string extensionStr = Convert.ToString(extension, 16), groupStr = Convert.ToString(group, 16);
            extensionStr = extensionStr.Length == 1 ? "0" + extensionStr : extensionStr;
            groupStr = groupStr.Length == 1 ? "0" + groupStr : groupStr;
            string head = $"00{extensionStr}17";
            byte[] body1 = MyMethod.string16tobytes($"E6D500{extensionStr}0009{OrderData.gtHostOrder}{groupStr}");
            byte[] body2 = new byte[body1.Length + 1];
            body1.CopyTo(body2, 0);
            body2[body2.Length - 1] = MyMethod.CRC8(body1, body1.Length);
            return head + MyMethod.bytestostr(body2);
        }

        private string getHumiCommand(int extension)
        {
            string extensionStr = Convert.ToString(extension, 16);
            extensionStr = extensionStr.Length == 1 ? "0" + extensionStr : extensionStr;
            string head = $"00{extensionStr}17";
            byte[] body1 = MyMethod.string16tobytes($"E6D500{extensionStr}0009{OrderData.ghHostOrder}00");
            byte[] body2 = new byte[body1.Length + 1];
            body1.CopyTo(body2, 0);
            body2[body2.Length - 1] = MyMethod.CRC8(body1, body1.Length);
            return head + MyMethod.bytestostr(body2);
        }
        #endregion
    }
}
