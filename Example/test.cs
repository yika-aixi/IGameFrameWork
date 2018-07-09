using System;
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

public class test : MonoBehaviour
{
    public string AssetName;
    public UpdateInfo Info;
    public bool 严格模式 = true;
    private EventComponent _eventComponent;
    private BaseComponent _baseComponent;
    private ResourceComponent _resourceComponent;
    private SceneComponent _sceneComponent;

    private DefaultVersionCheckComPontent _versionCheck;

    private DefaultUpdateAssetBundleComponent _update;
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
            //进行资源版本检测
            _versionCheck = GameEntry.GetComponent<DefaultVersionCheckComPontent>();
            if (!_versionCheck)
            {
                Debug.LogError("Default VersionCheck ComPontent is invalid.");
                return;
            }

            _versionCheck.Url = Info.AssetBundleUrl+"/"+ConstTable.VersionFileName;
            _versionCheck.Check(严格模式, x =>
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
                _UpdateAssetbundle(x, ()=> { _loadAsset1(); });
                
            }, errorHandle: Debug.LogError,stateUpdateHandle: Debug.Log);
            
        }
        else
        {
            _load();
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
    ,Action allCompleteHandle = null)
    {
        _update.UpdateAssetBundle(Info, abs, _versionCheck.PersistentInfos,_versionCheck.ServerVersionInfo.Version, (pro, str) =>
        {
            Debug.Log($"下载进度：{pro.Progress}，下载速度：{pro.Speed},下载描述：{str}");
        }, x1 =>
        {
            Debug.Log("更新完成：" + x1);
        }, ()=>
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
        //_resourceComponent.LoadAsset(AssetName, new LoadAssetCallbacks(_loadAssetSuccessCallback,  _loadAssetFailureCallback, _loadAssetUpdateCallback));
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
        var gameobject = (GameObject) asset;
        Instantiate(gameobject);
        Debug.LogFormat("资源名为:{0},duration:{1}", assetname, duration);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
