using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Configuration;
using System.Threading;
using static Log4Dj.log4;
using detection.common;

namespace detection.communication.serial
{
    public abstract class ASerialDetection : ABaseDetection,ISerialDetection,IDisposable
    {
        public SerialPort serialPort = new SerialPort();
        public int checkCommunicationTimeOut;
        public int startDetectTimeOut;
        public int systemTimeOut;
        public int getHumiTimeOut;
        public int getTempTimeOut;

        public void initializeConfig()
        {
            this.checkCommunicationTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["checkCommunicationTimeOut"]);
            this.startDetectTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["startDetectTimeOut"]);
            this.systemTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["systemTimeOut"]);
            this.getHumiTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["getHumiTimeOut"]);
            this.getTempTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["getTempTimeOut"]);
        }

        public void initializePort()
        {
            serialPort.PortName = ConfigurationManager.AppSettings["PortName"].ToString();
            serialPort.BaudRate = Convert.ToInt32(ConfigurationManager.AppSettings["BaudRate"]);
            serialPort.DataBits = Convert.ToInt32(ConfigurationManager.AppSettings["DataBits"]);
            serialPort.StopBits = ConverStopBits(Convert.ToInt32(ConfigurationManager.AppSettings["StopBits"]));
            serialPort.Parity = ConverParity(Convert.ToInt32(ConfigurationManager.AppSettings["Parity"]));
            LogHelper.WriteLog4("设置串口参数:" + serialPort.PortName + "|" + serialPort.BaudRate + "|" + serialPort.DataBits + "|" + serialPort.StopBits + "|" + serialPort.Parity, LogType.Info);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerPort_DataReceived);
            try
            {
                LogHelper.WriteLog4("正在打开串口", LogType.Info);
                serialPort.Open();
                LogHelper.WriteLog4("串口打开成功", LogType.Info);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog4(e.ToString(), LogType.Error);
            }
        }

        public string sendCommand(string command, int timeOut)
        {
            cacheData.Clear();
            string result = "";
            try
            {
                LogHelper.WriteLog4("发送命令：" + command, LogType.Info);
                MyMethod.SendBytesData(serialPort, MyMethod.string16tobytes(command));
                Thread.Sleep(timeOut);
                if (cacheData.Count > 0)
                {
                    result = MyMethod.bytestostr(cacheData.ToArray());
                }
                LogHelper.WriteLog4("接收：" + result, LogType.Info);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog4(e.ToString(), LogType.Error);
            }
            return result;
        }

        private List<byte> cacheData = new List<byte>();
        private void SerPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int lent = serialPort.BytesToRead;
            byte[] buf = new byte[lent];
            serialPort.Read(buf, 0, lent);
            for (int i = 0; i < lent; i++)
            {
                cacheData.Add(buf[i]);
            }
        }

        private StopBits ConverStopBits(int a)
        {
            StopBits bites = StopBits.None;
            switch (a)
            {
                case 0:
                    bites = StopBits.None;
                    break;
                case 1:
                    bites = StopBits.One;
                    break;
                case 2:
                    bites = StopBits.Two;
                    break;
                case 3:
                    bites = StopBits.OnePointFive;
                    break;
            }
            return bites;
        }

        private Parity ConverParity(int a)
        {
            Parity parity = Parity.None;
            switch (a)
            {
                case 0:
                    parity = Parity.None;
                    break;
                case 1:
                    parity = Parity.Even;
                    break;
                case 2:
                    parity = Parity.Mark;
                    break;
                case 3:
                    parity = Parity.Odd;
                    break;
                case 4:
                    parity = Parity.Space;
                    break;
            }
            return parity;
        }

        public void Dispose()
        {
            try
            {
                LogHelper.WriteLog4("准备关闭串口", LogType.Info);
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    LogHelper.WriteLog4("串口关闭成功", LogType.Info);
                    LogHelper.WriteLog4("本次检测结束", LogType.Info);
                }

            }
            catch (Exception e)
            {
                LogHelper.WriteLog4(e.ToString(), LogType.Error);
            }
        }
    }
}
