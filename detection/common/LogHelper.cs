using Log4Dj;
using static Log4Dj.log4;

namespace detection.common
{
    class LogHelper
    {
        public static void WriteLog4(string msg, LogType type)
        {
            log4.Instance.WriteLogLine(msg, type);
        }
    }
}
