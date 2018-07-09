//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月13日 7:15:06
//Assembly-CSharp

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Icarus.GameFramework.Download
{
    /// <summary>
    /// 在Unity使用 报  Invalid certificate received from server. Error code: 0xffffffff800b010a 错误的话
    /// 加入
    ///     #if UNITY
    ///         ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
    ///     #endif
    /// 会出现WSACancelBlockingCall错误
    /// </summary>
    public abstract class DownloadBase : IDownload
    {
        public int TimeOut { get; set; } = 5000;

        public string Url { get; protected set; }
        public abstract object DownlodTool { get; }
        protected string ContentMD5HeaderName => "Content-MD5";
        protected string ContentLengthHeaderName => "Content-Length";
        protected string ContentDispositionHeaderName => "Content-Disposition";
        /// <summary>
        /// 远程文件信息记录文件路径
        /// 该文件格式为:
        ///     第一行为下载文件的大小
        ///     第二行为文件的md5码 -- 可能是空的
        /// </summary>
        protected string ServerFileInfoFilePath => $"{SavePath}/{SaveFileName}_IcarusTemp";
        protected ulong ServerFileInfoTotalLength { get; private set; }
        protected string ServerFileInfoMd5 { get; private set; }

        protected ulong ServeLength { get; private set; }
        protected string ServerMd5 { get; private set; }
        protected string ServerFileName { get; private set; }

        protected string SavePath { get; private set; }
        protected string SaveFileName { get; private set; }
        protected string SaveFilePath => $"{SavePath}/{SaveFileName}";
        protected string SaveFilePathTemp => $"{SavePath}/{SaveFileName}_temp";

        protected ulong DownloadCacheFileLenght { get; set; }

        protected bool IsContinueDownload { get; private set; }

        protected Action<DownloadInfo> DownloadStartHandle { get; private set; }

        protected Action<DownloadProgressInfo, string> ProgressHandle { get; private set; }

        protected Action<string> CompleteHandle { get; private set; }

        protected Action<string> ErrorHandle { get; private set; }

        protected DownloadInfo DownloadInfo = default(DownloadInfo);
        public virtual void Download(string url, string savePath, string fileName = "", Action<DownloadInfo> downloadStartHandle = null, Action<DownloadProgressInfo, string> downloadProgressHandle = null, Action<string> completeHandle = null, Action<string> errorHandle = null)
        {
            Url = url;
            if (Path.HasExtension(savePath))
            {
                SavePath = Path.GetDirectoryName(savePath);
            }
            else
            {
                SavePath = savePath;
            }
            SaveFileName = fileName;
            DownloadStartHandle = downloadStartHandle;
            ProgressHandle = downloadProgressHandle;
            CompleteHandle = completeHandle;
            ErrorHandle = errorHandle;
            _updateServerFileInfo();     
        }

        /// <summary>
        /// 下载完成调用,处理文件和调用CompleteHandle
        /// </summary>
        protected void Complete()
        {
            Utility.FileUtil.MoveFile(SaveFilePathTemp, SaveFilePath);
            Utility.FileUtil.DeleteFile(ServerFileInfoFilePath);
            CompleteHandle?.Invoke(SaveFilePath);
        }

        protected abstract void Download();


        private async void _updateServerFileInfo()
        {
            ProgressHandle?.Invoke(new DownloadProgressInfo(0),
                            DownloadMessageCode.GetMessage((int)DownloadMessageCodeTable.开始确认远程));
#if UNITY
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
#endif
            MyWebClient client = new MyWebClient();
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(Url));
            request.Timeout = TimeOut;
            WebResponse response;
            try
            {
                response = await request.GetResponseAsync();
            }
            catch (WebException webEx)
            {
                ErrorHandle?.Invoke($"确认远程文件时出现网络错误.\n" +
                                    $"响应状态为:{webEx.Status}\n" +
                                    $"错误信息:{webEx.Message}\n" +
                                    $"堆栈信息:{webEx.StackTrace}");
                return;
            }
            catch (Exception e)
            {
                ErrorHandle?.Invoke($"确认远程文件时出现错误.\n" +
                                    $"错误信息:{e.Message}\n" +
                                    $"堆栈信息:{e.StackTrace}");
                return;
            }
            var contentLength = response.Headers.Get(ContentLengthHeaderName);
            if (string.IsNullOrEmpty(contentLength))
            {
                ErrorHandle?.Invoke($"确认远程文件失败了,没有获取到文件大小.");
                return;
            }
            else
            {
                long lenght;
                long.TryParse(contentLength, out lenght);
                ServeLength = (ulong)lenght;
            }
            
            var p = Url.Split('/', '\\');
            ServerFileName = p.Last();

            ServerMd5 = client.Headers.Get(ContentMD5HeaderName);
            response.Close();
            response.Dispose();
            request.Abort();

            if (string.IsNullOrEmpty(SaveFileName))
            {
                SaveFileName = ServerFileName;
            }

            ReadServerFileInfoFile();
            DownloadCacheFileLenght = GetDownloadCacheFileLenght();
            CheckIsContinueDownload();

            if (_updateServerFile())
            {
                DownloadInfo.SetTotalLength(ServeLength);
                DownloadInfo.SetDownloadLenght(DownloadCacheFileLenght);
                if (IsContinueDownload)
                {
                    //继续下载
                    ProgressHandle?.Invoke(new DownloadProgressInfo(1),
                            DownloadMessageCode.GetMessage((int)DownloadMessageCodeTable.确认结束_继续下载));
                }
                else
                {
                    Utility.FileUtil.DeleteFile(SaveFilePath);
                    Utility.FileUtil.DeleteFile(SaveFilePathTemp);
                    if (DownloadCacheFileLenght == 0)
                    {
                        //开始下载
                        ProgressHandle?.Invoke(new DownloadProgressInfo(1),
                            DownloadMessageCode.GetMessage((int)DownloadMessageCodeTable.确认结束_开始下载));
                    }
                    else
                    {
                        //重新下载
                        ProgressHandle?.Invoke(new DownloadProgressInfo(1),
                            DownloadMessageCode.GetMessage((int)DownloadMessageCodeTable.确认结束_重新下载));
                    }
                }
                DownloadStartHandle?.Invoke(DownloadInfo);
                Download();
            }
        }

        private bool _updateServerFile()
        {
            //不能继续下载就更新服务器文件信息记录文件
            if (!IsContinueDownload)
            {
                string cacheStr = $"{ServeLength}\n{ServerMd5}";
                var cacheInfoBy = System.Text.Encoding.UTF8.GetBytes(cacheStr);
                string error;
                var result = Utility.FileUtil.WritingFile(cacheInfoBy, ServerFileInfoFilePath, out error, fileMode: FileMode.Create);
                if (!result)
                {
                    ErrorHandle?.Invoke(error);
                }
                return result;
            }
            return true;
        }


#if UNITY || UNITY_2017_1_OR_NEWER
        public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }
