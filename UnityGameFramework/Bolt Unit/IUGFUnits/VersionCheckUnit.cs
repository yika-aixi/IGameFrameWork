//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月23日-10:20
//Icarus.Chess.Bolt

using Bolt;
using Icarus.GameFramework.Version;
using Icarus.UnityGameFramework.Runtime;
using Ludiq;
using System;
using System.Collections.Generic;

namespace Icarus.UnityGameFramework.Bolt.Units
{
    [UnitCategory("Icarus/IUGF")]
    [UnitTitle("资源版本比较管理器")]
    [UnitSubtitle("arg在更新操作时是Url,在获取资源组时是tag")]
    public class VersionCheckUnit:Unit
    {
        [DoNotSerialize]
        [PortLabel("arg")]
        public ValueInput _argIn;

        [DoNotSerialize]
        [PortLabel("app更新地址")]
        public ValueInput _appUpdateUrlIn;

        [DoNotSerialize]
        [PortLabel("严格模式")]
        public ValueInput _strictMode; 

        [Serialize]
        [Inspectable, UnitHeaderInspectable("操作类型:")]
        [InspectorToggleLeft]
        public VersionCheckCallType Type { get; set; }

        [DoNotSerialize]
        [PortLabel("Headers")]
        public ValueInput _headersIn;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput _enter;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput _exit;

        [DoNotSerialize]
        [PortLabel("检查更新失败")]
        public ControlOutput _errorExit;

        [DoNotSerialize]
        [PortLabel("检查更新状态")]
        public ControlOutput _stateUpdateExit;

        [DoNotSerialize]
        [PortLabel("检查状态描述")]
        public ValueOutput _stateStrOut;

        [DoNotSerialize]
        [PortLabel("失败信息")]
        public ValueOutput _errorMessageOut;

        [DoNotSerialize]
        [PortLabel("result")]
        public ValueOutput _resultOut;


        private IEnumerable<AssetBundleInfo> _assetBundles;
        private bool _isUpdateGroup;
        private VersionInfo _versionInfo;
        private string _errorMessage;
        private string _stateStr;
        protected override void Definition()
        {
            _enter = ControlInput(nameof(_enter),__enter);
            _exit = ControlOutput(nameof(_exit));

           

            switch (Type)
            {
                case VersionCheckCallType.检查更新:
                case VersionCheckCallType.判断资源组是否需要更新:
                case VersionCheckCallType.获取资源组更新列表:
                    _argIn = ValueInput<string>(nameof(_argIn));
                    Requirement(_argIn, _enter);
                    break; 
            }
            
            switch (Type)
            {
                case VersionCheckCallType.检查更新:
                case VersionCheckCallType.获取资源组更新列表:
                    _resultOut = ValueOutput(nameof(_resultOut), x => _assetBundles);
                    break;
                case VersionCheckCallType.获取服务器版本信息:
                case VersionCheckCallType.获取持久化目录版本信息:
                case VersionCheckCallType.获取本地最新版本信息:
                    _resultOut = ValueOutput(nameof(_resultOut), x => _versionInfo);
                    break;
                case VersionCheckCallType.判断资源组是否需要更新:
                    _resultOut = ValueOutput(nameof(_resultOut), x => _isUpdateGroup);
                    break;
            }

            if (Type == VersionCheckCallType.检查更新 ||
                Type == VersionCheckCallType.获取资源组更新列表)
            {
                _strictMode = ValueInput(nameof(_strictMode),true);
            }

            if (Type == VersionCheckCallType.检查更新)
            {
                _headersIn = ValueInput<Dictionary<string, string>>(nameof(_headersIn));
                _appUpdateUrlIn = ValueInput<string>(nameof(_appUpdateUrlIn));
                _errorExit = ControlOutput(nameof(_errorExit));
                _errorMessageOut = ValueOutput(nameof(_errorMessageOut), x => _errorMessage);
                _stateUpdateExit = ControlOutput(nameof(_stateUpdateExit));
                _stateStrOut = ValueOutput(nameof(_stateStrOut), x => _stateStr);
                Requirement(_appUpdateUrlIn,_enter);
                Succession(_enter, _errorExit);
            }

            Succession(_enter, _exit);

        }

        private ControlOutput __enter(Flow flow)
        {
            var version = GameEntry.GetComponent<DefaultVersionCheckCompontent>();

            string arg = "";

            if (Type != VersionCheckCallType.获取本地最新版本信息 ||
                Type != VersionCheckCallType.获取服务器版本信息 ||
                Type != VersionCheckCallType.获取持久化目录版本信息)
            {
                arg = flow.GetValue<string>(_argIn);
            }

            if (Type == VersionCheckCallType.检查更新)
            {
                version.Url = arg;
            }

            if (Type == VersionCheckCallType.检查更新 ||
                Type == VersionCheckCallType.获取资源组更新列表)
            {
                version.StrictMode = flow.GetValue<bool>(_strictMode);
            }

            switch (Type)
            {
                case VersionCheckCallType.检查更新:
                    var headers = flow.GetValue <Dictionary<string, string>>(_headersIn);
                    var appUpdateUrl = flow.GetValue<string>(_appUpdateUrlIn);
                    Flow fl = Flow.New(flow.stack.ToReference());
                    version.Check(headers, x =>
                    {
                        _assetBundles = x;
                        fl.Invoke(_exit);
                        fl.Dispose();
                    }, () => appUpdateUrl,ex =>
                    {
                        _errorMessage = ex;
                        fl.Invoke(_errorExit);
                        fl.Dispose();
                    }, x =>
                    {
                        _stateStr = x;
                        fl.Invoke(_stateUpdateExit);
                    });
                    return null;
                case VersionCheckCallType.获取服务器版本信息:
                    _versionInfo = version.ServerVersionInfo;
                    return _exit;
                case VersionCheckCallType.获取持久化目录版本信息:
                    _versionInfo = version.PersistentInfos;
                    return _exit;
                case VersionCheckCallType.获取本地最新版本信息:
                    _versionInfo = version.LocalVersionInfo;
                    return _exit;
                case VersionCheckCallType.判断资源组是否需要更新:
                    _isUpdateGroup = version.IsUpdateGroup(arg);
                    return _exit;
                case VersionCheckCallType.获取资源组更新列表:
                    _assetBundles = version.GetGroupVersion(arg);
                    return _exit;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}