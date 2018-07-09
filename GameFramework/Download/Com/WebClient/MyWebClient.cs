//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月13日 22:39:15
//Assembly-CSharp

using System;
using System.Net;

namespace Icarus.GameFramework.Download
{
    public class MyWebClient:WebClient
    {
        public int TimeOut;
        public ulong DownloadLenght;
        public ulong TotalLength;

        protected override WebRequest GetWebRequest(Uri uri)
        {
            var webRequest = (HttpWebRequest)base.GetWebRequest(uri);

            if (webRequest != null)
            {
                webRequest.Timeout = TimeOut;
                webRequest.ReadWriteTimeout = TimeOut;
                if (DownloadLenght > 0)
                {
                    UnityEngine.Debug.Log($"继续下载:{DownloadLenght},{TotalLength}");
                    webRequest.AddRange((long) DownloadLenght,(long)TotalLength);
//                    webRequest.Headers.Add("Range", $"bytes={DownloadLenght}-{TotalLength}");
                }
            }

            return webRequest;
        }
    }
}