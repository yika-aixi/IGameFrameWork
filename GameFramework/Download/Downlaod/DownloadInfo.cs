//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月18日 2:41:34
//Assembly-CSharp

namespace Icarus.GameFramework.Download
{
    public struct DownloadInfo
    {
        public DownloadInfo(ulong downloadLenght, ulong totalLength) : this()
        {
            DownloadLenght = downloadLenght;
            TotalLength = totalLength;
        }

        public DownloadInfo(ulong totalLength) : this(0,totalLength)
        {
        }


        /// <summary>
        /// 下载长度
        /// </summary>
        public ulong DownloadLenght { get; private set; }
        /// <summary>
        /// 总长度
        /// </summary>
        public ulong TotalLength { get; private set; }

        public void SetDownloadLenght(ulong downloadLenght)
        {
            DownloadLenght = downloadLenght;
        }

        internal void SetTotalLength(ulong totalLength)
        {
            TotalLength = totalLength;
        }
    }
}