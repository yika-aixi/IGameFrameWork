//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2018年09月15日-04:06
//Icarus.UnityGameFramework.Editor

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Icarus.GameFramework;
using Icarus.GameFramework.Localization;
using Icarus.UnityGameFramework.Runtime.I18N;
using IGameFrameWork.UnityGameFramework.Runtime;
using UnityEditor;
using UnityEngine;

namespace Icarus.UnityGameFramework.Editor
{
    public class LocalizationWindow : EditorWindow
    {
        [MenuItem("Icarus/Game Framework/Localization/Localization Tools", false, 31)]
        public static void Open()
        {
            var window = GetWindow<LocalizationWindow>(true, "多语言管理工具 - Localization", true);
            window.minSize = new Vector2(600, 630f);
        }

        private readonly Dictionary<string, List<KeyValuePair<string, string>>> _languages =
            new Dictionary<string, List<KeyValuePair<string, string>>>();

        private readonly Dictionary<string, bool> _showState = new Dictionary<string, bool>();
        private Language _languageSelect;

        private string _autoExportKey = "Localization_autoExportKey";
        private bool _autoExport = false;
        private string _deleteFileKey = "Localization_deleteFile";
        private bool _deleteFile = false;

        public void OnGUI()
        {
            if (_LocalizationCreateTypes == null || _LocalizationCreateTypes.Length == 0)
            {
                EditorGUILayout.HelpBox($"没有找到实现,请增加{typeof(LocalizationCreateAndParseBase).AssemblyQualifiedName}的实现",
                    MessageType.Warning);

                return;
            }

            _createTypeSelectIndex = EditorGUILayout.Popup(new GUIContent("LocalizationCreate Type:"),
                _createTypeSelectIndex,
                _LocalizationCreateTypes);


            _drowDeleteFile();

            //绘制自动导出
            _drowAutoExport();

            //绘制添加语言
            _drowAddLanguage();

            //子目录设置
            _setChildDirectory();

            //文件名设置
            _setFileName();

            //更新创建解析器实例
            _instantiationCreateType();

            //文件操作,读取或导出
            _fileLoadAndExport();

            _openFolder();
            
            //添加条目
            _addItem();

            //同步语言配置条目
            _syncItem();

            //绘制语言表
            _drowLanguageList();
        }

        private void _openFolder()
        {
            if (GUILayout.Button("Open Load Folder",GUILayout.Height(25f)))
            {
                OpenFolder.Open(_loadPath);
            }
        }

        private const string _fileNameKey = "Localization_fileNameKeyy";

        private void _setFileName()
        {
            _fileName = EditorGUILayout.TextField("文件名:", _fileName);
            EditorPrefs.SetString(_fileNameKey, _fileName);
        }

        private void _drowDeleteFile()
        {
            _deleteFile = EditorGUILayout.Toggle("删除语言时同时删除配置文件", _deleteFile);
            EditorPrefs.SetBool(_deleteFileKey, _deleteFile);
        }

        private void _drowAutoExport()
        {
            _autoExport = EditorGUILayout.Toggle("Auto Export", _autoExport);

            if (GUI.changed)
            {
                EditorPrefs.SetBool(_autoExportKey, _autoExport);
            }
        }

        private void _syncItem()
        {
            foreach (var language in _languages)
            {
                foreach (var pair in language.Value)
                {
                    if (!_keys.Exists(x=>x == pair.Key))
                    {
                        _keys.Add(pair.Key);
                    }    
                }
            }


            for (var i = 0; i < _keys.Count; i++)
            {
                var key = _keys[i];
                foreach (var language in _languages)
                {
                    if (language.Value.Count <= i)
                    {
                        language.Value.Add(new KeyValuePair<string, string>(key,string.Empty));
                    }
                    
                    if (language.Value[i].Key != key)
                    {
                        language.Value.Insert(i,new KeyValuePair<string, string>(key,string.Empty));
                        continue;
                    }
                    
                    language.Value[i] = new KeyValuePair<string, string>(key,language.Value[i].Value);
                }
            }
        }

