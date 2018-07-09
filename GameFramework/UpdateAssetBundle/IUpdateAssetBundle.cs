using Icarus.GameFramework.Version;
using System.Collections.Generic;
using Icarus.GameFramework.Download;

namespace Icarus.GameFramework.UpdateAssetBundle
{
    public interface IUpdateAssetBundle
    {
        /// <summary>
        /// 更新资源包
        /// </summary>
        /// <param name="updateInfo">更新信息</param>
        /// <param name="assetBundleifInfos">更新列表</param>
        /// <param name="persistentInfos">persistent的versionInfo</param>
        /// <param name="version">版本号</param>
        /// <param name="progressHandle">更新进度</param>
        /// <param name="anyCompleteHandle">更新好一个</param>
        /// <param name="allCompleteHandle">全部更新完成</param>
        /// <param name="errorHandle">更新出错</param>
        void UpdateAssetBundle(UpdateInfo updateInfo,IEnumerable<AssetBundleInfo> assetBundleifInfos, 
            VersionInfo persistentInfos,
            string version,
            GameFrameworkAction<DownloadProgressInfo, string> progressHandle = null,
            GameFrameworkAction<AssetBundleInfo> anyCompleteHandle = null, 
            GameFrameworkAction allCompleteHandle = null,
            GameFrameworkAction<string> errorHandle = null);
    }
}