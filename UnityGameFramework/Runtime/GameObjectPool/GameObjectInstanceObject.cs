//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2018年11月12日-10:24
//Icarus.UnityGameFramework.Runtime

using Icarus.GameFramework.ObjectPool;
using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    public class GameObjectInstanceObject:ObjectBase
    {
        private readonly GameObject _gameObjectAsset;
        private readonly GameObjectHelperBase _gameObjectHelperHelper;
        private GameObject _target;
        public GameObjectInstanceObject(string name, GameObject gameObjectAsset,GameObject gameObjectInstance,GameObjectHelperBase gameObjectHelper) : base(name, gameObjectInstance)
        {
            _gameObjectAsset = gameObjectAsset;
            _gameObjectHelperHelper = gameObjectHelper;
            _target = gameObjectInstance;
        }

        protected override void Release(bool isShutdown)
        {
            _gameObjectHelperHelper.ReleaseGameObject(_gameObjectAsset,(GameObject) Target);
        }

        protected override void OnSpawn()
        {
            _target.SetActive(true);
        }

        protected override void OnUnspawn()
        {
            _target.SetActive(false);
        }
    }
}