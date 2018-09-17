//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月26日-09:22
//Icarus.UnityGameFramework.Runtime

using System.Text;
using System.Text.RegularExpressions;
using Icarus.GameFramework;
using Icarus.UnityGameFramework.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.IGameFrameWork.UnityGameFramework.Runtime.I18N
{
    [System.Serializable]
    public class GetI18NEvent : UnityEvent<string> { }

    public class GetI18NComponent : MonoBehaviour
    {
        [TextArea]
        [SerializeField]
        private string _defaultStr = "--";

        [SerializeField]
        private string _key;

        [SerializeField]
        private bool _enableGet = true;

        [SerializeField]
        private GetI18NEvent _onGetI18NEvent;

        private I18NComponent _i18NComponent;
        
        public string Key
        {
            get
            {
                return _key;
            }

            set
            {
                _key = value;
            }
        }

        public GetI18NEvent OnGetI18NEvent
        {
            get
            {
                return _onGetI18NEvent;
            }

            set
            {
                _onGetI18NEvent = value;
            }
        }

        void Start()
        {
            _i18NComponent = GameEntry.GetComponent<I18NComponent>();

            if (!_i18NComponent)
            {
                throw new GameFrameworkException("I18NComponent 没有注册到 GameEntry 中.");
            }

            _i18NComponent.I18NManager.LanguageChange += _languageChange;
            GetI18NValue();
        }

        void OnEnable()
        {
            if (_enableGet && _i18NComponent)
            {
                GetI18NValue();
            }
        }

        private void _languageChange(object sender, Icarus.GameFramework.I18N.LanguageChangeEventArgs e)
        {
            GetI18NValue();
        }

        public void GetI18NValue()
        {
            var str = _i18NComponent.GetValue(_key);

            if (string.IsNullOrEmpty(str))
            {
                str = _defaultStr;
            }

            _onGetI18NEvent.Invoke(str.EscapeReplace());
        }
    }
}