//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月08日 21:40:22
//Assembly-CSharp

using Bolt;
using Icarus.GameFramework;
using Icarus.GameFramework.Event;
using Icarus.GameFramework.Resource;
using Icarus.UnityGameFramework.Runtime;
using Ludiq;
using System;
using System.Collections;
using System.Collections.Generic;
using ResourceInitCompleteEventArgs = Icarus.UnityGameFramework.Runtime.ResourceInitCompleteEventArgs;

namespace Icarus.UnityGameFramework.Bolt.Units
{
    [UnitCategory("Icarus/IUGF")]
    [UnitTitle("Asset Manager")]
    [UnitSubtitle("资源管理,'资源名'同时也是'资源组Tag'或'AB名'或'场景名'")]
    public class AssetsManagerUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput Enter;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput Exit;

        [Serialize]
        [Inspectable, UnitHeaderInspectable("操作类型:")]
        [InspectorToggleLeft]
        public AssetManagerCallType Type { get; set; }

        [DoNotSerialize]
        [PortLabel("资源名")]
        public ValueInput AssetName;

        [DoNotSerialize]
        [PortLabel("Unload Asset")]
        public ValueInput UnloadAsset;

        [DoNotSerialize]
        [PortLabel("优先级")]
        public ValueInput Priority;

        [DoNotSerialize]
        [PortLabel("执行GC.Collect")]
        public ValueInput PerformGCCollect;

        [DoNotSerialize]
        [PortLabel("资源")]
        public ValueOutput Asset;

        [DoNotSerialize]
        [PortLabel("是否存在")]
        public ValueOutput Exist;

        [DoNotSerialize]
        [PortLabel("结果列表")]
        public ValueOutput ResultList;

        [DoNotSerialize]
        [PortLabel("失败信息")]
        public ValueOutput ErrorMessage;

        [DoNotSerialize]
        [PortLabel("加载进度(0-1)")]
        public ValueOutput Progress;

        [DoNotSerialize]
        [PortLabel("执行完成")]
        public ControlOutput CompleteExit;

        [DoNotSerialize]
        [PortLabel("加载单个完成")]
        public ControlOutput anyLoadCompleteExit;

        [DoNotSerialize]
        [PortLabel("加载中")]
        public ControlOutput ProgressExit;

        [DoNotSerialize]
        [PortLabel("依赖资源名")]
        public ValueOutput DependencyName;

        [DoNotSerialize]
        [PortLabel("加载依赖")]
        public ControlOutput DependencyExit;

        [DoNotSerialize]
        [PortLabel("失败")]
        public ControlOutput ErrorExit;

        [Serialize]
        [Inspectable, UnitHeaderInspectable("Load Asset Type:")]
        [InspectorToggleLeft]
        public Type AssetType = typeof(UnityEngine.Object);


        private object _asset;
        private string _errorMessage;
        private string _dependencyName;
        private IEnumerable _resultList;
        private float _progress;
        private bool _exist;

