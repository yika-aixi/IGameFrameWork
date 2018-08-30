using Icarus.GameFramework;
using Icarus.GameFramework.Download;
using Icarus.GameFramework.UpdateAssetBundle;
using Icarus.GameFramework.Version;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Application = UnityEngine.Application;

namespace Icarus.UnityGameFramework.Runtime
{
    /// <summary>
    /// 使用UnityWebRequeDownload来更新
    /// </summary>
    [AddComponentMenu("Icarus/Game Framework/UpdateAssetBundle")]
    public class DefaultUpdateAssetBundleComponent: GameFrameworkComponent, IUpdateAssetBundle
    {
        public DownloadManager DownloadManager { get; private set; }
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
            UpdateAssetBundle(updateInfo, null, assetBundleifInfos, persistentInfos, version, progressHandle, anyCompleteHandle,
                allCompleteHandle, errorHandle);
        }

        public void UpdateAssetBundle(UpdateInfo updateInfo, Dictionary<string, string> headers, 
            IEnumerable<AssetBundleInfo> assetBundleifInfos,
            VersionInfo persistentInfos, string version, GameFrameworkAction<DownloadProgressInfo, string> progressHandle = null,
            GameFrameworkAction<AssetBundleInfo> anyCompleteHandle = null, GameFrameworkAction allCompleteHandle = null,
            GameFrameworkAction<string> errorHandle = null)
        {
            DownloadManager.AllCompleteHandle = () =>
            {
                persistentInfos.SetVersion(version);

                _updateVersionInfoFile(persistentInfos);
                allCompleteHandle?.Invoke();
            };
            List<DownloadUnitInfo> downloadUnitInfos = new List<DownloadUnitInfo>();
            foreach (var assetBundleInfo in assetBundleifInfos)
            {
                var downloadTools = new UnityWebRequestDownload(_coroutine);
                if (headers != null)
                {
                    var unityWeb = (UnityWebRequest) downloadTools.DownlodTool;

                    foreach (var pair in headers)
                    {
                        unityWeb.SetRequestHeader(pair.Key,pair.Value);
                    }
                }
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
                    Url = updateInfo.AssetBundleUrl + "/" + assetBundleInfo.PackFullName.Replace("dat", "zip"),
                    IsFindCacheLibrary = false, //ab包更新不找缓存
                    DownloadUtil = downloadTools,
                    Headers = headers

                });
            }

            DownloadManager.AddRangeDownload(downloadUnitInfos);
        }

        private void _updateVersionInfoFile(VersionInfo persistentInfos)
        {
            var by = persistentInfos.JiaMiSerialize();
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath,ConstTable.VersionFileName), by);
        }

        protected virtual void Start()
        {
            DownloadManager = new DownloadManager();
            DownloadManager.Init();
        }

    }
}