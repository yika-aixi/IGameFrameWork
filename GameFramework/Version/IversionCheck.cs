using System.Collections.Generic;

namespace Icarus.GameFramework.Version
{
    public interface IVersionCheck
    {
        /// <summary>
        /// version.info远程地址
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// 服务器version.info
        /// </summary>
        VersionInfo ServerVersionInfo { get; }

        /// <summary>
        /// 本地的最新版本信息
        /// </summary>
        VersionInfo LocalVersionInfo { get; }

        /// <summary>
        /// 开始检查
        /// </summary>
        /// <paramref name="completeHandle">参数1：更新列表，参数2：本地版本信息文件</paramref>
        /// <returns>所有需要更新的包</returns>
        void Check(bool strictMode = true,GameFrameworkAction<IEnumerable<AssetBundleInfo>> completeHandle = null,GameFrameworkAction<string> errorHandle = null,GameFrameworkAction<string> stateUpdateHandle = null);

        /// <summary>
        /// 获取指定资源组的更新列表
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        IEnumerable<AssetBundleInfo> GetGroupVersion(string tag);

        /// <summary>
        /// 指定组是否需要更新
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        bool IsUpdateGroup(string tag);

    }
}