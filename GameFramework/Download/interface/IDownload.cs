//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月09日 17:19:51
//Assembly-CSharp

using System;

namespace Icarus.GameFramework.Download
{
    public interface IDownload:IDisposable
    {
        /// <summary>
        /// 当前下载器超时时间
        /// </summary>
        int TimeOut { get; set; }
        /// <summary>
        /// 当前Url
        /// </summary>
        string Url { get; }
        /// <summary>
        /// 当前下载器
        /// </summary>
        object DownlodTool { get; }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="savePath">文件保存路径</param>
        /// <param name="fileName">文件名,默认为远程文件名</param>
        /// <param name="downloadStartHandle">下载开始</param>
        /// <param name="downloadProgressHandle">下载中,参数:下载进度(0-1),描述</param>
        /// <param name="completeHandle">下载成功,参数:文件保存路径</param>
        /// <param name="errorHandle">下载出错,参数:错误信息</param>
        void Download(string url,string savePath,string fileName = "",Action<DownloadInfo> downloadStartHandle = null, Action<DownloadProgressInfo,string> downloadProgressHandle = null, Action<string> completeHandle = null, Action<String> errorHandle = null);

    }
}