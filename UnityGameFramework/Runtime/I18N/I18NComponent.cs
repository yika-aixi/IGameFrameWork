using Icarus.GameFramework.I18N;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Icarus.UnityGameFramework.Runtime.I18N;
using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Icarus/Game Framework/I18N")]
    public partial class I18NComponent : MonoBehaviour
    {
        private I18NManager _manager = new I18NManager();
        public I18NManager I18NManager => _manager;
        [SerializeField]
        private string _defaultLanguage;
        public string DefaultLanguage
        {
            get { return _defaultLanguage;}
            set { _defaultLanguage = value; }
        }

        [SerializeField]
        private bool _isFindLocal = true;
        /// <summary>
        /// 查找本地,默认为查找
        /// </summary>
        public bool IsFindLocal
        {
            get { return _isFindLocal;}
            set { _isFindLocal = value; }
        }

        [SerializeField]
        private string _directoryName = ConstTable.DefaultDirectoryName;
        /// <summary>
        /// 查找目录的名字
        /// </summary>
        public string DirectoryName
        {
            get
            {
                return _directoryName;
            }

            set
            {
                _directoryName = value;
            }
        }

        [SerializeField]
        private string _suffixName = ConstTable.SuffixName;
        /// <summary>
        /// 文件后缀
        /// </summary>
        public string SuffixName
        {
            get { return _suffixName; }
            set { _suffixName = value; }
        }

        /// <summary>
        /// 嵌入式语言表
        /// </summary>
        [SerializeField]
        public Dictionary<string, Dictionary<string, string>> ConvenienceLanguageTable
        {
            get;
            set;
        } = new Dictionary<string, Dictionary<string, string>>();

        void Awake()
        {
            GameEntry.RegisterComponent(this);
            if (string.IsNullOrWhiteSpace(DefaultLanguage))
            {
                I18NManager.SetCurrentLanguage(DefaultLanguage);
            }
        }

        void Start()
        {
            if (IsFindLocal)
            {
                var path = GameFramework.Utility.Path.GetCombinePath(Application.persistentDataPath, DirectoryName);
                var files = Directory.GetFiles(path, $"*.{SuffixName}", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    return;
                }

                _readFiles(files);

            }
        }

        private void _readFiles(string[] files)
        {
            foreach (var path in files)
            {
                var table = File.ReadAllLines(path);
                Dictionary<string, string> tempTable = new Dictionary<string, string>();

                for (var i = 1; i < table.Length; i++)
                {
                    var csv = table[i].Split('\t');
                    tempTable.Add(csv[0], csv[1]);
                }

                I18NManager.AddLanguageTable(table[0], tempTable);
            }
        }

        /// <summary>
        /// 增加当语言发生变化时执行的回调
        /// </summary>
        /// <param name="handle"></param>
        public void AddLanguageChangeEvent(EventHandler<LanguageChangeEventArgs> handle)
        {
            _manager.LanguageChange += handle;
        }

        /// <summary>
        /// 移除语言发生变化时执行的回调
        /// </summary>
        /// <param name="handle"></param>
        public void RemoveLanguageChangeEvent(EventHandler<LanguageChangeEventArgs> handle)
        {
            _manager.LanguageChange -= handle;
        }

        public void SetCurrentLanguage(string language)
        {
            _manager.SetCurrentLanguage(language);
        }

        /// <summary>
        /// 如果没有在文件中找到就会去快捷表找,如果找不到就返回string.Empty
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            var value = _manager.GetValue(key);

            if (string.IsNullOrEmpty(value))
            {
                if (ConvenienceLanguageTable[CurrentLanguage].ContainsKey(key))
                {
                    return ConvenienceLanguageTable[CurrentLanguage][key];
                }
            }

            return value;
        }

        /// <summary>
        /// 获取当前支持的语言表
        /// 文件 并集 快捷表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetLanguges()
        {
            var names = _manager.GetLanguges();
            var convenienceNames = ConvenienceLanguageTable.Keys;
            return names.Union(convenienceNames);
        }

        public string CurrentLanguage => _manager.CurrentLanguage;


    }

    public partial class I18NComponent : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<string> _languageNames = new List<string>();
        [SerializeField]
        private List<string> _keys = new List<string>();
        [SerializeField]
        private List<string> _values = new List<string>();

        public void OnBeforeSerialize()
        {
            _languageNames.Clear();
            //            _entity.Clear();
            _keys.Clear();
            _values.Clear();
            _languageNames.AddRange(ConvenienceLanguageTable.Keys);
            bool initKeys = false;
            foreach (var pair in ConvenienceLanguageTable)
            {
                if (!initKeys)
                {
                    _keys.AddRange(pair.Value.Keys);
                    initKeys = true;
                }
                foreach (var valuePair in pair.Value)
                {
                    _values.Add(valuePair.Value);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            ConvenienceLanguageTable = new Dictionary<string, Dictionary<string, string>>();
            int i = 0;
            foreach (var languageName in _languageNames)
            {
                ConvenienceLanguageTable.Add(languageName, new Dictionary<string, string>());
                foreach (var key in _keys)
                {
                    if (_values.Count <= i)
                    {
                        _values.Add(String.Empty);
                    }
                    ConvenienceLanguageTable[languageName].Add(key, _values[i]);
                    i++;
                }
            }
        }

        int _getValueIndex(int keyIndex)
        {
            return keyIndex * _keys.Count / _keys.Count;
        }
    }
}