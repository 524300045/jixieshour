using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StriveEngine.BinaryDemoServer
{
    public enum LogLevel
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UnKnown = 0,

        /// <summary>
        /// 处理DEBUG级别的日志
        /// </summary>
        [Description("处理DEBUG级别的日志")]
        Debug = 1,

        /// <summary>
        /// 处理INFO级别的日志
        /// </summary>
        [Description("处理INFO级别的日志")]
        Info = 2,

        /// <summary>
        /// 处理警告类的日志
        /// </summary>
        [Description("处理警告类的日志")]
        Warn = 3,

        /// <summary>
        /// 处理错误级别的日志
        /// </summary>
        [Description("处理错误级别的日志")]
        Error = 4,

        /// <summary>
        /// 处理致命错误级别的日志
        /// </summary>
        [Description("处理致命错误级别的日志")]
        Fatal = 5
    }

    public class LogHelper
    {
        private static readonly ILog ILog = LogManager.GetLogger("Log");
        private static readonly ILog ISendMail = LogManager.GetLogger("Email");
        /// <summary>
        /// 写日志，默认级别是 LogLevel.Error
        /// </summary>
        /// <param name="message">写日志的内容</param>
        public static void Log(string message)
        {
            Log(message, LogLevel.Error);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">写日志的内容</param>
        /// <param name="logLevel">日志级别</param>
        public static void Log(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    ILog.Debug(message);
                    break;
                case LogLevel.Info:
                    ILog.Info(message);
                    break;
                case LogLevel.Warn:
                    ILog.Warn(message);
                    break;
                case LogLevel.Error:
                    ILog.Error(message);
                    break;
                case LogLevel.Fatal:
                    ILog.Fatal(message);
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 写日志，默认日志级别是 LogLevel.Error。会根据异常的类型来判断是否发送通知邮件
        /// </summary>
        /// <param name="exception">异常信息</param>
        public static void Log(Exception exception)
        {
            Log(exception, LogLevel.Error);
        }

        /// <summary>
        /// 写日志，会根据异常的类型来判断是否发送通知邮件
        /// </summary>
        /// <param name="exception">异常信息</param>
        /// <param name="logLevel">日志级别</param>
        public static void Log(Exception exception, LogLevel logLevel)
        {
            // 写日志
            switch (logLevel)
            {
                case LogLevel.Debug:
                    ILog.Debug(exception.Message, exception);
                    break;
                case LogLevel.Info:
                    ILog.Info(exception.Message, exception);
                    break;
                case LogLevel.Warn:
                    ILog.Warn(exception.Message, exception);
                    break;
                case LogLevel.Error:
                    ILog.Error(exception.Message, exception);
                    break;
                case LogLevel.Fatal:
                    ILog.Fatal(exception.Message, exception);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="content"></param>
        public static void SendMail(string content)
        {
            try
            {
                ISendMail.Fatal(content);
            }
            catch (Exception)
            {
            }
        }
    }
}
