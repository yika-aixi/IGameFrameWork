//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using Icarus.GameFramework.Event;

namespace Icarus.UnityGameFramework.Runtime
{
    /// <summary>
    /// 多资源加载完成事件。
    /// </summary>
    public sealed class LoadAssetsCompleteEventArgs : GameEventArgs
    {
        /// <summary>
        /// 多资源加载完成事件编号。
        /// </summary>
        public static readonly int EventId = typeof(LoadAssetsCompleteEventArgs).GetHashCode();

        /// <summary>
        /// 获取多资源加载完成事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
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

        /// <summary>
        /// 清理加载场景成功事件。
        /// </summary>
        public override void Clear()
        {
            AssetNames = null;
            Assets = null;
            Duration = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载场景成功事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>加载场景成功事件。</returns>
        public LoadAssetsCompleteEventArgs Fill(GameFramework.Resource.LoadAssetsCompleteEventArgs e)
        {
            AssetNames = e.AssetNames;
            Assets = e.Assets;
            Duration = e.Duration;
            UserData = e.UserData;

            return this;
        }
    }
}
