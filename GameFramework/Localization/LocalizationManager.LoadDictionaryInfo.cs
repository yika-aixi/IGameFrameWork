//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年03月24日-07:24
//Icarus.GameFramework

using Icarus.GameFramework.DataStruct;

namespace Icarus.GameFramework.Localization
{
    internal partial class LocalizationManager
    {
        private sealed class LoadDictionaryInfo
        {
            private readonly LoadType m_LoadType;
            private readonly object m_UserData;

            public LoadDictionaryInfo(LoadType loadType, object userData)
            {
                m_LoadType = loadType;
                m_UserData = userData;
            }

            public LoadType LoadType
            {
                get
                {
                    return m_LoadType;
                }
            }

            public object UserData
            {
                get
                {
                    return m_UserData;
                }
            }
        }
    }
}