//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年03月06日 21:18:27
//Client

namespace Icarus.GameFramework
{
    public interface ILog
    {
        /// <summary>
        /// 是否启用Log
        /// </summary>
        bool Enable { get; set; }
        
        /// <summary>
        /// Enable 为 false 时关闭哪些类型的log
        /// </summary>
        CloseLevel CloseLevel { get; set; }

        /// <summary>
        /// 显示log
        /// </summary>
        /// <param name="message">log 消息</param>
        /// <param name="type">Log类型</param>
        void ShowLog(object message, LogType type = LogType.Debug);

        /// <summary>
        /// 显示log
        /// </summary>
        /// <param name="message">log 消息</param>
        /// <param name="type">Log类型</param>
        void ShowLog(string message, LogType type = LogType.Debug);
    }

    /// <summary>
    /// log类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 普通log
        /// </summary>
        Debug,
        /// <summary>
        /// 信息log
        /// </summary>
        Info,
        /// <summary>
        /// 警告log
        /// </summary>
        Warning,
        /// <summary>
        /// 错误log
        /// </summary>
        Error,
        /// <summary>
        /// 异常log
        /// </summary>
        Exception
    }

    /// <summary>
    /// 关闭等级
    /// </summary>
    public enum CloseLevel
    {
        /// <summary>
        /// 关闭所有 
        /// </summary>
        All,
        /// <summary>
        /// 关闭 普通log
        /// </summary>
        Log,
        /// <summary>
        ///  关闭 普通log 和 警告log
        /// </summary>
        Warning,
        /// <summary>
        /// 除了异常log 外全部关闭
        /// </summary>
        Error

    }
}