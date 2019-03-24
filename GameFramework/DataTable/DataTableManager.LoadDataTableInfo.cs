//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年03月24日-07:17
//Icarus.GameFramework

using Icarus.GameFramework.DataStruct;

namespace Icarus.GameFramework.DataTable
{
    internal partial class DataTableManager
    {
        private sealed class LoadDataTableInfo
        {
            private readonly LoadType m_LoadType;
            private readonly object m_UserData;

            public LoadDataTableInfo(LoadType loadType, object userData)
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