        protected override void Definition()
        {
            Enter = ControlInput(nameof(Enter), _enter);
            Exit = ControlOutput(nameof(Exit));
            if (Type == AssetManagerCallType.加载单个资源)
            {
                anyLoadCompleteExit = ControlOutput(nameof(anyLoadCompleteExit));
            }

            switch (Type)
            {
                case AssetManagerCallType.加载多个资源:
                case AssetManagerCallType.加载场景:
                case AssetManagerCallType.卸载场景:
                    CompleteExit = ControlOutput(nameof(CompleteExit));
                    break;
            }

            switch (Type)
            {
                case AssetManagerCallType.加载单个资源:
                case AssetManagerCallType.加载场景:
                case AssetManagerCallType.卸载场景:
                case AssetManagerCallType.判断资源是否存在:
                case AssetManagerCallType.获取资源包资源列表:
                case AssetManagerCallType.获取资源组资源包列表:
                    AssetName = ValueInput<string>(nameof(AssetName));
                    Requirement(AssetName, Enter);
                    break;
            }

            if (Type == AssetManagerCallType.加载多个资源)
            {
                AssetName = ValueInput<IEnumerable<string>>(nameof(AssetName));
                Requirement(AssetName, Enter);

                Asset = ValueOutput(AssetType, nameof(Asset), x => _asset);

                Requirement(AssetName, Asset);

                anyLoadCompleteExit = ControlOutput(nameof(anyLoadCompleteExit));

                ResultList = ValueOutput(nameof(ResultList), x => _resultList);

                Requirement(AssetName, Asset);

            }

            switch (Type)
            {
                case AssetManagerCallType.强制释放未被使用资源:
                case AssetManagerCallType.预订执行释放未被使用资源:
                    PerformGCCollect = ValueInput<bool>(nameof(PerformGCCollect));
                    break;
            }

            switch (Type)
            {
                case AssetManagerCallType.加载单个资源:
                case AssetManagerCallType.加载多个资源:
                case AssetManagerCallType.加载场景:
                    Priority = ValueInput(nameof(Priority), 0);
                    DependencyExit = ControlOutput(nameof(DependencyExit));
                    DependencyName = ValueOutput(nameof(DependencyName), x => _dependencyName);
                    ProgressExit = ControlOutput(nameof(ProgressExit));
                    Progress = ValueOutput(nameof(Progress), x => _progress);
                    Succession(Enter, ProgressExit);
                    break;
            }
            
            switch (Type)
            {
                case AssetManagerCallType.加载单个资源:
                case AssetManagerCallType.加载多个资源:
                case AssetManagerCallType.加载场景:
                case AssetManagerCallType.卸载场景:
                    ErrorExit = ControlOutput(nameof(ErrorExit));
                    ErrorMessage = ValueOutput(nameof(ErrorMessage), x => _errorMessage);
                    Succession(Enter, ErrorExit);
                    break;
                case AssetManagerCallType.判断资源是否存在:
                    Exist = ValueOutput(nameof(Exist), x => _exist);
                    break;
                case AssetManagerCallType.获取资源包资源列表:
                case AssetManagerCallType.获取资源组资源包列表:
                case AssetManagerCallType.获取所有资源组:
                    ResultList = ValueOutput(nameof(ResultList), x => _resultList);
                    break;
            }

            if (Type == AssetManagerCallType.加载单个资源)
            {
                Asset = ValueOutput(AssetType, nameof(Asset), x => _asset);
                Requirement(AssetName, Asset);
            }

            if (Type == AssetManagerCallType.卸载资源)
            {
                UnloadAsset = ValueInput<object>(nameof(UnloadAsset));
            }

            Succession(Enter, Exit);
        }

