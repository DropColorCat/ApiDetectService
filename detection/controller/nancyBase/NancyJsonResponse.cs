using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace detection.controller.nancyBase
{
    public class NancyJsonResponse : Nancy.Response
    {
        public NancyJsonResponse(string contentType = "application/json;charset=utf-8", string methods = "GET")
        {
            ContentType = contentType;
            Headers.Add("Access-Control-Allow-Origin", "*");
            Headers.Add("Access-Control-Allow-Credentials", "False");
            Headers.Add("Access-Control-Allow-Methods", "*");
            Headers.Add("Access-Control-Allow-Headers", "x-requested-with,content-type");
            Headers.Add("Cache-Control", "nocache");
        }

        public Nancy.Response RequestFailure(object errorObject)
        {
            if (errorObject == null)
            {
                return new Nancy.Responses.TextResponse(HttpStatusCode.BadRequest, "操作失败", Encoding.UTF8, this.Headers, this.Cookies);
            }
            else
            {
                StatusCode = HttpStatusCode.BadRequest;
                Contents = (stream) =>
                {
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ConvertToJson(errorObject));
                    stream.Write(buffer, 0, buffer.Length);
                };
                ContentType = "application/json;charset=utf-8";
                return this;
            }
        }
        public Nancy.Response RequestText(string text, string contentType = "text/plain;charset=utf-8")
        {
            Nancy.Responses.TextResponse res = new Nancy.Responses.TextResponse(HttpStatusCode.OK, text, Encoding.UTF8, this.Headers, this.Cookies);
            res.ContentType = contentType;
            return res;
        }

        public Nancy.Response RequestOK(object obj)
        {
            if (obj == null)
            {

                var rsp = new Nancy.Responses.TextResponse(HttpStatusCode.OK, "执行成功", Encoding.UTF8, this.Headers, this.Cookies);
                return rsp;
            }
            else if (obj is string)
            {
                return RequestText(obj.ToString());
            }
            else
            {
                StatusCode = HttpStatusCode.OK;
                Contents = (stream) =>
                {
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ConvertToJson(obj));
                    stream.Write(buffer, 0, buffer.Length);
                };

                return this;
            }
        }
        private string ConvertToJson(object obj)
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            return JsonConvert.SerializeObject(obj, Formatting.None, jsonSetting);
        }
    }
}
