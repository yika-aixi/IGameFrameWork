using System.Collections;
using System.Collections.Generic;
using Icarus.GameFramework;
using Icarus.GameFramework.Resource;
using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    public class BaseComponent : MonoBehaviour
    {
        public bool EditorResourceMode;
        public IResourceManager EditorResourceHelper;

        void Awake()
        {
            GameEntry.RegisterComponent(this);
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            GameFrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
#if UNITY_5_6_OR_NEWER
            Application.lowMemory -= OnLowMemory;
#endif
            GameFrameworkEntry.Shutdown();
        }


        private void OnLowMemory()
        {
            Log.Info("Low memory reported...");

            ObjectPoolComponent objectPoolComponent = GameEntry.GetComponent<ObjectPoolComponent>();
            if (objectPoolComponent != null)
            {
                objectPoolComponent.ReleaseAllUnused();
            }

            ResourceComponent resourceCompoent = GameEntry.GetComponent<ResourceComponent>();
            if (resourceCompoent != null)
            {
                resourceCompoent.ForceUnloadUnusedAssets(true);
            }
        }

        internal void Shutdown()
        {
            Destroy(gameObject);
        }
    }

}

