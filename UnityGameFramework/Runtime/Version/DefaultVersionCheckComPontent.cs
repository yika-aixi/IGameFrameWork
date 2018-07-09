using Icarus.GameFramework;
using Icarus.GameFramework.Version;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Icarus/Game Framework/Default VersionCheck")]
    public class DefaultVersionCheckComPontent : MonoBehaviour, IVersionCheck
    {
        public string Url { get; set; }
        public VersionInfo ServerVersionInfo { get; private set; }

        private VersionInfo _localVersionInfo;
        public VersionInfo LocalVersionInfo
        {
            get
            {
                if (_localVersionInfo == null)
                {
                    _localVersionInfo = new VersionInfo();
                }
                _localVersionInfo.SetAssetBundleInfos(
                    PersistentInfos.AssetBundleInfos.
                        Union(StreamingVersionInfo.AssetBundleInfos).ToList());

                return _localVersionInfo;
            }
        }

        public VersionInfo StreamingVersionInfo { get; private set; }
        public VersionInfo PersistentInfos { get; private set; }
        private GameFrameworkAction<string> _errorHandle;
        private GameFrameworkAction<IEnumerable<AssetBundleInfo>> _completeHandle;
        private GameFrameworkAction<string> _stateUpdateHandle;
        private bool _strictMode;
        /// <summary>
        /// 严格模式下将会计算本地资源包md5和远程版本文件对比,
        /// 否则只会对比本地版本文件里记录的md5
        /// </summary>
        /// <param name="strictMode">严格模式,默认为:严格</param>
        /// <param name="completeHandle">检查完成,参数:更新列表</param>
        /// <param name="errorHandle">检查失败,参数:失败信息</param>
        /// <param name="stateUpdateHandle">检查状态,参数:当前状态</param>
        public void Check(bool strictMode = true, GameFrameworkAction<IEnumerable<AssetBundleInfo>> completeHandle = null,
            GameFrameworkAction<string> errorHandle = null, GameFrameworkAction<string> stateUpdateHandle = null)
        {
            _isInitCheck = false;
            _strictMode = strictMode;
            _completeHandle = completeHandle;
            _errorHandle = errorHandle;
            _stateUpdateHandle = stateUpdateHandle;
            _stateUpdate("资源版本,检查开始!");
            StartCoroutine(_check());
        }

        void _stateUpdate(string str)
        {
            //todo 临时写法,后面加入国际化变为国际化
            _stateUpdateHandle?.Invoke(str);
        }

        private bool _isInitCheck = false;

        ///<inheritdoc cref="IVersionCheck"/>
        ///<exception cref="GameFrameworkException">Check 函数还未执行过或还未执行完成 </exception>
        /// <returns></returns>
        public IEnumerable<AssetBundleInfo> GetGroupVersion(string tag)
        {
            if (!_isInitCheck)
            {
                throw new GameFrameworkException("没有执行Check 函数或还未执行完成~");
            }

            var serverABs = ServerVersionInfo.GetGroupAssetBundleInfos(tag);
            var localABs = LocalVersionInfo.GetGroupAssetBundleInfos(tag);
            return _chckAssetBundleList(serverABs, localABs);
        }

        public bool IsUpdateGroup(string tag)
        {
            if (!_isInitCheck)
            {
                throw new GameFrameworkException("没有执行Check 函数或还未执行完成~");
            }

            var serverABs = ServerVersionInfo.GetGroupAssetBundleInfos(tag);
            var localABs = LocalVersionInfo.GetGroupAssetBundleInfos(tag);
            return _existUpdateAssetBundle(serverABs, localABs);
        }

        private IEnumerator _check()
        {
            var url = GameFramework.Utility.Path.GetRemotePath(Path.Combine(Application.streamingAssetsPath,
                ConstTable.VersionFileName));
            VersionInfo streamInfos;
            VersionInfo persistentInfos;
            VersionInfo serverInfos;
            VersionInfo version;
            using (WWW www = new WWW(url))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    version = _jieMi(www.bytes);
                    if (version == null)
                    {
                        streamInfos = new VersionInfo();
                    }
                    else
                    {
                        streamInfos = version;
                        _stateUpdate($"发现App资源版本,版本为:{streamInfos.Version}");
                    }
                }
                else
                {
                    streamInfos = new VersionInfo();
                }
            }

            var versionInfoFilePath = Path.Combine(Application.persistentDataPath, ConstTable.VersionFileName);
            if (File.Exists(versionInfoFilePath))
            {
                var versionInfoFileBy =
                    File.ReadAllBytes(versionInfoFilePath);
                version = _jieMi(versionInfoFileBy);
                if (version != null)
                {
                    persistentInfos = version;
                    _stateUpdate($"发现本地资源版本,版本为:{persistentInfos.Version}");
                }
                else
                {
                    persistentInfos = new VersionInfo();
                }
            }
            else
            {
                persistentInfos = new VersionInfo();
            }
            

            PersistentInfos = persistentInfos;
            StreamingVersionInfo = streamInfos;

            _persistentAssetBundleCheck();

            var localAllInfo = new VersionInfo(persistentInfos.Version,
                persistentInfos.AssetBundleInfos.Union(streamInfos.AssetBundleInfos).ToList());

            using (WWW www = new WWW(Url))
            {
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    _stateUpdate($"资源版本检查失败!");
                    _errorHandle?.Invoke(www.error);
                    yield break;
                }

                version = _jieMi(www.bytes);
                if (version == null)
                {
                    _stateUpdate($"资源版本检查失败!");
                    _errorHandle?.Invoke("解析服务器version.info失败.");
                    yield break;
                }
                serverInfos = version;
            }

            List<AssetBundleInfo> result = _chckVersion(serverInfos, localAllInfo);
            ServerVersionInfo = serverInfos;
            _isInitCheck = true;
            _stateUpdate($"资源版本检查完成! 最新版本为:{ServerVersionInfo.Version}");
            _completeHandle?.Invoke(result);
        }

        private bool _persistentAssetBundleCheck()
        {
            _stateUpdate($"本地资源检查中....");
            List<AssetBundleInfo> missingAssetBundleInfos = new List<AssetBundleInfo>();
            foreach (var bundleInfo in PersistentInfos.AssetBundleInfos)
            {
                if (!File.Exists(Path.Combine(Application.persistentDataPath,bundleInfo.PackFullName)))
                {
                    missingAssetBundleInfos.Add(bundleInfo);
                }
            }

            if (missingAssetBundleInfos.Count > 0)
            {
                _stateUpdate($"发现丢失资源,一共丢失:{missingAssetBundleInfos.Count}");
            }

            foreach (var missingAssetBundleInfo in missingAssetBundleInfos)
            {
                PersistentInfos.Remove(missingAssetBundleInfo);
            }
            _stateUpdate($"本地资源检查完成!");
            return true;
        }

        private List<AssetBundleInfo> _chckVersion(VersionInfo serverInfos, VersionInfo localAllInfo)
        {
            List<AssetBundleInfo> result = new List<AssetBundleInfo>();
            foreach (var abInfo in serverInfos.AssetBundleInfos)
            {
                var serverInfo = serverInfos.GetAssetBundleInfo(abInfo.PackFullName);
                var localInfo = localAllInfo.GetAssetBundleInfo(abInfo.PackFullName);

                if (_isEqual(serverInfo,localInfo))
                {
                    continue;
                }

                //加入结果列表
                result.Add(abInfo);
            }

            return result;
        }

        AssetBundleInfo _findAssetBundleInfo(IEnumerable<AssetBundleInfo> abs, string abFilePath)
        {
            abFilePath = GameFramework.Utility.Path.GetRegularPath(abFilePath);
            return abs.FirstOrDefault(x => x.PackFullName == abFilePath);
        }

        private IEnumerable<AssetBundleInfo> _chckAssetBundleList(IEnumerable<AssetBundleInfo> serverABs,
            IEnumerable<AssetBundleInfo> localABs,bool getGroup = true)
        {
            List<AssetBundleInfo> result = new List<AssetBundleInfo>();

            if (serverABs == null)
            {
                return result;
            }

            if (localABs == null)
            {
                return serverABs;
            }

            foreach (var abInfo in serverABs)
            {
                var serverInfo = _findAssetBundleInfo(serverABs, abInfo.PackFullName);
                var localInfo = _findAssetBundleInfo(localABs,abInfo.PackFullName);

                if (_isEqual(serverInfo, localInfo, getGroup))
                {
                    continue;
                }

                //加入结果列表
                result.Add(abInfo);
            }

            return result;
        }

        /// <summary>
        /// 资源组中是否有需要更新的
        /// </summary>
        /// <param name="serverABs"></param>
        /// <param name="localABs"></param>
        /// <param name="getGroup"></param>
        /// <returns></returns>
        private bool _existUpdateAssetBundle(IEnumerable<AssetBundleInfo> serverABs,
            IEnumerable<AssetBundleInfo> localABs, bool getGroup = true)
        {
            if (serverABs == null)
            {
                return false;
            }

            if (localABs == null)
            {
                return true;
            }

            foreach (var abInfo in serverABs)
            {
                var serverInfo = _findAssetBundleInfo(serverABs, abInfo.PackFullName);
                var localInfo = _findAssetBundleInfo(localABs, abInfo.PackFullName);

                if (_isEqual(serverInfo, localInfo, getGroup))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        bool _isEqual(AssetBundleInfo serverABInfo, AssetBundleInfo localABInfo, bool getGroup = false)
        {
            if (serverABInfo.Optional)
            {
                if (localABInfo == null && !getGroup)
                {
                    return true;
                }
            }

            if (localABInfo != null)
            {
                if (_strictMode)
                {
                    var md5 = GameFramework.Utility.MD5Util.GetFileMd5(
                        Path.Combine(Application.persistentDataPath,localABInfo.PackFullName));

                    if (serverABInfo.MD5 == md5)
                    {
                        return true;
                    }
                }
                else
                {
                    if (serverABInfo.MD5 == localABInfo.MD5)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        VersionInfo _jieMi(byte[] bytes)
        {
            try
            {
                VersionInfo version = new VersionInfo();
                version = version.JieMiDeserialize(bytes);
                return version;
            }
            catch (Exception e)
            {
                Debug.LogError("解密失败.\n"+e+"\n"+e.StackTrace);
                return null;
            }
        }

        void Awake()
        {
            GameEntry.RegisterComponent(this);

        }
    }
}