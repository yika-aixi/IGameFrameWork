//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年03月11日 23:14:54
//Client

using System;

namespace Icarus.GameFramework
{
    public static partial class Log
    {
        private static ILog _logInstance;

        public static ILog Instance
        {
            get
            {
                if (_logInstance == null)
                {
                    throw new NullReferenceException("no excute SetLog()");
                }
                return _logInstance;
            }
        }

        public static bool HasLog()
        {
            return _logInstance != null;
        }

        public static void SetLog(ILog log)
        {
            _logInstance = log;
        }

        /// <summary>
        /// 显示log,关于Log的设置使用
        /// Instance来进行设置
        /// </summary>
        /// <param name="message"></param>
        public static void ShowLog(object message)
        {
            Instance.ShowLog(message);
        }

        /// <summary>
        /// 同步将Log写入文件
        /// </summary>
        /// <param name="log">需要写入的日志</param>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="errorMessage">写入错误信息</param>
        /// <param name="extensionName">扩展名,默认为:"IcarusLog"</param>
        /// <returns>是否写入成功,成功: True 失败: False</returns>
        public static bool WritingFile(string log,string path,string fileName,
            out string errorMessage,string extensionName = "IcarusLog")
        {
            var by = Utility.Converter.GetBytes(log);
            return Utility.FileUtil.WritingFile(by,path,fileName,extensionName,out errorMessage);

        }
        
        /// <summary>
        /// 异步将Log写入文件
        /// 如果频繁向一个文件写入并且isAppend为true将会等待所有的写入完成才会调用completeHandle,失败回调不受影响
        /// </summary>
        /// <param name="log">需要写入的日志</param>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="extensionName">扩展名,默认为:"IcarusLog"</param>
        /// <param name="isAppend">写入追加(True : 全部写入完成才会调用completeHandle , False : 完成一次写入就调用completeHandle)</param>
        /// <param name="completeHandle">成功回调,参数为:文件路径</param>
        /// <param name="errorHandle">失败回调,参数为:失败信息</param>
        /// <returns>是否写入成功,成功: True 失败: False</returns>
        public static void WritingFileAsync(string log, string path, string fileName, string extensionName = "IcarusLog",bool isAppend = false, Action<string> completeHandle = null, Action<string> errorHandle = null)
        {
            var by = Utility.Converter.GetBytes(log);
            Utility.FileUtil.WritingFileAsync(by,path,fileName,extensionName, isAppend,completeHandle:completeHandle,errorHandle:errorHandle);
        }
    }
}