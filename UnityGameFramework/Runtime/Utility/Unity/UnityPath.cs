//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System.IO;
using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    public static partial class Utility
    {
        /// <summary>
        /// Unity路径相关的实用函数。
        /// </summary>
        public static class UnityPath
        {
            /// <summary>
            /// 获取当前平台的StreamingAssetsPath目录
            /// 安卓平台特殊,返回Application.streamingAssetsPath
            /// </summary>
            /// <returns></returns>
            public static string GetStreamingAssetsPath()
            {
                //苹果 - windows 返回
#if UNITY_EDITOR||UNITY_STANDALONE_WIN || UNITY_IPHONE
                return "file://" + Application.streamingAssetsPath;
#else
                return UnityEngine.Application.streamingAssetsPath;
#endif
            }

            /// <summary>
            /// 获取当前平台的PersistentDataPath目录
            /// </summary>
            /// <returns></returns>
            public static string GetPersistentDataPath()
            {
                return UnityEngine.Application.persistentDataPath;
            }
        }
    }
}
