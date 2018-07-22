//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System.Diagnostics;

namespace Icarus.GameFramework
{
    /// <summary>
    /// 日志类。
    /// </summary>
    public static partial class Log
    {
        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="message">日志内容。</param>
        [Conditional("DEBUG")]
        public static void Debug(object message)
        {
            Instance.ShowLog(message);
        }

        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="message">日志内容。</param>
        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            Instance.ShowLog(message);
        }

        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0)
        {
            Instance.ShowLog(string.Format(format, arg0));
        }

        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0, object arg1)
        {
            Instance.ShowLog(string.Format(format, arg0, arg1));
        }

        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        /// <param name="arg2">日志参数 2。</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0, object arg1, object arg2)
        {
            Instance.ShowLog(string.Format(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args)
        {
            Instance.ShowLog(string.Format(format, args));
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(object message)
        {
            Instance.ShowLog(message, LogType.Info);
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            Instance.ShowLog(message, LogType.Info);
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        public static void Info(string format, object arg0)
        {
            Instance.ShowLog(string.Format(format, arg0), LogType.Info);
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        public static void Info(string format, object arg0, object arg1)
        {
            Instance.ShowLog(string.Format(format, arg0, arg1), LogType.Info);
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        /// <param name="arg2">日志参数 2。</param>
        public static void Info(string format, object arg0, object arg1, object arg2)
        {
            Instance.ShowLog(string.Format(format, arg0, arg1, arg2), LogType.Info);
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        public static void Info(string format, params object[] args)
        {
            Instance.ShowLog(string.Format(format, args), LogType.Info);
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public static void Warning(object message)
        {
            Instance.ShowLog(message, LogType.Warning);
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public static void Warning(string message)
        {
            Instance.ShowLog(message, LogType.Warning);
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        public static void Warning(string format, object arg0)
        {
            Instance.ShowLog(string.Format(format, arg0), LogType.Warning);
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        public static void Warning(string format, object arg0, object arg1)
        {
            Instance.ShowLog(string.Format(format, arg0, arg1), LogType.Warning);
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        /// <param name="arg2">日志参数 2。</param>
        public static void Warning(string format, object arg0, object arg1, object arg2)
        {
            Instance.ShowLog(string.Format(format, arg0, arg1, arg2), LogType.Warning);
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        public static void Warning(string format, params object[] args)
        {
            Instance.ShowLog(string.Format(format, args), LogType.Warning);
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public static void Error(object message)
        {
            Instance.ShowLog(message, LogType.Error);
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public static void Error(string message)
        {
            Instance.ShowLog(message, LogType.Error);
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        public static void Error(string format, object arg0)
        {
            Instance.ShowLog(string.Format(format, arg0), LogType.Error);
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        public static void Error(string format, object arg0, object arg1)
        {
            Instance.ShowLog(string.Format(format, arg0, arg1), LogType.Error);
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        /// <param name="arg2">日志参数 2。</param>
        public static void Error(string format, object arg0, object arg1, object arg2)
        {
            Instance.ShowLog(string.Format(format, arg0, arg1, arg2), LogType.Error);
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        public static void Error(string format, params object[] args)
        {
            Instance.ShowLog(string.Format(format, args), LogType.Error);
        }

        /// <summary>
        /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public static void Fatal(object message)
        {
            Instance.ShowLog(message, LogType.Exception);
        }

        /// <summary>
        /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public static void Fatal(string message)
        {
            Instance.ShowLog(message, LogType.Exception);
        }

        /// <summary>
        /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        public static void Fatal(string format, object arg0)
        {
            Instance.ShowLog(string.Format(format, arg0), LogType.Exception);
        }

        /// <summary>
        /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        public static void Fatal(string format, object arg0, object arg1)
        {
            Instance.ShowLog(string.Format(format, arg0, arg1), LogType.Exception);
        }

        /// <summary>
        /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        /// <param name="arg2">日志参数 2。</param>
        public static void Fatal(string format, object arg0, object arg1, object arg2)
        {
            Instance.ShowLog(string.Format(format, arg0, arg1, arg2), LogType.Exception);
        }

        /// <summary>
        /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        public static void Fatal(string format, params object[] args)
        {
            Instance.ShowLog(string.Format(format, args), LogType.Exception);
        }
    }
}
