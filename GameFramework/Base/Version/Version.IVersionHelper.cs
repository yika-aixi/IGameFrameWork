//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年03月24日-04:31
//Icarus.GameFramework

namespace Icarus.GameFramework.Version
{
    public static partial class Version
    {
        /// <summary>
        /// 版本号辅助器接口。
        /// </summary>
        public interface IVersionHelper
        {
            /// <summary>
            /// 获取游戏版本号。
            /// </summary>
            string GameVersion
            {
                get;
            }

            /// <summary>
            /// 获取内部游戏版本号。
            /// </summary>
            int InternalGameVersion
            {
                get;
            }
        }
    }
}