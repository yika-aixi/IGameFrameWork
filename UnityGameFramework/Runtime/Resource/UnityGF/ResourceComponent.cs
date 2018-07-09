//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using Icarus.GameFramework;
using Icarus.GameFramework.ObjectPool;
using Icarus.GameFramework.Resource;
using System;
using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    /// <summary>
    /// 资源组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Icarus/Game Framework/Resource")]
    public sealed partial class ResourceComponent:MonoBehaviour
    {
        private const int DefaultPriority = 0;

        private IResourceManager m_ResourceManager = null;
        [SerializeField]
        private EventComponent _eventComponent = null;
        private bool m_EditorResourceMode = false;
        private bool m_ForceUnloadUnusedAssets = false;
        private bool m_PreorderUnloadUnusedAssets = false;
        private bool m_PerformGCCollect = false;
        private AsyncOperation m_AsyncOperation = null;
        private float m_LastOperationElapse = 0f;
        private ResourceHelperBase m_ResourceHelper = null;
        
        [SerializeField]
        private ReadWritePathType m_ReadWritePathType = ReadWritePathType.Unspecified;

        [SerializeField]
        private float m_UnloadUnusedAssetsInterval = 60f;

        [SerializeField]
        private float m_AssetAutoReleaseInterval = 60f;

        [SerializeField]
        private int m_AssetCapacity = 64;

        [SerializeField]
        private float m_AssetExpireTime = 60f;

        [SerializeField]
        private int m_AssetPriority = 0;

        [SerializeField]
        private float m_ResourceAutoReleaseInterval = 60f;

        [SerializeField]
        private int m_ResourceCapacity = 16;

        [SerializeField]
        private float m_ResourceExpireTime = 60f;

        [SerializeField]
        private int m_ResourcePriority = 0;

        [SerializeField]
        private string m_UpdatePrefixUri = null;

        [SerializeField]
        private int m_UpdateRetryCount = 3;

        [SerializeField]
        private Transform m_InstanceRoot = null;
        [SerializeField]
        private string m_ResourceHelperTypeName = "Icarus.UnityGameFramework.Runtime.DefaultResourceHelper";

        [SerializeField]
        private ResourceHelperBase m_CustomResourceHelper = null;
        [SerializeField]
        private string m_LoadResourceAgentHelperTypeName = "Icarus.UnityGameFramework.Runtime.DefaultLoadResourceAgentHelper";

        [SerializeField]
        private LoadResourceAgentHelperBase m_CustomLoadResourceAgentHelper = null;

        [SerializeField]
        private int m_LoadResourceAgentHelperCount = 3;

        /// <summary>
        /// 获取资源只读路径。
        /// </summary>
        public string ReadOnlyPath
        {
            get
            {
                return m_ResourceManager.ReadOnlyPath;
            }
        }

        /// <summary>
        /// 获取资源读写路径。
        /// </summary>
        public string ReadWritePath
        {
            get
            {
                return m_ResourceManager.ReadWritePath;
            }
        }
        
        /// <summary>
        /// 获取资源读写路径类型。
        /// </summary>
        public ReadWritePathType ReadWritePathType
        {
            get
            {
                return m_ReadWritePathType;
            }
        }

        /// <summary>
        /// 设置当前变体。
        /// </summary>
        public string CurrentVariant
        {
            get
            {
                return m_ResourceManager.CurrentVariant;
            }
        }

        /// <summary>
        /// 获取或设置无用资源释放间隔时间。
        /// </summary>
        public float UnloadUnusedAssetsInterval
        {
            get
            {
                return m_UnloadUnusedAssetsInterval;
            }
            set
            {
                m_UnloadUnusedAssetsInterval = value;
            }
        }

        /// <summary>
        /// 获取当前资源适用的游戏版本号。
        /// </summary>
        public string ApplicableGameVersion
        {
            get
            {
                return m_ResourceManager.ApplicableGameVersion;
            }
        }

        /// <summary>
        /// 获取当前资源内部版本号。
        /// </summary>
        public int InternalResourceVersion
        {
            get
            {
                return m_ResourceManager.InternalResourceVersion;
            }
        }

        /// <summary>
        /// 获取已准备完毕资源数量。
        /// </summary>
        public int AssetCount
        {
            get
            {
                return m_ResourceManager.AssetCount;
            }
        }

        /// <summary>
        /// 获取已准备完毕资源数量。
        /// </summary>
        public int ResourceCount
        {
            get
            {
                return m_ResourceManager.ResourceCount;
            }
        }
        
        /// <summary>
        /// 获取加载资源代理总数量。
        /// </summary>
        public int LoadTotalAgentCount
        {
            get
            {
                return m_ResourceManager.LoadTotalAgentCount;
            }
        }

        /// <summary>
        /// 获取可用加载资源代理数量。
        /// </summary>
        public int LoadFreeAgentCount
        {
            get
            {
                return m_ResourceManager.LoadFreeAgentCount;
            }
        }

        /// <summary>
        /// 获取工作中加载资源代理数量。
        /// </summary>
        public int LoadWorkingAgentCount
        {
            get
            {
                return m_ResourceManager.LoadWorkingAgentCount;
            }
        }

        /// <summary>
        /// 获取等待加载资源任务数量。
        /// </summary>
        public int LoadWaitingTaskCount
        {
            get
            {
                return m_ResourceManager.LoadWaitingTaskCount;
            }
        }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float AssetAutoReleaseInterval
        {
            get
            {
                return m_ResourceManager.AssetAutoReleaseInterval;
            }
            set
            {
                m_ResourceManager.AssetAutoReleaseInterval = m_AssetAutoReleaseInterval = value;
            }
        }

        /// <summary>
        /// 获取或设置资源对象池的容量。
        /// </summary>
        public int AssetCapacity
        {
            get
            {
                return m_ResourceManager.AssetCapacity;
            }
            set
            {
                m_ResourceManager.AssetCapacity = m_AssetCapacity = value;
            }
        }

        /// <summary>
        /// 获取或设置资源对象池对象过期秒数。
        /// </summary>
        public float AssetExpireTime
        {
            get
            {
                return m_ResourceManager.AssetExpireTime;
            }
            set
            {
                m_ResourceManager.AssetExpireTime = m_AssetExpireTime = value;
            }
        }

        /// <summary>
        /// 获取或设置资源对象池的优先级。
        /// </summary>
        public int AssetPriority
        {
            get
            {
                return m_ResourceManager.AssetPriority;
            }
            set
            {
                m_ResourceManager.AssetPriority = m_AssetPriority = value;
            }
        }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float ResourceAutoReleaseInterval
        {
            get
            {
                return m_ResourceManager.ResourceAutoReleaseInterval;
            }
            set
            {
                m_ResourceManager.ResourceAutoReleaseInterval = m_ResourceAutoReleaseInterval = value;
            }
        }

        /// <summary>
        /// 获取或设置资源对象池的容量。
        /// </summary>
        public int ResourceCapacity
        {
            get
            {
                return m_ResourceManager.ResourceCapacity;
            }
            set
            {
                m_ResourceManager.ResourceCapacity = m_ResourceCapacity = value;
            }
        }

        /// <summary>
        /// 获取或设置资源对象池对象过期秒数。
        /// </summary>
        public float ResourceExpireTime
        {
            get
            {
                return m_ResourceManager.ResourceExpireTime;
            }
            set
            {
                m_ResourceManager.ResourceExpireTime = m_ResourceExpireTime = value;
            }
        }

        /// <summary>
        /// 获取或设置资源对象池的优先级。
        /// </summary>
        public int ResourcePriority
        {
            get
            {
                return m_ResourceManager.ResourcePriority;
            }
            set
            {
                m_ResourceManager.ResourcePriority = m_ResourcePriority = value;
            }
        }

        void Awake()
        {
            GameEntry.RegisterComponent(this);
        }

        [SerializeField]
        private BaseComponent _baseComponent;
        private void Start()
        {
            _baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (_baseComponent == null)
            {
                Log.Fatal("Base component is invalid.");
                return;
            }
            _eventComponent = GameEntry.GetComponent<EventComponent>();
            if (_eventComponent == null)
            {
                Log.Fatal("Event component is invalid.");
                return;
            }

            m_EditorResourceMode = _baseComponent.EditorResourceMode;
            m_ResourceManager = m_EditorResourceMode ? _baseComponent.EditorResourceHelper : GameFrameworkEntry.GetModule<IResourceManager>();
            if (m_ResourceManager == null)
            {
                Log.Fatal("Resource manager is invalid.");
                return;
            }
            m_ResourceManager.ResourceInitComplete += OnResourceInitComplete;

            m_ResourceManager.SetReadOnlyPath(Application.streamingAssetsPath);
            if (m_ReadWritePathType == ReadWritePathType.TemporaryCache)
            {
                m_ResourceManager.SetReadWritePath(Application.temporaryCachePath);
            }
            else
            {
                if (m_ReadWritePathType == ReadWritePathType.Unspecified)
                {
                    m_ReadWritePathType = ReadWritePathType.PersistentData;
                }

                m_ResourceManager.SetReadWritePath(Application.persistentDataPath);
            }

            if (m_EditorResourceMode)
            {
                return;
            }

            m_ResourceManager.SetObjectPoolManager(GameFrameworkEntry.GetModule<IObjectPoolManager>());
            m_ResourceManager.AssetAutoReleaseInterval = m_AssetAutoReleaseInterval;
            m_ResourceManager.AssetCapacity = m_AssetCapacity;
            m_ResourceManager.AssetExpireTime = m_AssetExpireTime;
            m_ResourceManager.AssetPriority = m_AssetPriority;
            m_ResourceManager.ResourceAutoReleaseInterval = m_ResourceAutoReleaseInterval;
            m_ResourceManager.ResourceCapacity = m_ResourceCapacity;
            m_ResourceManager.ResourceExpireTime = m_ResourceExpireTime;
            m_ResourceManager.ResourcePriority = m_ResourcePriority;
            
            m_ResourceHelper = Helper.CreateHelper(m_ResourceHelperTypeName, m_CustomResourceHelper);
            if (m_ResourceHelper == null)
            {
                Log.Error("Can not create resource helper.");
                return;
            }

            m_ResourceHelper.name = string.Format("Resource Helper");
            Transform transform = m_ResourceHelper.transform;
            transform.SetParent(this.transform);
            transform.localScale = Vector3.one;

            m_ResourceManager.SetResourceHelper(m_ResourceHelper);

            if (m_InstanceRoot == null)
            {
                m_InstanceRoot = (new GameObject("Load Resource Agent Instances")).transform;
                m_InstanceRoot.SetParent(gameObject.transform);
                m_InstanceRoot.localScale = Vector3.one;
            }

            for (int i = 0; i < m_LoadResourceAgentHelperCount; i++)
            {
                AddLoadResourceAgentHelper(i);
            }
        }

        private void Update()
        {
            m_LastOperationElapse += Time.unscaledDeltaTime;
            if (m_AsyncOperation == null && (m_ForceUnloadUnusedAssets || m_PreorderUnloadUnusedAssets && m_LastOperationElapse >= m_UnloadUnusedAssetsInterval))
            {
                Log.Debug("Unload unused assets...");
                m_ForceUnloadUnusedAssets = false;
                m_PreorderUnloadUnusedAssets = false;
                m_LastOperationElapse = 0f;
                m_AsyncOperation = Resources.UnloadUnusedAssets();
            }

            if (m_AsyncOperation != null && m_AsyncOperation.isDone)
            {
                m_AsyncOperation = null;
                if (m_PerformGCCollect)
                {
                    Log.Debug("GC.Collect...");
                    m_PerformGCCollect = false;
                    GC.Collect();
                }
            }
        }
        
        /// <summary>
        /// 设置当前变体。
        /// </summary>
        /// <param name="currentVariant">当前变体。</param>
        public void SetCurrentVariant(string currentVariant)
        {
            m_ResourceManager.SetCurrentVariant(!string.IsNullOrEmpty(currentVariant) ? currentVariant : null);
        }

        /// <summary>
        /// 设置解密资源回调函数。
        /// </summary>
        /// <param name="decryptResourceCallback">要设置的解密资源回调函数。</param>
        /// <remarks>如果不设置，将使用默认的解密资源回调函数。</remarks>
        public void SetDecryptResourceCallback(DecryptResourceCallback decryptResourceCallback)
        {
            m_ResourceManager.SetDecryptResourceCallback(decryptResourceCallback);
        }

        /// <summary>
        /// 预订执行释放未被使用的资源。
        /// </summary>
        /// <param name="performGCCollect">是否使用垃圾回收。</param>
        public void UnloadUnusedAssets(bool performGCCollect)
        {
            m_PreorderUnloadUnusedAssets = true;
            if (performGCCollect)
            {
                m_PerformGCCollect = performGCCollect;
            }
        }

        /// <summary>
        /// 强制执行释放未被使用的资源。
        /// </summary>
        /// <param name="performGCCollect">是否使用垃圾回收。</param>
        public void ForceUnloadUnusedAssets(bool performGCCollect)
        {
            m_ForceUnloadUnusedAssets = true;
            if (performGCCollect)
            {
                m_PerformGCCollect = performGCCollect;
            }
        }

        /// <summary>
        /// 使用单机模式并初始化资源。
        /// </summary>
        public void InitResources()
        {
            m_ResourceManager.InitResources();
        }
        

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, null, DefaultPriority, loadAssetCallbacks, null);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, assetType, DefaultPriority, loadAssetCallbacks, null);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, null, priority, loadAssetCallbacks, null);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            LoadAsset(assetName, null, DefaultPriority, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, assetType, priority, loadAssetCallbacks, null);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            LoadAsset(assetName, assetType, DefaultPriority, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            LoadAsset(assetName, null, priority, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            m_ResourceManager.LoadAsset(assetName, assetType, priority, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 卸载资源。
        /// </summary>
        /// <param name="asset">要卸载的资源。</param>
        public void UnloadAsset(object asset)
        {
            m_ResourceManager.UnloadAsset(asset);
        }
        
        /// <summary>
        /// 增加加载资源代理辅助器。
        /// </summary>
        /// <param name="index">加载资源代理辅助器索引。</param>
        private void AddLoadResourceAgentHelper(int index)
        {
            LoadResourceAgentHelperBase loadResourceAgentHelper = Helper.CreateHelper(m_LoadResourceAgentHelperTypeName, m_CustomLoadResourceAgentHelper, index);
            if (loadResourceAgentHelper == null)
            {
                Log.Error("Can not create load resource agent helper.");
                return;
            }

            loadResourceAgentHelper.name = string.Format("Load Resource Agent Helper - {0}", index.ToString());
            Transform transform = loadResourceAgentHelper.transform;
            transform.SetParent(m_InstanceRoot);
            transform.localScale = Vector3.one;

            m_ResourceManager.AddLoadResourceAgentHelper(loadResourceAgentHelper);
        }

        private void OnResourceInitComplete(object sender, GameFramework.Resource.ResourceInitCompleteEventArgs e)
        {
            _eventComponent.Fire(this, ReferencePool.Acquire<ResourceInitCompleteEventArgs>().Fill(e));
        }
    }
}
