using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace detection.controller.nancyBase
{
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Responses;
    using Newtonsoft.Json.Linq;

    public class BaseModule : NancyModule
    {
        /// <summary>
        /// 平台API 
        /// </summary>
        public BaseModule(string modulePath)
            : base("/api" + modulePath)
        {

        }

        public BaseModule()
        {
            
        }
        #region 公共代码
        public T BindData<T>(bool fromBody = false)
        {
            if (this.Request.Form.Count > 0 && !fromBody)
            {
                return this.Bind<T>();
            }
            else
            {
                var jsonText = new System.IO.StreamReader(this.Request.Body).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonText);
            }
        }

        public T BindData<T>(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
            return default(T);
        }

        public DateTime ConvertStringToDate(string dateString, DateTime defaultDate)
        {
            if (!string.IsNullOrEmpty(dateString))
            {
                try
                {
                    return DateTime.Parse(dateString);
                }
                catch (Exception)
                {
                    throw new Exception("参数:时间字符串格式不正确");
                }
            }
            return defaultDate;
        }

        protected string ValidPagination(int curPageIndx, int perPageSize)
        {
            if (curPageIndx < 0)
            {
                return "参数:pageIndex小于0";
            }

            if (perPageSize <= 0)
            {
                return "参数:pageSize小于等于0";
            }

            return null;
        }

        public Nancy.Response RequestOK(object obj = null)
        {
            return new NancyJsonResponse(methods: Request.Method).RequestOK(obj);
        }

        public Nancy.Response RequestText(string text, string contentType = "text/plain")
        {
            return new NancyJsonResponse(methods: Request.Method, contentType: contentType).RequestText(text);
        }

        public Nancy.Response RequestFailure(object errorObject)
        {
            return new NancyJsonResponse(methods: Request.Method).RequestFailure(errorObject);
        }
        #endregion
    }
}