        private void _drowAddLanguage()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _languageSelect = (Language) EditorGUILayout.EnumPopup("Language", _languageSelect);
                if (GUILayout.Button("添加所选语言"))
                {
                    if (_languageSelect == Language.Unspecified)
                    {
                        return;
                    }

                    _createLanguage(_languageSelect.ToString());
                }
            }
            EditorGUILayout.EndHorizontal();
            if (_languageSelect == Language.Unspecified)
            {
                EditorGUILayout.HelpBox("没有指定语言,无法进行添加", MessageType.Warning);
            }
        }

        private const string _childDirectoryNameKey = "Localization_childDirectoryNameKey";

        private void _setChildDirectory()
        {
            _childDirectoryName = EditorGUILayout.TextField("子目录名(导出)", _childDirectoryName);
            EditorPrefs.SetString(_childDirectoryNameKey, _childDirectoryName);
        }

        private int _oldSelectIndex = -1;
        private LocalizationCreateAndParseBase _localizationCreate;

        private void _instantiationCreateType()
        {
            if (_oldSelectIndex != _createTypeSelectIndex || _localizationCreate == null)
            {
                System.Type createType = _getType(_LocalizationCreateTypes[_createTypeSelectIndex]);

                _localizationCreate = (LocalizationCreateAndParseBase) Activator.CreateInstance(createType);
                _oldSelectIndex = _createTypeSelectIndex;
            }
        }

        private System.Type _getType(string type)
        {
            return GameFramework.Utility.Assembly.GetType(type);
        }

        private string[] _LocalizationCreateTypes;

        private void OnEnable()
        {
            _initAutoExport();
            _setLocalizationCreate();
            _instantiationCreateType();
            _initChildDirectoryName();
            _initLoad();
            _initFileName();
            _initDeleteFile();
        }

        private void _initAutoExport()
        {
            if (EditorPrefs.HasKey(_autoExportKey))
            {
                _autoExport = EditorPrefs.GetBool(_autoExportKey);
            }
        }

        private void _initDeleteFile()
        {
            if (EditorPrefs.HasKey(_deleteFileKey))
            {
                _deleteFile = EditorPrefs.GetBool(_deleteFileKey);
            }
        }

        private string _loadPath;

        private void _initLoad()
        {
            if (EditorPrefs.HasKey(_pathKey))
            {
                _loadPath = EditorPrefs.GetString(_pathKey);
            }
            else
            {
                _loadPath = Application.dataPath;
            }

            //如果存在路径,并且路径不是空的,就进行初始化读取
            if (!string.IsNullOrWhiteSpace(_loadPath))
            {
                if (Directory.Exists(_loadPath))
                {
                    _readDirectory(_loadPath);
                }
            }
        }

        private void _initFileName()
        {
            if (EditorPrefs.HasKey(_fileNameKey))
            {
                _fileName = EditorPrefs.GetString(_fileNameKey);
            }
        }

        private void _initChildDirectoryName()
        {
            if (EditorPrefs.HasKey(_childDirectoryNameKey))
            {
                _childDirectoryName = EditorPrefs.GetString(_childDirectoryNameKey);
            }
        }

        private void _setLocalizationCreate()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var result = assemblies.Where(x => !x.GetName().FullName.Contains("System"))
                .Select(x => x.GetName().FullName).ToArray();
            _LocalizationCreateTypes = Type.GetTypeNames(typeof(LocalizationCreateAndParseBase), result);
        }

        bool _isClickEnter()
        {
            return IsEnterClick();
        }

        protected bool IsEnterClick(bool isUsed = true)
        {
            if (!IsKeyClick(KeyCode.Return, isUsed) && !IsKeyClick(KeyCode.KeypadEnter, isUsed)) return false;

            GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
            if (isUsed)
            {
                Event.current.Use(); // Ignore event, otherwise there will be control name conflicts!
            }

            return true;
        }

        protected bool IsKeyClick(KeyCode code, bool isUsed = true)
        {
            if (!Event.current.isKey)
                return false;

            if (Event.current.type != EventType.KeyUp)
            {
                if (isUsed)
                {
                    if (Event.current.type != EventType.Used)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return Event.current.keyCode == code;
        }


        private void _addItem()
        {
            if (_languages.Count > 0)
            {
                //添加数据
                EditorGUILayout.BeginHorizontal();
                {
                    _key = EditorGUILayout.TextField("添加条目：", _key, GUILayout.Height(20f));
                    if (GUILayout.Button("+") || _isClickEnter())
                    {
                        if (string.IsNullOrEmpty(_key))
                        {
                            return;
                        }

                        _addItem(_key);

                        _autoExportUpdate();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void _fileLoadAndExport()
        {
            _load();

            if (_languages.Count > 0)
            {
                _export();
            }
        }

        private void _load()
        {
            if (GUILayout.Button("读取", GUILayout.Height(25)))
            {
                var path = _openSelectFolderPath("读取目录", true);
                _loadPath = path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }

                _readDirectory(path);
            }
        }

        private void _readDirectory(string path)
        {
            var filePaths =
                Directory.GetFiles(path, $"{_fileName}.{_localizationCreate.ExtensionName}",
                    SearchOption.AllDirectories);
            if (filePaths.Length <= 0)
            {
                return;
            }

            _clear();

            foreach (var filePath in filePaths)
            {
                var fileContent = File.ReadAllText(filePath);
                var languageName = _getLanguageName(filePath);
                _addLanguage(languageName);

                Dictionary<string,string> dict = new Dictionary<string, string>();
                
                _localizationCreate.Parse(fileContent, dict, null);
                
                foreach (var pair in dict)
                {
                    _languages[languageName].Add(new KeyValuePair<string, string>(pair.Key,pair.Value));
                }
            }
        }

        private string _getLanguageName(string filePath)
        {
            var RegularPathLoadPath = Utility.Path.GetRegularPath(_loadPath);
            var RegularPathFilePath = Utility.Path.GetRegularPath(filePath);
            var fileName = Path.GetFileName(RegularPathFilePath);
            var dir = Utility.Path.GetRegularPath(Path.GetDirectoryName(RegularPathFilePath));
            dir = dir.Replace(RegularPathLoadPath, "");
            dir = dir.Replace(fileName, "");
            dir = dir.Remove(0, 1);
            dir = dir.Remove(dir.Length - 1, 1);
            var languageName = dir.Split('/')[0];
            return languageName;
        }

        private void _addItem(string key)
        {
            if (!_keys.Exists(x=>x == key))
            {
                _keys.Add(key);
            }
//            foreach (var language in _languages)
//            {
//                if (!_languages[language.Key].ContainsKey(key))
//                {
//                    _languages[language.Key].Add(key, string.Empty);
//                }
//            }
        }

        private void _clear()
        {
            _languages.Clear();
            _keys.Clear();
            _showState.Clear();
        }

        private string _pathKey = "saveAndLoadPath-Window";
        private int _createTypeSelectIndex;
        private int _helperTypeSelectIndex;
        private string _childDirectoryName = "Dictionaries";

        private void _export()
        {
            if (GUILayout.Button("导出", GUILayout.Height(25)))
            {
                var path = _openSelectFolderPath("导出目录");
                _export(path);
            }
        }

        private void _export(string path)
        {
            foreach (var pair in _languages)
            {
                var dir = GameFramework.Utility.Path.GetCombinePath(path, pair.Key, _childDirectoryName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                _localizationCreate.CreateFile(pair.Value.ToDictionary(x=>x.Key,x=>x.Value),
                    GameFramework.Utility.Path.GetCombinePath(dir, _fileName));
            }
        }

        string _openSelectFolderPath(string title, bool savePath = false)
        {
            var path = EditorUtility.OpenFolderPanel(title, EditorPrefs.GetString(_pathKey), "");

            if (!string.IsNullOrWhiteSpace(path) && savePath)
            {
                EditorPrefs.SetString(_pathKey, path);
            }

            return path;
        }

        private void _drowLanguageList()
        {
            if (_languages.Count > 0)
            {
                if (GUILayout.Button("删除全部语言"))
                {
                    _deleteAllLanguageFile();
                    _clear();
                }
            }

            _pos = EditorGUILayout.BeginScrollView(_pos);
            {
                foreach (var language in _languages.ToDictionary(k=>k.Key,v=>v.Value))
                {
                    var languageName = language.Key;
                    if (!_showState.ContainsKey(languageName))
                    {
                        _showState.Add(languageName, false);
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        _showState[languageName] =
                            EditorGUILayout.Foldout(_showState[languageName], languageName, true);
                        if (GUILayout.Button("删除"))
                        {
                            _showState.Remove(languageName);

                            _languages.Remove(languageName);
                            _deleteLanguageFile(languageName);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (!_showState.ContainsKey(languageName) || !_showState[languageName]) continue;
                    _drowLanguageTable(languageName);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        void _deleteAllLanguageFile()
        {
            foreach (var language in _languages)
            {
                _deleteLanguageFile(language.Key);
            }
        }

        private void _deleteLanguageFile(string languageName)
        {
            if (!_deleteFile) return;

            if (Directory.Exists(_loadPath))
            {
                var path = Utility.Path.GetCombinePath(_loadPath, languageName, _childDirectoryName,
                    $"{_fileName}.{_localizationCreate.ExtensionName}");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private string _key;

        private int _i;
        //保证所有语言文件的条目都是一样的
        private List<string> _keys = new List<string>();
        private void _drowLanguageTable(string languageName)
        {
            EditorGUI.indentLevel += 1;
            {
                var tempTable = _languages[languageName].ToArray();
                _i = 0;
                foreach (var pair in tempTable)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        var key = pair.Key;
                        var value = pair.Value;

                        EditorGUIUtility.labelWidth = 50;
                        _keys[_i] = EditorGUILayout.TextField(string.Empty, _keys[_i],
                            GUILayout.Height(20f));

                        EditorGUIUtility.labelWidth = 60;
                        var newValue = EditorGUILayout.TextField(string.Empty, value, GUILayout.Height(20f));

                        if (GUI.changed)
                        {
                            //Todo 这里很迷呀,会增加键,不会报错
                            _languages[languageName][_i] = new KeyValuePair<string, string>(_keys[_i],newValue);
                        }

                        if (GUILayout.Button("删除"))
                        {
                            _removeItem(pair);
                            _autoExportUpdate();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    _i++;
                }
            }
            EditorGUI.indentLevel -= 1;
            EditorGUIUtility.labelWidth = 0;
        }

        private void _autoExportUpdate()
        {
            if (!_autoExport) return;

            _export(_loadPath);
        }


        private void _removeItem(KeyValuePair<string, string> item)
        {
            _keys.Remove(item.Key);
            
            foreach (var language in _languages)
            {
                language.Value.Remove(item);
            }
        }

        private Vector2 _pos;

        private void _createLanguage(string languageName)
        {
            if (string.IsNullOrWhiteSpace(languageName) || _languageNamesExists(languageName))
            {
                return;
            }

            _addLanguage(languageName);

            _showState.Add(languageName, true);
        }

        private bool _languageNamesExists(string languageName)
        {
            return _languages.SelectMany(t => _languages).Any(languages => languages.Key == languageName);
        }

        private int _missingNameCount;
        private string _fileName = "Default";

        void _addLanguage(string languageName)
        {
            if (string.IsNullOrWhiteSpace(languageName))
            {
                languageName = "Missing" + _missingNameCount;
                _missingNameCount++;
            }

            _languages.Add(languageName, new List<KeyValuePair<string, string>>());
            foreach (var key in _keys)
            {
                _languages[languageName].Add(new KeyValuePair<string, string>(key, string.Empty));
            }
        }
    }
}