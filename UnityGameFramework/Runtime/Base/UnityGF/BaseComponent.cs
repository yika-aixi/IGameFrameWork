using System;
using System.Collections;
using System.Collections.Generic;
using Icarus.GameFramework;
using Icarus.GameFramework.Localization;
using Icarus.GameFramework.Resource;
using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    /// <summary>
    /// 基础组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Base")]
    public sealed class BaseComponent : GameFrameworkComponent
    {
        private const int DefaultDpi = 96;  // default windows dpi

        private float m_GameSpeedBeforePause = 1f;

        [SerializeField]
        private bool m_EditorResourceMode = true;
        
        [SerializeField]
        private Language m_EditorLanguage = Language.Unspecified;

        [SerializeField]
        private string m_LogHelperTypeName = "Icarus.UnityGameFramework.Runtime.UnityLog";

        [SerializeField]
        private string m_ZipHelperTypeName = "Icarus.UnityGameFramework.Runtime.DefaultZipHelper";

        [SerializeField]
        private string m_JsonHelperTypeName = "Icarus.UnityGameFramework.Runtime.DefaultJsonHelper";

        [SerializeField]
        private int m_FrameRate = 30;

        [SerializeField]
        private float m_GameSpeed = 1f;

        [SerializeField]
        private bool m_RunInBackground = true;

        [SerializeField]
        private bool m_NeverSleep = true;

        /// <summary>
        /// 获取或设置是否使用编辑器资源模式（仅编辑器内有效）。
        /// </summary>
        public bool EditorResourceMode
        {
            get
            {
                return m_EditorResourceMode;
            }
            set
            {
                m_EditorResourceMode = value;
            }
        }
        
        /// <summary>
        /// 获取或设置编辑器语言（仅编辑器内有效）。
        /// </summary>
        public Language EditorLanguage
        {
            get
            {
                return m_EditorLanguage;
            }
            set
            {
                m_EditorLanguage = value;
            }
        }

        /// <summary>
        /// 获取或设置编辑器资源辅助器。
        /// </summary>
        public IResourceManager EditorResourceHelper
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置游戏帧率。
        /// </summary>
        public int FrameRate
        {
            get
            {
                return m_FrameRate;
            }
            set
            {
                Application.targetFrameRate = m_FrameRate = value;
            }
        }

        /// <summary>
        /// 获取或设置游戏速度。
        /// </summary>
        public float GameSpeed
        {
            get
            {
                return m_GameSpeed;
            }
            set
            {
                Time.timeScale = m_GameSpeed = (value >= 0f ? value : 0f);
            }
        }

        /// <summary>
        /// 获取游戏是否暂停。
        /// </summary>
        public bool IsGamePaused
        {
            get
            {
                return m_GameSpeed <= 0f;
            }
        }

        /// <summary>
        /// 获取是否正常游戏速度。
        /// </summary>
        public bool IsNormalGameSpeed
        {
            get
            {
                return m_GameSpeed == 1f;
            }
        }

        /// <summary>
        /// 获取或设置是否允许后台运行。
        /// </summary>
        public bool RunInBackground
        {
            get
            {
                return m_RunInBackground;
            }
            set
            {
                Application.runInBackground = m_RunInBackground = value;
            }
        }

        /// <summary>
        /// 获取或设置是否禁止休眠。
        /// </summary>
        public bool NeverSleep
        {
            get
            {
                return m_NeverSleep;
            }
            set
            {
                m_NeverSleep = value;
                Screen.sleepTimeout = value ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            InitLogHelper();
            Log.Info("Unity Version: {0}", Application.unityVersion);

#if UNITY_5_3_OR_NEWER || UNITY_5_3
            InitZipHelper();
            InitJsonHelper();

            GameFramework.Utility.Converter.ScreenDpi = Screen.dpi;
            if (GameFramework.Utility.Converter.ScreenDpi <= 0)
            {
                GameFramework.Utility.Converter.ScreenDpi = DefaultDpi;
            }

            m_EditorResourceMode &= Application.isEditor;
            if (m_EditorResourceMode)
            {
                Log.Info("During this run, Game Framework will use editor resource files, which you should validate first.");
            }

            Application.targetFrameRate = m_FrameRate;
            Time.timeScale = m_GameSpeed;
            Application.runInBackground = m_RunInBackground;
            Screen.sleepTimeout = m_NeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
#else
            Log.Error("Game Framework only applies with Unity 5.3 and above, but current Unity version is {0}.", Application.unityVersion);
            GameEntry.Shutdown(ShutdownType.Quit);
#endif
#if UNITY_5_6_OR_NEWER
            Application.lowMemory += OnLowMemory;
#endif
        }

        private void Start()
        {

        }

        private void Update()
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

        /// <summary>
        /// 暂停游戏。
        /// </summary>
        public void PauseGame()
        {
            if (IsGamePaused)
            {
                return;
            }

            m_GameSpeedBeforePause = GameSpeed;
            GameSpeed = 0f;
        }

        /// <summary>
        /// 恢复游戏。
        /// </summary>
        public void ResumeGame()
        {
            if (!IsGamePaused)
            {
                return;
            }

            GameSpeed = m_GameSpeedBeforePause;
        }

        /// <summary>
        /// 重置为正常游戏速度。
        /// </summary>
        public void ResetNormalGameSpeed()
        {
            if (IsNormalGameSpeed)
            {
                return;
            }

            GameSpeed = 1f;
        }

        internal void Shutdown()
        {
            Destroy(gameObject);
        }

        private void InitLogHelper()
        {
            if (string.IsNullOrEmpty(m_LogHelperTypeName))
            {
                return;
            }

            Type logHelperType = GameFramework.Utility.Assembly.GetType(m_LogHelperTypeName);

            if (logHelperType == null)
            {
                throw new GameFrameworkException(string.Format("Can not find log helper type '{0}'.", logHelperType));
            }

            ILog logHelper = (ILog)Activator.CreateInstance(logHelperType);

            if (logHelper == null)
            {
                throw new GameFrameworkException(string.Format("Can not create log helper instance '{0}'.", logHelperType));
            }

            Log.SetLog(logHelper);
        }

        private void InitZipHelper()
        {
            if (string.IsNullOrEmpty(m_ZipHelperTypeName))
            {
                return;
            }

            Type zipHelperType = GameFramework.Utility.Assembly.GetType(m_ZipHelperTypeName);
            if (zipHelperType == null)
            {
                Log.Error("Can not find Zip helper type '{0}'.", m_ZipHelperTypeName);
                return;
            }

            GameFramework.Utility.Zip.IZipHelper zipHelper = (GameFramework.Utility.Zip.IZipHelper)Activator.CreateInstance(zipHelperType);
            if (zipHelper == null)
            {
                Log.Error("Can not create Zip helper instance '{0}'.", m_ZipHelperTypeName);
                return;
            }

            GameFramework.Utility.Zip.SetZipHelper(zipHelper);
        }

        private void InitJsonHelper()
        {
            if (string.IsNullOrEmpty(m_JsonHelperTypeName))
            {
                return;
            }

            Type jsonHelperType = GameFramework.Utility.Assembly.GetType(m_JsonHelperTypeName);
            if (jsonHelperType == null)
            {
                Log.Error("Can not find JSON helper type '{0}'.", m_JsonHelperTypeName);
                return;
            }

            GameFramework.Utility.Json.IJsonHelper jsonHelper = (GameFramework.Utility.Json.IJsonHelper)Activator.CreateInstance(jsonHelperType);
            if (jsonHelper == null)
            {
                Log.Error("Can not create JSON helper instance '{0}'.", m_JsonHelperTypeName);
                return;
            }

            GameFramework.Utility.Json.SetJsonHelper(jsonHelper);
        }

//        private void InitProfilerHelper()
//        {
//            if (string.IsNullOrEmpty(m_ProfilerHelperTypeName))
//            {
//                return;
//            }
//
//            Type profilerHelperType = GameFramework.GameFramework.Utility.Assembly.GetType(m_ProfilerHelperTypeName);
//            if (profilerHelperType == null)
//            {
//                Log.Error("Can not find profiler helper type '{0}'.", m_ProfilerHelperTypeName);
//                return;
//            }
//
//            GameFramework.Utility.Profiler.IProfilerHelper profilerHelper = (GameFramework.Utility.Profiler.IProfilerHelper)Activator.CreateInstance(profilerHelperType, Thread.CurrentThread);
//            if (profilerHelper == null)
//            {
//                Log.Error("Can not create profiler helper instance '{0}'.", m_ProfilerHelperTypeName);
//                return;
//            }
//
//            GameFramework.Utility.Profiler.SetProfilerHelper(profilerHelper);
//        }

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
    }

}

