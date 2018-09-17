//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月26日-09:22
//Icarus.UnityGameFramework.Runtime

using System.Text;
using System.Text.RegularExpressions;
using Icarus.GameFramework;
using Icarus.UnityGameFramework.Runtime;
using IGameFrameWork.UnityGameFramework.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.IGameFrameWork.UnityGameFramework.Runtime.I18N
{
    [System.Serializable]
    public class GetLocalizationEvent : UnityEvent<string> { }

    public class GetLocalizationComponent : MonoBehaviour
    {
        [TextArea]
        [SerializeField]
        private string _defaultStr = "--";

        [SerializeField]
        private string _key;

        [SerializeField]
        private bool _enableGet = true;

        public bool IsGetRawString = true;
        [ConditionalHide("IsGetRawString",true,true)]
        [SerializeField] private string[] _args;

        [SerializeField]
        private GetLocalizationEvent _onGetLocalizationEvent;

        private LocalizationComponent _localization;
        
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

        public GetLocalizationEvent OnGetLocalizationEvent
        {
            get
            {
                return _onGetLocalizationEvent;
            }

            set
            {
                _onGetLocalizationEvent = value;
            }
        }

        public string[] Args
        {
            get { return _args; }
            set { _args = value; }
        }

        void Start()
        {
            _localization = GameEntry.GetComponent<LocalizationComponent>();

            if (!_localization)
            {
                throw new GameFrameworkException("LocalizationComponent 没有注册到 GameEntry 中.");
            }
            GetI18NValue();
        }

        void OnEnable()
        {
            if (_enableGet && _localization)
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
            string str;
            
            if (IsGetRawString)
            {
                str = _localization.GetRawString(_key);
            }
            else
            {
                str = _localization.GetString(_key, _args);
            }

            if (string.IsNullOrEmpty(str))
            {
                str = _defaultStr;
            }

            _onGetLocalizationEvent.Invoke(str.EscapeReplace());
        }
    }
}