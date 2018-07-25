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
        /// 严格模式 - 对比文件md5
        /// 不严格模式 - 只对比文件记录得md5
        /// </summary>
        bool StrictMode { get; set; }

        /// <summary>
        /// 服务器version.info
        /// </summary>
        VersionInfo ServerVersionInfo { get; }

        /// <summary>
        /// 本地的最新版本信息
        /// </summary>
        VersionInfo LocalVersionInfo { get; }

        /// <summary>
        /// 获取持久化目录版本信息
        /// </summary>
        VersionInfo PersistentInfos { get;}

        /// <summary>
        /// 开始检查
        /// </summary>
        /// <paramref name="checkAssetBundleInfoCompleteHandle">参数1：更新列表，参数2：本地版本信息文件</paramref>
        /// <returns>所有需要更新的包</returns>
        void Check(
            GameFrameworkAction<IEnumerable<AssetBundleInfo>> checkAssetBundleInfoCompleteHandle = null,
            GameFrameworkFunc<string> getAppUpdateUrl = null,
            GameFrameworkAction<string> errorHandle = null,GameFrameworkAction<string> stateUpdateHandle = null);

        /// <summary>
        /// 开始检查
        /// </summary>
        /// <paramref name="checkAssetBundleInfoCompleteHandle">参数1：更新列表，参数2：本地版本信息文件</paramref>
        /// <returns>所有需要更新的包</returns>
        void Check(Dictionary<string,string> headers,
            GameFrameworkAction<IEnumerable<AssetBundleInfo>> checkAssetBundleInfoCompleteHandle = null,
            GameFrameworkFunc<string> getAppUpdateUrl = null,
            GameFrameworkAction<string> errorHandle = null, GameFrameworkAction<string> stateUpdateHandle = null);

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