        private ControlOutput _enter(Flow flow)
        {
            var resource = GameEntry.GetComponent<ResourceComponent>();
            var scene = GameEntry.GetComponent<SceneComponent>();
            var @event = GameEntry.GetComponent<EventComponent>();

            if (!resource)
            {
                throw new Exception("ResourceComponent 没有注册到 GameEntry");
            }

            if (!scene)
            {
                throw new Exception("SceneComponent 没有注册到 GameEntry");
            }

            if (!@event)
            {
                throw new Exception("EventComponent 没有注册到 GameEntry");
            }

            string assetName = "";

            switch (Type)
            {
                case AssetManagerCallType.加载单个资源:
                case AssetManagerCallType.加载场景:
                case AssetManagerCallType.卸载场景:
                case AssetManagerCallType.判断资源是否存在:
                case AssetManagerCallType.获取资源包资源列表:
                case AssetManagerCallType.获取资源组资源包列表:
                    assetName = flow.GetValue<string>(AssetName);
                    break;
            }

            int priority = 0;
            switch (Type)
            {
                case AssetManagerCallType.加载单个资源:
                case AssetManagerCallType.加载场景:
                    priority = flow.GetValue<int>(Priority);
                    break;
            }

           

            bool performGCCollect = false;

            switch (Type)
            {
                case AssetManagerCallType.强制释放未被使用资源:
                case AssetManagerCallType.预订执行释放未被使用资源:
                    performGCCollect = flow.GetValue<bool>(PerformGCCollect);
                    break;
            }

            object unloadAsset = null;

            if (Type == AssetManagerCallType.卸载资源)
            {
                unloadAsset = flow.GetValue<object>(UnloadAsset);
            }

            Flow fl = Flow.New(flow.stack.ToReference());
            switch (Type)
            {
                case AssetManagerCallType.加载单个资源:
                    resource.LoadAsset(assetName, AssetType, priority,_getLoadAssetCallbacks(fl,true));
                    break;
                case AssetManagerCallType.加载多个资源:
                    var names = flow.GetValue<IEnumerable<string>>(AssetName);
                    resource.LoadAssets(names, AssetType, priority,
                        (assetNames, assets, duration, data) =>
                        {
                            _resultList = assets;
                            fl.Invoke(CompleteExit);
                            fl.Dispose();
                        }, _getLoadAssetCallbacks(fl,false));
                    break;
                case AssetManagerCallType.加载场景:
                    var id = _getEventID<LoadSceneSuccessEventArgs>();
                    @event.Subscribe(id,
                        (sender, args) =>
                        {
                            _loadSceneComplete(fl, sender, args);
                            @event.Unsubscribe(id, (o, eventArgs) => _loadSceneComplete(flow, o, eventArgs));
                        });

                    id = _getEventID<LoadSceneFailureEventArgs>();
                    @event.Subscribe(id,
                        (sender, args) =>
                        {
                            _loadSceneFailure(fl, sender, args);
                            @event.Unsubscribe(id, (o, eventArgs) => _loadSceneFailure(flow, o, eventArgs));
                        });

                    id = _getEventID<LoadSceneUpdateEventArgs>();
                    @event.Subscribe(id,
                        (sender, args) =>
                        {
                            _loadSceneUpdate(fl, sender, args);
                            @event.Unsubscribe(id, (o, eventArgs) => _loadSceneUpdate(flow, o, eventArgs));
                        });

                    id = _getEventID<LoadSceneDependencyAssetEventArgs>();
                    @event.Subscribe(id,
                        (sender, args) =>
                        {
                            _loadSceneDependencyAsset(fl, sender, args);

                            @event.Unsubscribe(id, (o, eventArgs) => _loadSceneDependencyAsset(flow, o, eventArgs));
                        });


                    scene.LoadScene(assetName, priority);
                    break;
                case AssetManagerCallType.卸载资源:
                    resource.UnloadAsset(unloadAsset);
                    break;
                case AssetManagerCallType.卸载场景:

                    id = _getEventID<UnloadSceneSuccessEventArgs>();

                    @event.Subscribe(id, (sender, args) =>
                    {
                        _unloadSceneSuccess(fl, sender, args);
                        @event.Unsubscribe(id, (o, eventArgs) => { _unloadSceneSuccess(flow, o, eventArgs); });
                    });


                    id = _getEventID<UnloadSceneFailureEventArgs>();
                    @event.Subscribe(id, (sender, args) =>
                    {
                        _unloadSceneFailure(fl, sender, args);
                        @event.Unsubscribe(id, (o, eventArgs) => { _unloadSceneFailure(flow, o, eventArgs); });
                    });

                    scene.UnloadScene(assetName);
                    break;
                case AssetManagerCallType.判断资源是否存在:
                    _exist = resource.ExistAsset(assetName);
                    break;
                case AssetManagerCallType.获取资源包资源列表:
                    _resultList = resource.GetAssetsList(assetName);
                    break;
                case AssetManagerCallType.获取资源组资源包列表:
                    _resultList = resource.GetAssetGroupList(assetName);
                    break;
                case AssetManagerCallType.获取所有资源组:
                    _resultList = resource.GetAllGroupList();
                    break;
                case AssetManagerCallType.强制释放未被使用资源:
                    resource.ForceUnloadUnusedAssets(performGCCollect);
                    break;
                case AssetManagerCallType.预订执行释放未被使用资源:
                    resource.UnloadUnusedAssets(performGCCollect);
                    break;
                case AssetManagerCallType.初始化:

                    var baseC = GameEntry.GetComponent<BaseComponent>();

                    if (baseC.EditorResourceMode)
                    {
                        return CompleteExit;
                    }

                    id = _getEventID<ResourceInitCompleteEventArgs>();

                    @event.Subscribe(id, (sender, args) =>
                    {
                        _initComplete(fl, sender, args);
                        @event.Unsubscribe(id, (o, eventArgs) =>
                        {
                            _initComplete(fl, sender, args);
                        });
                    });

                    resource.InitResources();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Exit;
        }

        private LoadAssetCallbacks _getLoadAssetCallbacks(Flow fl,bool successIsFlowDisplay)
        {
            return new LoadAssetCallbacks
            ((name, asset, duration, data) =>
            {
                _asset = asset;
                fl.Invoke(anyLoadCompleteExit);
                if(successIsFlowDisplay)
                {
                    fl.Dispose();
                }
            }, (name, status, message, data) =>
            {
                _errorMessage = $"加载资源失败,状态为:{status},错误信息:{message}";
                fl.Invoke(ErrorExit);
                if (successIsFlowDisplay)
                {
                    fl.Dispose();
                }
            }, (name, progress, data) =>
            {
                _progress = progress;
                fl.Invoke(ProgressExit);
            }, (name, dependencyAssetName, count, totalCount, data) =>
            {
                _dependencyName = dependencyAssetName;
                fl.Invoke(DependencyExit);
            });
        }

        private void _initComplete(Flow fl, object sender, GameEventArgs args)
        {
            fl.Invoke(CompleteExit);
            fl.Dispose();
        }

        private void _unloadSceneFailure(Flow flow, object sender, GameEventArgs args)
        {
            var failureArgs = (UnloadSceneFailureEventArgs)sender;
            _errorMessage = $"卸载{failureArgs.SceneAssetName}场景失败.";
            flow.Invoke(ErrorExit);
            flow.Dispose();
        }

        private void _unloadSceneSuccess(Flow flow, object sender, GameEventArgs args)
        {
            flow.Invoke(CompleteExit);
            flow.Dispose();
        }

        private int _getEventID<T>() where T : GameEventArgs, new()
        {
            var args = ReferencePool.Acquire<T>();
            var id = args.Id;
            ReferencePool.Release(args);
            return id;
        }

        private void _loadSceneDependencyAsset(Flow flow, object sender, GameEventArgs args)
        {
            var dependency = (LoadSceneDependencyAssetEventArgs)sender;
            _dependencyName = dependency.DependencyAssetName;
            flow.Invoke(DependencyExit);
        }

        private void _loadSceneUpdate(Flow flow, object sender, GameEventArgs args)
        {
            var sceneUpdate = (LoadSceneUpdateEventArgs)sender;
            _progress = sceneUpdate.Progress;
            flow.Invoke(ProgressExit);
        }

        private void _loadSceneFailure(Flow flow, object sender, GameEventArgs args)
        {
            var failure = (LoadSceneFailureEventArgs)sender;
            _errorMessage = $"加载{failure.SceneAssetName}场景失败,失败信息:{failure.ErrorMessage}";
            flow.Invoke(ErrorExit);
            flow.Dispose();
        }

        private void _loadSceneComplete(Flow flow, object sender, GameEventArgs e)
        {
            flow.Invoke(CompleteExit);
            flow.Dispose();
        }
    }
}