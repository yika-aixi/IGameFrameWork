//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月16日 3:38:38
//Assembly-CSharp

namespace Icarus.GameFramework.Download
{
    internal class FileCacheEntity
    {
        /// <summary>
        /// 所属url
        /// </summary>
        public string Url;
        /// <summary>
        /// 文件地址
        /// </summary>
        public string FilePath;
        /// <summary>
        /// 缓存时间
        /// </summary>
        public ulong CacheTime;
        /// <summary>
        /// 过期时间
        /// </summary>
        public ulong ExpiredTime;

        public static FileCacheEntity Deserialization(string str)
        {
            var ps = str.Split('\t');
            var entity = new FileCacheEntity()
            {
                Url = ps[0],
                FilePath = ps[1],
                CacheTime = ulong.Parse(ps[2]),
                ExpiredTime = ulong.Parse(ps[3])
            };
            return entity;
        }

        public static string Serialization(FileCacheEntity entity)
        {
            return $"{entity.Url}\t" +
                   $"{entity.FilePath}\t" +
                   $"{entity.CacheTime}\t" +
                   $"{entity.ExpiredTime}";
        }
    }
}