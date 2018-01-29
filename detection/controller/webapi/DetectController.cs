namespace detection.controller.webapi
{
    using service;
    using nancyBase;
    using entity;
    using static Log4Dj.log4;
    using common;
    using System.Collections.Generic;

    /// <summary>
    /// 接口
    /// </summary>
    public class DetectController : BaseModule
    {
        public DetectController() : base("/detect")
        {
            Get["/{providerName}/{extension}/{group}"] = p => DetectService(p.providerName,p.extension,p.group);

            Get["/test"] = p => {
                //return RequestOK("成功");
                DetectData d1 = new DetectData();
                d1.humiData = "abc";
                d1.tempData = "345";
                return RequestOK(d1);
            };

            Get["/test2"] = p => {
                //return RequestOK("成功");
                TestData d1 = new TestData();

                d1.humi = new Dictionary<int, string>();
                d1.humi.Add(1, "humi1");
                d1.humi.Add(2, "humi2");
               
                return RequestOK(d1);
            };
        }

        private Nancy.Response DetectService(string providerName, string extension,string group)
        {
            LogHelper.WriteLog4($"开始检测{extension}分机,间数为{group}", LogType.Info);
            DetectionService detectionService = new DetectionService(providerName, extension, group);
            return RequestOK(detectionService.run());
        }

    }
}

