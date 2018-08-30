using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Icarus.GameFramework;
using Icarus.GameFramework.Event;
using Icarus.GameFramework.Resource;
using Icarus.GameFramework.UpdateAssetBundle;
using Icarus.GameFramework.Version;
using UnityEngine;
using Icarus.UnityGameFramework.Runtime;
using UnityEngine.UI;
using LoadAssetsCompleteEventArgs = Icarus.UnityGameFramework.Runtime.LoadAssetsCompleteEventArgs;
using Object = UnityEngine.Object;

public class test : MonoBehaviour
{
    public string AssetName;
    public UpdateInfo Info;
    public bool 严格模式 = true;
    public string AppUpdateUrl;
    private EventComponent _eventComponent;
    private BaseComponent _baseComponent;
    private ResourceComponent _resourceComponent;
    private SceneComponent _sceneComponent;

    private DefaultVersionCheckCompontent _versionCheck;

    private DefaultUpdateAssetBundleComponent _update;

    public bool 检查资源版本 = true;
    // Use this for initialization
    void Start()
    {
        _eventComponent = GameEntry.GetComponent<EventComponent>();
        _baseComponent = GameEntry.GetComponent<BaseComponent>();
        _resourceComponent = GameEntry.GetComponent<ResourceComponent>();
        _sceneComponent = GameEntry.GetComponent<SceneComponent>();
        if (!_baseComponent)
        {
            Debug.LogError("Base component is invalid.");
            return;
        }
        if (!_sceneComponent)
        {
            Debug.LogError("Scene Component 没有找到!");
            return;
        }
        _eventComponent.Subscribe(ReferencePool.Acquire<LoadSceneSuccessEventArgs>().Id, _loadSceneComplete);

        if (!_baseComponent.EditorResourceMode)
        {
            if (!检查资源版本)
            {
                _resourceComponent.InitResources();
                return;
            }

            _checkVersionAndUpdateAssetbundle();
        }
        else
        {
            _load();
        }
    }
    [ContextMenu("检查并更新资源")]
    public void _checkVersionAndUpdateAssetbundle()
    {
        //进行资源版本检测
        _versionCheck = GameEntry.GetComponent<DefaultVersionCheckCompontent>();
        if (!_versionCheck)
        {
            Debug.LogError("Default VersionCheck ComPontent is invalid.");
            return;
        }

        _versionCheck.Url = Info.AssetBundleUrl + "/" + ConstTable.VersionFileName;
        _versionCheck.StrictMode = 严格模式;
        _versionCheck.Check(x =>
        {
            _isInit = true;
            foreach (var info in x)
            {
                Debug.Log("需要更新的资源:" + info);
            }

            _update = GameEntry.GetComponent<DefaultUpdateAssetBundleComponent>();
            if (!_update)
            {
                Debug.LogError("Default UpdateAsset Bundle ComPontent is invalid.");
                return;
            }
            _UpdateAssetbundle(x, () => { _loadAsset1(); });

        }, () => AppUpdateUrl, errorHandle: Debug.LogError, stateUpdateHandle: Debug.Log);

    }

    [SerializeField]
    private string _groupTag;
    [ContextMenu("显示资源组")]
    void _showAssetGroupABList()
    {
        var list = _resourceComponent.GetAssetGroupList(_groupTag);

        if (list == null)
        {
            return;
        }

        foreach (var s in list)
        {
            Log.Debug($"资源组:{_groupTag},资源包:{s}");
        }
    }
    [SerializeField]
    private string _abName;
    [ContextMenu("显示资源包")]
    void _showAssetsList()
    {
        var list = _resourceComponent.GetAssetsList(_abName);

        if (list == null)
        {
            return;
        }

        foreach (var s in list)
        {
            Log.Debug($"资源包:{_abName},资源:{s}");
        }
    }


