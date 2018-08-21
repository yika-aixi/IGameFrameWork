//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;

namespace Icarus.GameFramework.Resource
{
    /// <summary>
    /// 多资源加载完成事件。
    /// </summary>
    public sealed class LoadAssetsCompleteEventArgs : GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化多资源加载完成事件的新实例。
        /// </summary>
        /// <param name="assetNames">资源名集合</param>
        /// <param name="assets">资源集合</param>
        /// <param name="duration">加载持续时间。</param>
        /// <param name="userData">用户自定义数据。</param>
        public LoadAssetsCompleteEventArgs(IEnumerable<string> assetNames, IEnumerable<object> assets, float duration, object userData)
        {
            AssetNames = assetNames;
            Assets = assets;
            Duration = duration;
            UserData = userData;
        }


        /// <summary>
        /// 加载成功的资源名,失败的不会在其中
        /// </summary>
        public IEnumerable<string> AssetNames { get; private set; }

        /// <summary>
        /// 加载成功的资源,失败的不会在其中
        /// </summary>
        public IEnumerable<object> Assets { get; private set; }

        /// <summary>
        /// 获取加载持续时间。
        /// </summary>
        public float Duration
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }
    }
}
