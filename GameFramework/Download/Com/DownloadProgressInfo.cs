//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月12日 21:22:51
//Assembly-CSharp


namespace Icarus.GameFramework.Download
{
    public struct DownloadProgressInfo
    {
        public DownloadProgressInfo(float progress) : this(progress,default(DownloadInfo))
        {
        }

        public DownloadProgressInfo(float progress, DownloadInfo info) : this(0, progress,info)
        {
        }

        public DownloadProgressInfo(ulong speed, float progress, DownloadInfo info) : this()
        {
            Speed = speed;
            Progress = progress;
            Info = info;
        }

        public ulong Speed { get; }
        public float Progress { get; }
        public DownloadInfo Info{get;}
    }
}