//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2018年11月12日-11:25
//Icarus.UnityGameFramework.Runtime

using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    public class GameObjectPoolComponent:GameFrameworkComponent
    {
        [SerializeField]
        private string m_DataTableHelperTypeName = "Icarus.UnityGameFramework.Runtime.DefaultGameObjectHelper";

        private DefaultGameObjectHelper _defaultGameObjectHelper;

        public DefaultGameObjectHelper GameObjectHelper
        {
            get { return _defaultGameObjectHelper; }
        }

        void Start()
        {
            _defaultGameObjectHelper = Helper.CreateHelper(m_DataTableHelperTypeName, GameObjectHelper);
        }
    }
}