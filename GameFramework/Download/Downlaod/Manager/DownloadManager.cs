//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月15日 23:16:23
//Assembly-CSharp

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Icarus.GameFramework.Download
{
    /// <summary>
    /// 关于获取缓存文件路径的问题,为什么没有获取单个的是因为url下载地址可能会下载保存在不同的多个地方,
    /// 所以如果是直接获取缓存的话就只能返回该url所有缓存了,但是下载时寻找缓存可以准确定位
    /// 我的缓存key = url|path|fileName
    /// 准确获取的函数就不暴露了,因为需要path参数,都知道path了那还需要获取干嘛!
    /// </summary>
    public sealed class DownloadManager:IDisposable
    {
        private static readonly Dictionary<string, FileCacheEntity> _cacheFileLibraryContent = new Dictionary<string, FileCacheEntity>();

        /// <summary>
        /// 全部下载完成
        /// </summary>
        public Action AllCompleteHandle;

        /// <summary>
        /// 当前下载数量
        /// </summary>
        public int CurrentDownloadCount { get; private set; }

        /// <summary>
        /// 等待下载数量
        /// </summary>
        public int WaitDownloadCount { get; private set; }

        /// <summary>
        /// 同时下载数量,默认3个
        /// </summary>
        public int DownloadCount = 3;

        /// <summary>
        /// 统一的超时时间(毫秒) 默认:5秒
        /// </summary>
        public int TimeOut = 5000;

        /// <summary>
        /// 统一的缓存过期天数 默认:永不过期
        /// </summary>
        public int CacheExpiredDay = -1;

        /// <summary>
        /// 统一的重试次数
        /// </summary>
        public int RetryCount = 3;
        
        private string CacheLibraryFileName => "IcarusDownloadFileCaches.IcarusLibrary";
        
        private static bool _isInit;

        public DownloadManager()
        {
            Init();
        }

        /// <summary>
        /// 获取缓存库路径
        /// </summary>
        public string GetCacheLibraryFilePath()
        {
#if UNITY || UNITY_2017_1_OR_NEWER
            return $"{UnityEngine.Application.persistentDataPath}{Path.DirectorySeparatorChar}{CacheLibraryFileName}";
#else
                throw new NotImplementedException("没有缓存库的路径进行设置,目前只设置了Unity的");
#endif 
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            lock (_cacheFileLibraryContent)
            {
                //todo 读取缓存库
                if (!_isInit)
                {
                    _isInit = true;
                    if (!Utility.FileUtil.IsFileExists(GetCacheLibraryFilePath()))
                    {
                        return;
                    }
                    var libraryInfo = File.ReadAllLines(GetCacheLibraryFilePath());
                    foreach (var line in libraryInfo)
                    {
                        var entity = FileCacheEntity.Deserialization(line);
                        _cacheFileLibraryContent.Add(_getKey(entity.Url,entity.FilePath), entity);
                    }
                    _clearCacheLibrary();
                }
            }
        }

        /// <summary>
        /// 清理缓存库中的过期缓存
        /// </summary>
        private void _clearCacheLibrary()
        {
            List<string> deleteList = new List<string>();
            foreach (var entity in _cacheFileLibraryContent.Values)
            {
                //跳过永不过期的
                if (entity.ExpiredTime == 0)
                {
                    continue;
                }
                //还未过期
                if (entity.ExpiredTime >= (ulong) DateTime.Now.Ticks)
                {
                    //文件存在就跳过
                    if (Utility.FileUtil.IsFileExists(entity.FilePath))
                    {
                        continue;
                    }
                }

                deleteList.Add(entity.Url);
            }

            foreach (var url in deleteList)
            {
                _cacheFileLibraryContent.Remove(url);
            }

        }

        /// <summary>
        /// 该地址是否存在缓存
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool HasUrlIsExistsCache(string url)
        {
            Init();
            return _cacheFileLibraryContent.Keys.Any(key => key.Contains(url));
        }

        /// <summary>
        /// 获取所有url的缓存路径
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllCacheUrl()
        {
            Init();
            return _cacheFileLibraryContent.Keys;
        }

        /// <summary>
        /// 获取该Url的所有缓存路径
        /// </summary>
        /// <exception cref="Exception">该缓存不存在</exception>
        /// <returns></returns>
        public IEnumerable<string> GetAllCachePath(string url)
        {
            Init();
            return _cacheFileLibraryContent.TakeWhile(x => x.Key.Contains(url)).Select(x => x.Value.FilePath);
        }

        /// <summary>
        /// 获取所有缓存路径
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllCachePath()
        {
            Init();
            return _cacheFileLibraryContent.Select(x => x.Value.FilePath);
        }

        private static readonly List<DownloadUnitInfo> _downloadQueue = new List<DownloadUnitInfo>();

        /// <summary>
        /// 添加多个
        /// </summary>
        /// <param name="downloads">下载信息</param>
        public void AddRangeDownload(IEnumerable<DownloadUnitInfo> downloads)
        {
            Init();
            foreach (var download in downloads)
            {
                ++WaitDownloadCount;
                _downloadQueue.Add(download);
            }

            Update();
        }

        /// <summary>
        /// 添加指定的下载器进行下载,并且缓存没过期就返回缓存
        /// </summary>
        /// <param name="priority">优先级,越大优先级越高</param>
        /// <param name="url">下载地址</param>
        /// <param name="savePath">文件保存路径</param>
        /// <param name="fileName">文件名.默认为下载的文件名</param>
        /// <param name="timeOut">毫秒,默认为统一超时时间</param>
        /// <param name="cacheExpiredDay">过期天数</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="downloadProgressHandle">下载中</param>
        /// <param name="completeHandle">下载完成</param>
        /// <param name="errorHandle">下载失败</param>
        /// <param name="isFindCacheLibrary">是否查找缓存库,默认查找</param>
        public IDownload AddDownload<T>(int priority, string url, string savePath, string fileName = "", int timeOut = -1,
            int cacheExpiredDay = -1,
            int retryCount = -1, Action<DownloadInfo> downloadStartHandle = null, Action<DownloadProgressInfo, string> downloadProgressHandle = null,
            Action<string> completeHandle = null,
            Action<String> errorHandle = null, bool isFindCacheLibrary = true) where T : IDownload, new()
        {
            Init();
            _error = false;
            IDownload download = new T();
            DownloadUnitInfo info = new DownloadUnitInfo()
            {
                DownloadUtil = download,
                Priority = priority,
                Url = url,
                SavePath = savePath,
                FileName = fileName,
                TimeOut = timeOut,
                RetryCount = retryCount,
                CacheExpiredDay = cacheExpiredDay,
                DownloadStartHandle = downloadStartHandle,
                DownloadProgressHandle = downloadProgressHandle,
                CompleteHandle = completeHandle,
                ErrorHandle = errorHandle,
                IsFindCacheLibrary = isFindCacheLibrary
            };
            _downloadQueue.Add(info);
            ++WaitDownloadCount;
            Update();
            return download;
        }

        /// <summary>
        /// 更新队列
        /// </summary>
        private void _updateDownloadQueue()
        {
            _downloadQueue.RemoveAll(x => x == null);
            DownloadUnitInfo temp;
            for (var i = 0; i < _downloadQueue.Count; i++)
            {
                temp = _downloadQueue[i];
                for (var j = 0; j < _downloadQueue.Count; j++)
                {
                    if (temp.Priority < _downloadQueue[j].Priority)
                    {
                        temp = _downloadQueue[j];
                        _downloadQueue[j] = _downloadQueue[i];
                        _downloadQueue[i] = temp;
                    }
                }
            }
//            foreach (var info in _downloadQueue)
//            {
//                UnityEngine.Debug.Log(info);
//            }
        }

        private bool _error = false;
        public void Update()
        {
            _updateDownloadQueue();
            if (CurrentDownloadCount == 0 && WaitDownloadCount == 0 && !_error)
            {
                AllCompleteHandle?.Invoke();
                return;
            }
            for (int i = 0; i < DownloadCount; i++)
            {
                if (i > _downloadQueue.Count - 1)
                {
                    return;
                }
                if (CurrentDownloadCount >= DownloadCount)
                {
                    return;
                }
                ++CurrentDownloadCount;
                --WaitDownloadCount;
                var download = _downloadQueue[_downloadQueue.Count - 1];
                _downloadQueue.Remove(download);
                _startdownload(download);
            }
            
        }

        private void _startdownload(DownloadUnitInfo download)
        {
            //如果没有指定文件名就将文件名设置为远程文件名
            if (string.IsNullOrEmpty(download.FileName))
            {
                var p = download.Url.Split('/', '\\');
                download.FileName = p.Last();
            }

            //todo 查找缓存
            if (download.IsFindCacheLibrary)
            {
                //命中
                if (_hasCache(download))
                {
                    //文件存在就返回
                    if (Utility.FileUtil.IsFileExists(Path.Combine(download.SavePath,download.FileName)))
                    {
                        _complete(download);
                        return;
                    }
                }
            }

            download.TimeOut = download.TimeOut < 0 ? TimeOut : download.TimeOut;
            download.RetryCount = download.RetryCount < 0 ? RetryCount : download.RetryCount;
            download.CacheExpiredDay = download.CacheExpiredDay < 0
                ? CacheExpiredDay
                : download.CacheExpiredDay;

            download.DownloadUtil.Download(
                    download.Url, download.SavePath, download.FileName,download.DownloadStartHandle, download.DownloadProgressHandle,
                    x =>
                    {
                        _complete(download);
                    }, ex =>
                    {
                        _error = true;
                        if (download.CurrentRetryCount < download.RetryCount)
                        {
                            ++download.CurrentRetryCount;
                            UnityEngine.Debug.Log($"失败重来,当前重试次数:{download.CurrentRetryCount},失败信息：\n{ex}");
                            _startdownload(download);
                            return;
                        }
                        --CurrentDownloadCount;
                        download.ErrorHandle?.Invoke(ex);
                        download.DownloadUtil.Dispose();
                        Update();
                    });
        }

        private void _complete(DownloadUnitInfo download)
        {
            --CurrentDownloadCount;
            _addOrUpdateCache(download);
            download.CompleteHandle?.Invoke(_getCache(download).FilePath);
            download.DownloadUtil.Dispose();
            Update();
        }

        private string _getKey(DownloadUnitInfo info)
        {
            return _getKey(info.Url, info.SavePath, info.FileName);
        }

        private string _getKey(string url, string savePath, string fileName)
        {
            return _getKey(url, Utility.FileUtil.UpdatePathDirectorySeparator(Path.Combine(savePath, fileName)));
        }

        private string _getKey(string url, string filePath)
        {
            return $"{url}|{filePath}";
        }
        StringBuilder _sb = new StringBuilder();
        private void _addOrUpdateCache(DownloadUnitInfo info)
        {
            _sb.Clear();
            var cacheTime = (ulong)DateTime.Now.Ticks;
            ulong cacheExpiredTime = 0;

            if (info.CacheExpiredDay > 0)
            {
                DateTime centuryBegin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + CacheExpiredDay);
                cacheExpiredTime = (ulong) centuryBegin.Ticks;
            }

            var expiredTime = cacheExpiredTime;

            if (!_hasCache(info))
            {
                _cacheFileLibraryContent.Add(_getKey(info), new FileCacheEntity()
                {
                    Url = info.Url,
                    FilePath = Utility.FileUtil.UpdatePathDirectorySeparator(Path.Combine(info.SavePath, info.FileName)),
                    CacheTime = cacheTime,
                    ExpiredTime = expiredTime
                });
            }
            else
            {
                //更新缓存的缓存时间和过期时间
                _cacheFileLibraryContent[_getKey(info)].CacheTime = cacheTime;
                _cacheFileLibraryContent[_getKey(info)].ExpiredTime = expiredTime;
            }
            foreach (var cacheEntity in _cacheFileLibraryContent)
            {
                _sb.AppendLine(FileCacheEntity.Serialization(cacheEntity.Value));
            }
            File.WriteAllText(GetCacheLibraryFilePath(), _sb.ToString());
        }

        private bool _hasCache(DownloadUnitInfo info)
        {
            return _cacheFileLibraryContent.ContainsKey(_getKey(info));
        }

        private FileCacheEntity _getCache(DownloadUnitInfo info)
        {
            if (_hasCache(info))
            {
                return _cacheFileLibraryContent[_getKey(info)];
            }
            return null;
        }

        public void Dispose()
        {
            _downloadQueue.ForEach(x=>x.DownloadUtil.Dispose());
        }
    }
}