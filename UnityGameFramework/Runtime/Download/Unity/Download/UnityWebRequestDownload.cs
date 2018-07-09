//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月19日 0:44:49
//Assembly-CSharp

using System.Collections;
using Icarus.GameFramework.Download;
using UnityEngine;
using UnityEngine.Networking;

namespace Icarus.UnityGameFramework.Runtime
{
    public class UnityWebRequestDownload : DownloadBase
    {
        public override object DownlodTool => webRequest;

        private UnityWebRequest webRequest = new UnityWebRequest();
        private StreamDownloadHandle streamDownload = new StreamDownloadHandle();
        private CoroutineManager manager;

        public UnityWebRequestDownload(CoroutineManager coroutineManager)
        {
            manager = coroutineManager;
        }

        protected override void Download()
        {
            webRequest.url = Url;
            webRequest.timeout = TimeOut;
            streamDownload.ErrorHandle = ErrorHandle;
            streamDownload.FilePath = SaveFilePathTemp;
            webRequest.downloadHandler = streamDownload;
            //            webRequest.chunkedTransfer = true;
            if (IsContinueDownload)
            {
                webRequest.SetRequestHeader("Range", $"bytes={DownloadCacheFileLenght}-{ServeLength}");
            }
            manager.Start_Coroutine(_download());
        }

        private ulong oldLenght = 0;
        private IEnumerator _download()
        {
            //避免下载完了,但是生成文件问题
            if (DownloadCacheFileLenght >= ServeLength)
            {
                Complete();
                yield break;
            }

            var result = webRequest.SendWebRequest();
            if (IsContinueDownload)
            {
                while (webRequest.responseCode != 206)
                {
                    Debug.LogError($"无法继续下载,本地大小:{DownloadCacheFileLenght}," +
                                   $"文件总大小:{ServeLength}");
                    yield return null;
                }
            }
            while (!result.isDone)
            {
                DownloadInfo.SetDownloadLenght(DownloadCacheFileLenght + webRequest.downloadedBytes);
                ProgressHandle?
                    .Invoke(
                           new DownloadProgressInfo(webRequest.downloadedBytes - oldLenght,
                           GetProgress(webRequest.downloadedBytes), DownloadInfo),
                           DownloadMessageCode.GetMessage((int)DownloadMessageCodeTable.下载中));
                oldLenght = webRequest.downloadedBytes;
                yield return null;
            }

            if (!string.IsNullOrEmpty(webRequest.error))
            {
                ErrorHandle?.Invoke($"下载失败.\n" +
                                   $"错误信息为:{webRequest.error}\n" +
                                   $"网络状态码:{webRequest.responseCode}");
                yield break;
            }

            Complete();
        }

        public override void Dispose()
        {
            base.Dispose();
            webRequest.Abort();
        }
    }
}