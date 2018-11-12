//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2018年11月12日-10:30
//Icarus.UnityGameFramework.Runtime

using Icarus.GameFramework;
using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    public class DefaultGameObjectHelper:GameObjectHelperBase
    {
        public override GameObject InstantiateGameObject(GameObject gameObjectAsset)
        {
            return Instantiate(gameObjectAsset);
        }

        public override void ReleaseGameObject(GameObject gameObjectAsset, GameObject gameObjectInstance)
        {
            m_ResourceComponent.UnloadAsset(gameObjectAsset);
            Destroy(gameObjectInstance);
        }
        
        
        private ResourceComponent m_ResourceComponent = null;
        private void Start()
        {
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Fatal("Resource component is invalid.");
                return;
            }
        }
    }
}