//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月23日-11:22
//Icarus.Chess.Bolt

using System.Collections.Generic;
using Bolt;
using Icarus.GameFramework.Download;
using Icarus.GameFramework.UpdateAssetBundle;
using Icarus.GameFramework.Version;
using Icarus.UnityGameFramework.Runtime;
using Ludiq;

namespace Icarus.UnityGameFramework.Bolt
{
    [UnitCategory("Icarus/IUGF")]
    [UnitTitle("资源更新")]
    public class UpdateAssetBundleUnit:Unit
    {
        [PortLabelHidden]
        public ControlInput _enter;

        [PortLabel("资源包更新Url")]
        [Serialize]
        public ValueInput _UrlIn;

        [PortLabel("更新列表")]
        public ValueInput _assetBundleifInfos;
         
        [PortLabel("更新进度")]
        public ControlOutput _progressExit;

        [PortLabel("更新进度信息")]
        public ValueOutput _progressInfoOut;

        [PortLabel("更新进度描述")]
        public ValueOutput _progressStrOut;
        
        [PortLabel("更新完成一个")]
        public ControlOutput _anyCompleteExit;

        [PortLabel("更新完成一个")]
        public ValueOutput _assetBundleInfoOut;

        [PortLabel("全部更新完成")]
        public ControlOutput _allCompleteExit;
        
        [PortLabel("更新异常")]
        [Serialize]
        public ControlOutput _errorExit;

        [PortLabel("更新错误信息")]
        [Serialize]
        public ValueOutput _errorMessageOut;

        private DownloadProgressInfo _downloadProgress;
        private string _downloadProgressStr;
        private AssetBundleInfo _assetBundleInfo;
        private string _errorMessage;

        protected override void Definition()
        {
            _enter = ControlInput(nameof(_enter), ___enter);
            _allCompleteExit = ControlOutput(nameof(_allCompleteExit));
            _UrlIn = ValueInput<string>(nameof(_UrlIn));
            _assetBundleifInfos = ValueInput<IEnumerable<AssetBundleInfo>>(nameof(_assetBundleifInfos));

            {
                _progressExit = ControlOutput(nameof(_progressExit));
                _progressInfoOut = ValueOutput(nameof(_progressInfoOut),x=>_downloadProgress);
                _progressStrOut = ValueOutput(nameof(_progressStrOut),x=>_downloadProgressStr);
            }

            {
                _anyCompleteExit = ControlOutput(nameof(_anyCompleteExit));
                _assetBundleInfoOut = ValueOutput(nameof(_assetBundleInfoOut),x=>_assetBundleInfo);
            }
            
            {
                _errorExit = ControlOutput(nameof(_errorExit));
                _errorMessageOut = ValueOutput(nameof(_errorMessageOut),x=>_errorMessage);
            }
            
        }

        private ControlOutput ___enter(Flow flow)
        {
            var update = GameEntry.GetComponent<DefaultUpdateAssetBundleComponent>();
            var check = GameEntry.GetComponent<DefaultVersionCheckCompontent>();
            var url = flow.GetValue<string>(_UrlIn);
            var abList = flow.GetValue<IEnumerable<AssetBundleInfo>>(_assetBundleifInfos);
            Flow fl = Flow.New(flow.stack.ToReference());
            update.UpdateAssetBundle(new UpdateInfo()
            {
                AssetBundleUrl = url,
            }, abList, check.PersistentInfos, check.ServerVersionInfo.Version, (x, y) =>
            {
                _downloadProgress = x;
                _downloadProgressStr = y;
                fl.Invoke(_progressExit);
            }, x =>
            {
                _assetBundleInfo = x;
                fl.Invoke(_anyCompleteExit);
            },()=>
            {
                fl.Invoke(_allCompleteExit);
                fl.Dispose();
            }, ex =>
            {
                _errorMessage = ex;
                fl.Invoke(_errorExit);
                fl.Dispose();
            });

            return null;
        }
    }
}