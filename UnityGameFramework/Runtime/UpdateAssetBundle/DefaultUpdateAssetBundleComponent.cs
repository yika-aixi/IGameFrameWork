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
    [AddComponentMenu("Icarus/Game Framework/Default Download")]
    public class DefaultUpdateAssetBundleComponent:MonoBehaviour,IUpdateAssetBundle
    {
        public DownloadManager DownloadManager;
        public CoroutineManager Coroutine;
        private string VersionInfoFileName = "version.info";
        public void UpdateAssetBundle(UpdateInfo updateInfo, IEnumerable<AssetBundleInfo> assetBundleifInfos, 
            VersionInfo persistentInfos,
            string version,
            GameFrameworkAction<DownloadProgressInfo, string> progressHandle = null,
            GameFrameworkAction<AssetBundleInfo> anyCompleteHandle = null,
            GameFrameworkAction allCompleteHandle = null,
            GameFrameworkAction<string> errorHandle = null)
        {
            int appVersion = -1;

            try
            {
                appVersion = int.Parse(Application.version.Split('.').Last());
            }
            catch (Exception e)
            {
                errorHandle?.Invoke($"请确保 Edit-->Project Settings-->Player --> Other Setting 下的 Version 字段‘.’分割的最后一位是int值，如：0.1.1s.2,‘2’就是我默认的规则");
                return;
            }
            if (appVersion < updateInfo.MinAppVersion)
            {
                Application.OpenURL(updateInfo.AppUpdateUrl);
                return;
            }
            DownloadManager.AllCompleteHandle = ()=>
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
                    DownloadUtil = new UnityWebRequestDownload(Coroutine)

                });
            }

            DownloadManager.AddRangeDownload(downloadUnitInfos);
        }

        private void _updateVersionInfoFile(VersionInfo persistentInfos)
        {
            var by = persistentInfos.JiaMiSerialize();
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, VersionInfoFileName), by);
        }

        protected virtual void Awake()
        {
            GameEntry.RegisterComponent(this);
        }

        protected virtual void Start()
        {
            DownloadManager = new DownloadManager();
            DownloadManager.Init();
        }

    }
}