    public Button button;
    private void _checkOutGroup()
    {
        Debug.Log("资源组检查");
        if (_versionCheck.IsUpdateGroup("Checkpoint_1"))
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    private bool _isUpadteComplete;
    private void _UpdateAssetbundle(System.Collections.Generic.IEnumerable<AssetBundleInfo> abs
    , Action allCompleteHandle = null)
    {
        _update.UpdateAssetBundle(Info, abs, _versionCheck.PersistentInfos, _versionCheck.ServerVersionInfo.Version, (pro, str) =>
         {
             Debug.Log($"下载进度：{pro.Progress}，下载速度：{pro.Speed},下载描述：{str}");
         }, x1 =>
         {
             Debug.Log("更新完成：" + x1);
         }, () =>
         {
             _isUpadteComplete = true;
             Debug.Log("全部更新完成!");
             allCompleteHandle?.Invoke();
         }, ex =>
         {
             Debug.Log("更新出错：" + ex);
         });
    }

    public void LoadAsset()
    {
        if (!_isUpadteComplete)
        {
            Debug.Log("更新还未完成~");
            return;
        }
        _loadAsset();
    }

    private bool _isInit = false;
    public void DownloadOrUpdateGroup(string groupTag)
    {
        if (!_isInit)
        {
            Debug.Log("初始化检查还未完成");
            return;
        }

        var list = _versionCheck.GetGroupVersion(groupTag).ToList();
        Debug.Log($"资源组:{groupTag},资源包个数为:{list.Count}");
        foreach (var info in list)
        {
            Debug.Log($"资源包:\n{info}");
        }

        _UpdateAssetbundle(list, _loadAsset1);

    }
    [ContextMenu("加载场景")]
    public void LoadScene()
    {
        LoadScene(AssetName);

    }
    public void LoadScene(string name)
    {
        _sceneComponent.LoadScene(name);
    }

    private void _loadSceneComplete(object sender, GameEventArgs e)
    {
        Debug.Log("关卡1加载完成!s");
    }

    void _loadAsset1()
    {
        var eventArgs = ReferencePool.Acquire<Icarus.UnityGameFramework.Runtime.ResourceInitCompleteEventArgs>();
        _eventComponent.Subscribe(eventArgs.Id, _checkOutGroup);
        _resourceComponent.InitResources();
        ReferencePool.Release(eventArgs);

    }

    [SerializeField]
    private string groupTag;
    [ContextMenu("加载指定资源组")]
    void _loadGroup()
    {
        List<string> assetsPath = new List<string>();
        var assetNames = _resourceComponent.GetAssetGroupList(groupTag);
        foreach (var abName in assetNames)
        {
            if (abName.Contains("default"))
            {
                assetsPath = _resourceComponent.GetAssetsList(abName).ToList();
            }
        }
        _eventComponent.Subscribe(ReferencePool.Acquire<LoadAssetsCompleteEventArgs>().Id, _allComplete);

        _resourceComponent.LoadAssets(assetsPath, null, 0, new LoadAssetCallbacks(_loadAssetSuccessCallback));
    }

    private void _allComplete(object sender, GameEventArgs e)
    {
        var args = (LoadAssetsCompleteEventArgs) e;

        _allComplete(args.AssetNames, args.Assets, args.Duration, args.UserData);
    }

    //[SerializeField]
    private int loadCount = 2;
    string[] result1;
    string[] result2;
    [ContextMenu("加载指定资源组-LoadAssets,分2次")]
    void _loadGroupCount1()
    {
        List<string> assetsPath = new List<string>();
        var assetNames = _resourceComponent.GetAssetGroupList(groupTag);
        foreach (var abName in assetNames)
        {
            if (abName.Contains("default"))
            {
                assetsPath = _resourceComponent.GetAssetsList(abName).ToList();
            }
        }

        var count = assetsPath.Count / loadCount;
        result1 = new string[count];
        result2 = new string[count];
        for (var i = 0; i < result1.Length; i++)
        {
            result1[i] = assetsPath[i];
        }
        for (var i = 0; i < result2.Length; i++)
        {
            result2[i] = assetsPath[i + count];
        }
        //        var result = assetsPath.Take(count);
        Debug.Log($"result Count:{result1.Length}");
        Debug.Log($"result Count:{result2.Length}");
        //        var result = assetsPath.Take(count);
        _eventComponent.Subscribe(ReferencePool.Acquire<LoadAssetsCompleteEventArgs>().Id,_loadAssets);

        _resourceComponent.LoadAssets(result1, null, 0,
            new LoadAssetCallbacks(_loadAssetSuccessCallback2, _loadAssetFailureCallback, _loadDependency));

    }

    private void _loadAssets(object sender, GameEventArgs e)
    {
        _eventComponent.Unsubscribe(e.Id, _loadAssets);
        _allComplete(sender,e);

        _eventComponent.Subscribe(ReferencePool.Acquire<LoadAssetsCompleteEventArgs>().Id, _allComplete);

        _resourceComponent.LoadAssets(result2, null, 0,
            new LoadAssetCallbacks(_loadAssetSuccessCallback2, _loadAssetFailureCallback, _loadDependency));
    }

    private void Update()
    {
        _loadGroupCount2Update();
    }

    private void _loadGroupCount2Update()
    {
        if (!loadAssetMult)
        {
            return;
        }

        if (result1 != null)
        {
            if (completeCount == result1.Length)
            {
                completeCount = 0;
                result1 = null;
                _allComplete(null, null, 0, null);
                foreach (var s in result2)
                {
                    _resourceComponent.LoadAsset(s,
                        new LoadAssetCallbacks((assetName, asset, duration, data) =>
                        {
                            completeCount++;
                            _loadAssetSuccessCallback(assetName, asset, duration, data);
                        }));
                }

            }
        }
        
        if (result2 != null)
        {
            if (completeCount == result2.Length)
            {
                _allComplete(null, null, 0, null);
                completeCount = 0;
                result2 = null;
                loadAssetMult = false;

            }
        }
    }
   
    private int completeCount;
    private bool loadAssetMult;
    [ContextMenu("加载指定资源组-LoadAsset,分2次")]
    void _loadGroupCount2()
    {
        loadAssetMult = true;
        List<string> assetsPath = new List<string>();
        var assetNames = _resourceComponent.GetAssetGroupList(groupTag);
        foreach (var abName in assetNames)
        {
            if (abName.Contains("default"))
            {
                assetsPath = _resourceComponent.GetAssetsList(abName).ToList();
            }
        }

        var count = assetsPath.Count / loadCount;
        result1 = new string[count];
        result2 = new string[count];
        for (var i = 0; i < result1.Length; i++)
        {
            result1[i] = assetsPath[i];
        }
        for (var i = 0; i < result2.Length; i++)
        {
            result2[i] = assetsPath[i + count];
        }
        //        var result = assetsPath.Take(count);
        Debug.Log($"result Count:{result1.Length}");
        Debug.Log($"result Count:{result2.Length}");

        foreach (var s in result1)
        {
            _resourceComponent.LoadAsset(s,
                new LoadAssetCallbacks((assetName, asset, duration, data) =>
                {
                    completeCount++;
                    _loadAssetSuccessCallback(assetName, asset, duration, data);
                }));
        }
    }

    private void _loadDependency(string assetname, string dependencyassetname, int loadedcount, int totalcount, object userdata)
    {
        Debug.LogError($"资源{assetname},依赖资源:{dependencyassetname}");
    }


    private void _allComplete(IEnumerable<string> assetnames, IEnumerable<object> assets, float duration, object userdata)
    {
        Debug.LogError("All Complete");
        if (assets == null)
        {
            return;
        }
        Debug.LogError($"assets Count:{assets.Count()}");
        foreach (var asset in assets)
        {
            Instantiate((Object)asset);
        }
    }


    private void _checkOutGroup(object sender, GameEventArgs e)
    {
        _checkOutGroup();
    }

    void _loadAsset()
    {
        var eventArgs = ReferencePool.Acquire<Icarus.UnityGameFramework.Runtime.ResourceInitCompleteEventArgs>();
        _eventComponent.Subscribe(eventArgs.Id, _loadAsset);
        ReferencePool.Release(eventArgs);
        _resourceComponent.InitResources();
    }

    private void _loadAsset(object sender, GameEventArgs e)
    {
        _load();
        var eventArgs = ReferencePool.Acquire<Icarus.UnityGameFramework.Runtime.ResourceInitCompleteEventArgs>();
        //        _eventComponent.Unsubscribe(eventArgs.Id,_loadAsset);
        ReferencePool.Release(eventArgs);
    }

    [ContextMenu("加载资源")]
    void _load()
    {
        Debug.Log("资源加载完成");
        _resourceComponent.LoadAsset(AssetName, new LoadAssetCallbacks(_loadAssetSuccessCallback, _loadAssetFailureCallback, _loadAssetUpdateCallback));
    }

    private void _loadAssetUpdateCallback(string assetname, float progress, object userdata)
    {
        Debug.LogFormat("加载中.加载进度:{0}", progress);
    }

    private void _loadAssetFailureCallback(string assetname, LoadResourceStatus status, string errormessage, object userdata)
    {
        Debug.LogErrorFormat("加载失败.资源名:{0},状态:{1},错误信息:{2}", assetname, status, errormessage);
    }

    private void _loadAssetSuccessCallback(string assetname, object asset, float duration, object userdata)
    {
        var gameobject = (GameObject)asset;
        Instantiate(gameobject);
        Debug.LogErrorFormat("资源名为:{0},duration:{1}", assetname, duration);
    }

    private void _loadAssetSuccessCallback2(string assetname, object asset, float duration, object userdata)
    {
        Debug.LogErrorFormat("资源名为:{0},duration:{1}", assetname, duration);
    }

}
