using Icarus.GameFramework;
using Icarus.GameFramework.Download;
using Icarus.GameFramework.UpdateAssetBundle;
using Icarus.GameFramework.Version;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Application = UnityEngine.Application;

namespace Icarus.UnityGameFramework.Runtime
{
    /// <summary>
    /// 使用UnityWebRequeDownload来更新
    /// </summary>
    [AddComponentMenu("Icarus/Game Framework/UpdateAssetBundle")]
    public class DefaultUpdateAssetBundleComponent: UnityGameFrameWorkBehaviour, IUpdateAssetBundle
    {
        private DownloadManager _downloadManager;
        [SerializeField]
        private CoroutineManager _coroutine;
        public void UpdateAssetBundle(UpdateInfo updateInfo, IEnumerable<AssetBundleInfo> assetBundleifInfos, 
            VersionInfo persistentInfos,
            string version,
            GameFrameworkAction<DownloadProgressInfo, string> progressHandle = null,
            GameFrameworkAction<AssetBundleInfo> anyCompleteHandle = null,
            GameFrameworkAction allCompleteHandle = null,
            GameFrameworkAction<string> errorHandle = null)
        {
            _downloadManager.AllCompleteHandle = ()=>
            {
                persistentInfos.SetVersion(version);

                _updateVersionInfoFile(persistentInfos);
                allCompleteHandle?.Invoke();
            };
            List<DownloadUnitInfo> downloadUnitInfos = new List<DownloadUnitInfo>();
            foreach (var assetBundleInfo in assetBundleifInfos)
            {
                downloadUnitInfos.Add(new DownloadUnitInfo()
                {
                    CompleteHandle = x =>
                    {
                        persistentInfos.AddOrUpdateAssetBundleInfo(assetBundleInfo);

                        _updateVersionInfoFile(persistentInfos);

                        anyCompleteHandle?.Invoke(assetBundleInfo);
                        //解压
                        Utility.ZipUtil.UnzipZip(x, Application.persistentDataPath);
                        GameFramework.Utility.FileUtil.DeleteFile(x);
                    },
                    ErrorHandle = errorHandle.Invoke,
                    DownloadProgressHandle = progressHandle.Invoke,
                    FileName = assetBundleInfo.PackName.Replace("dat", "zip"),
                    SavePath = Path.Combine(Application.persistentDataPath, assetBundleInfo.PackPath),
                    Url = updateInfo.AssetBundleUrl+"/"+ assetBundleInfo.PackFullName.Replace("dat","zip"),
                    IsFindCacheLibrary = false, //ab包更新不找缓存
                    DownloadUtil = new UnityWebRequestDownload(_coroutine)

                });
            }

            _downloadManager.AddRangeDownload(downloadUnitInfos);
        }

        private void _updateVersionInfoFile(VersionInfo persistentInfos)
        {
            var by = persistentInfos.JiaMiSerialize();
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath,ConstTable.VersionFileName), by);
        }

        protected virtual void Start()
        {
            _downloadManager = new DownloadManager();
            _downloadManager.Init();
        }

    }
}