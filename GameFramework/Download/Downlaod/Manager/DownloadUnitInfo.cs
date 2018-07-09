//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月16日 18:02:26
//Assembly-CSharp

using System;

namespace Icarus.GameFramework.Download
{
    public class DownloadUnitInfo
    {
        /// <summary>
        /// 下载器
        /// </summary>
        public IDownload DownloadUtil;
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority;
        /// <summary>
        /// 下载地址
        /// </summary>
        public string Url;
        /// <summary>
        /// 文件保存路径
        /// </summary>
        public string SavePath;
        /// <summary>
        /// 文件名,默认为下载文件名
        /// </summary>
        public string FileName;
        /// <summary>
        /// 超时时间,毫秒
        /// </summary>
        public int TimeOut = 5000;
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount = 3;
        public int CurrentRetryCount { get; internal set; }

        /// <summary>
        /// 缓存过期天数,默认为 -1(永不过期)
        /// </summary>
        public int CacheExpiredDay { get; set; } = -1;
        /// <summary>
        /// 下载开始
        /// </summary>
        public Action<DownloadInfo> DownloadStartHandle;
        /// <summary>
        /// 下载中
        /// </summary>
        public Action<DownloadProgressInfo, string> DownloadProgressHandle;
        /// <summary>
        /// 下载完成
        /// </summary>
        public Action<string> CompleteHandle;
        /// <summary>
        /// 下载出错
        /// </summary>
        public Action<String> ErrorHandle;
        /// <summary>
        /// 是否查找缓存库,默认为查找
        /// </summary>
        public bool IsFindCacheLibrary = true;

        public override string ToString()
        {
            return $"下载地址:{Url},优先级:{Priority}";
        }
    }
}