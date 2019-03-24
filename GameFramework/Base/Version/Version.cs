//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年03月24日-04:29
//Icarus.GameFramework

namespace Icarus.GameFramework.Version
{
    /// <summary>
    /// 版本
    /// </summary>
    public static partial  class Version
    {
        private const string GameFrameworkVersionString = "3.1.6";
     
        private static IVersionHelper s_VersionHelper = null;

        /// <summary>
        /// 获取游戏框架版本号。
        /// </summary>
        public static string GameFrameworkVersion
        {
            get
            {
                return GameFrameworkVersionString;
            }
        }

        /// <summary>
        /// 获取游戏版本号。
        /// </summary>
        public static string GameVersion
        {
            get
            {
                if (s_VersionHelper == null)
                {
                    return string.Empty;
                }

                return s_VersionHelper.GameVersion;
            }
        }

        /// <summary>
        /// 获取内部游戏版本号。
        /// </summary>
        public static int InternalGameVersion
        {
            get
            {
                if (s_VersionHelper == null)
                {
                    return 0;
                }

                return s_VersionHelper.InternalGameVersion;
            }
        }

        /// <summary>
        /// 设置版本号辅助器。
        /// </summary>
        /// <param name="versionHelper">要设置的版本号辅助器。</param>
        public static void SetVersionHelper(IVersionHelper versionHelper)
        {
            s_VersionHelper = versionHelper;
        }
    }
}