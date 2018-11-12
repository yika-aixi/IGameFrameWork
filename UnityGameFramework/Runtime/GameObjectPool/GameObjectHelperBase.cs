//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2018年11月12日-10:28
//Icarus.UnityGameFramework.Runtime

using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    public abstract class GameObjectHelperBase:MonoBehaviour
    {
        /// <summary>
        /// 实例化GameObject。
        /// </summary>
        /// <param name="gameObjectAsset">要实例化的GameObject资源。</param>
        /// <returns>实例化后的GameObject。</returns>
        public abstract GameObject InstantiateGameObject(GameObject gameObjectAsset);

        /// <summary>
        /// 释放GameObject。
        /// </summary>
        /// <param name="gameObjectAsset">要释放的GameObject资源。</param>
        /// <param name="gameObjectInstance">要释放的GameObject实例。</param>
        public abstract void ReleaseGameObject(GameObject gameObjectAsset, GameObject gameObjectInstance);
    }
}