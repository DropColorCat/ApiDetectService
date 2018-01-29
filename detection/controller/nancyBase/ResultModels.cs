using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace detection.controller.nancyBase
{
    /// <summary>
    /// 处理结果编码
    /// </summary>
    public class ResultCodeValue
    {
        public static readonly string ResultOK = "OK";
        public static readonly string UserNameNotMatch = "UserNameNotMatch";
        public static readonly string PasswordeNotMatch = "PasswordeNotMatch";
        public static readonly string OperationFailed = "OperationFailed";
        public static readonly string ExceptionFailed = "ExceptionFailed";
        public static readonly string ArgumentNull = "ArgumentNull";
        public static readonly string InvalidArgument = "InvalidArgument";
        public static readonly string InvalidArgumentType = "InvalidArgumentType";
        public static readonly string InvalidFormat = "InvalidFormat"; 
        public static readonly string NoObject = "NoObject";
        public static readonly string NoNecessaryData = "NoNecessaryData";
        public static readonly string FileFailed = "FileFailed";
        public static readonly string HeavyLog = "HeavyLog";
    }
    /// <summary>
    /// api返回信息框架
    /// </summary>
    public class ResultModel
    {
        /// <summary>
        /// 构造函数,默认初始化Code为：OK Description为：处理成功
        /// </summary>
        public ResultModel()
        {
            this.Code = ResultCodeValue.ResultOK;
            this.Message = "执行成功";
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="des"></param>
        public ResultModel(string code,string des)
        {
            this.Code = code;
            this.Message = des;
        }
        /// <summary>
        /// 错误码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 错误码描述
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }
    }
}