#endif
        /// <summary>
        /// 读取服务器文件信息记录文件数据
        /// </summary>
        /// <returns>True:读取到,False:没读取到</returns>
        protected bool ReadServerFileInfoFile()
        {
            //如果存在信息文件
            if (Utility.FileUtil.IsFileExists(ServerFileInfoFilePath))
            {
                //获取信息文件第一行
                var contentStr = Utility.FileUtil.GetFileLinesContentUTF8(ServerFileInfoFilePath, 1);
                ServerFileInfoTotalLength = StrToulong(contentStr);
                ServerFileInfoMd5 = Utility.FileUtil.GetFileLinesContentUTF8(ServerFileInfoFilePath, 2);
                return true;
            }

            return false;
        }

        protected ulong StrToulong(string content)
        {
            ulong lenght;
            ulong.TryParse(content, out lenght);
            return lenght;
        }

        /// <summary>
        /// 获取下载文件缓存的大小
        /// </summary>
        /// <returns></returns>
        protected ulong GetDownloadCacheFileLenght()
        {
            //获取文件下载的长度
            return (ulong)Utility.FileUtil.GetFileLenght(SaveFilePathTemp);
        }

        /// <summary>
        /// 检查是否可以继续下载
        /// </summary>
        /// <returns></returns>
        protected void CheckIsContinueDownload()
        {
            if (ServeLength == ServerFileInfoTotalLength && DownloadCacheFileLenght != 0)
            {
                IsContinueDownload = true;
            }

            if (!string.IsNullOrEmpty(ServerMd5))
            {
                IsContinueDownload = ServerMd5.Equals(ServerFileInfoMd5);
            }
        }

        protected Thread TimeThread { get; private set; }
        protected ulong Time { get; private set; }
        protected void StartTime()
        {
            ThreadStart timeFun = TimeFun;
            TimeThread = new Thread(timeFun);
            TimeThread.Start();
        }

        private void TimeFun()
        {
            while (TimeThread.IsAlive)
            {
                Thread.Sleep(1000);
                ++Time;
            }
        }

        protected void StopTime()
        {
            TimeThread.Abort();
            TimeThread = null;
        }

        protected virtual ulong GetSpeed(ulong lenght)
        {
            if (Time == 0)
            {
                return 0;
            }

            return lenght / Time * 8;
        }

        protected virtual float GetProgress(ulong lenght)
        {
            return (DownloadCacheFileLenght + lenght) / (float) ServeLength;
        }

        public virtual void Dispose()
        {
            if (TimeThread == null)
                return;

            TimeThread.Abort();
            TimeThread = null;
        }
    }
}