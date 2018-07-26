//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年03月06日 21:32:19
//Client
using System;
using Icarus.GameFramework;
using UnityEngine;
using LogType = Icarus.GameFramework.LogType;

namespace Icarus.UnityGameFramework.Runtime
{

    public class UnityLog : ILog
    {
        /// <summary>
        /// 默认为True
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 默认为 Warning
        /// </summary>
        public CloseLevel CloseLevel { get; set; }
        public void ShowLog(object message, LogType type = LogType.Debug)
        {
            _showLog(message, type);

        }

        public void ShowLog(string message, LogType type = LogType.Debug)
        {
            _showLog(message, type);
        }

        private void _showLog(object message, LogType type)
        {
            switch (type)
            {
                case LogType.Debug:
                    _log(message,true);
                    break;
                case LogType.Info:
                    _log(message,false);
                    break;
                case LogType.Warning:
                    _warning(message);
                    break;
                case LogType.Error:
                    _error(message);
                    break;
                case LogType.Exception:
                    _exception(message);
                    break;
            }
        }

        private void _exception(object message)
        {
            if (Enable | (!Enable && CloseLevel != CloseLevel.All))
            {
                Debug.LogException(new Exception(message.ToString()));
            }
        }

        private void _error(object message)
        {
            if (Enable | (!Enable && CloseLevel < CloseLevel.Error))
            {
                Debug.LogError(message);
            }
        }

        private void _warning(object message)
        {
            if (Enable | (!Enable && CloseLevel < CloseLevel.Warning))
            {
                Debug.LogWarning(message);
            }
        }

        private void _log(object message,bool isDebug)
        {
            if (Enable)
            {
                if (isDebug)
                {
                    Debug.Log($"<color=#888888>{message}</color>");
                    return;
                }
                Debug.Log(message);
            }
        }

        public UnityLog():this(true,CloseLevel.Warning){}

        public UnityLog(bool enable):this(enable,CloseLevel.Warning){}

        public UnityLog(bool enable,CloseLevel level)
        {
            Enable = enable;
            CloseLevel = level;
        }


    }
}
