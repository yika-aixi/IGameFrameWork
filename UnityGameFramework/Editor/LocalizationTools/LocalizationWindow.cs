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
using System.Xml;
using Icarus.GameFramework;
using Icarus.GameFramework.Localization;
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
            var window = GetWindow<LocalizationWindow>(true, WindowTitle, true);
            window.minSize = new Vector2(600, 630f);
        }

        #region 语言变量
        private const string WindowTitle = "多语言管理工具 - Localization";
        private const string NullLocalizationCreateTypesHelp = "没有找到实现,请增加{0}的实现";
        private const string LocalizationCreateTypeLabel = "LocalizationCreate Type";
        private const string FileNameLabel = "文件名:";
        private const string RemoveLanguageAndDeleteConfigFileLabel = "删除语言时同时删除配置文件";
        private const string AutoExportLabel = "Auto Export";
        private const string LanguageLabel = "Language";
        private const string AddLanguageLabel = "添加所选语言";
        private const string NoSelectLanguageHelp = "没有指定语言,无法进行添加";
        private const string ExportDirLabel = "子目录名(导出)";
        private const string ExportFontsLabel = "字体子目录名(导出)";
        private const string AddItemLabel = "添加条目：";
        private const string ReadLabel = "读取";
        private const string ReadDir = "读取目录";
        private const string RefreshFontsXmlLabel = "刷新字体导出数据";
        private const string SelectXmlFileLabel = "Select Xml File";
        private const string ExportLabel = "导出";
        private const string SelectExportLabel = "导出目录";
        private const string ErrorTitle = "错误";
        private const string FontsExportXmlNoExistsMessage = "字体导出记录配置文件不存在,读取失败.";
        private const string ConfirmLabel = "ok";
        private const string ExportFontErrorMessage = "Unity 默认字体不支持导出。";
        private const string WarningTitle = "警告";
        private const string FileExistsMessage = "已存在相同名的字体文件";
        private const string OverwriteLabel = "覆盖";
        private const string SkipLabel = "跳过";
        private const string ReservedLabel = "都保留";
        private const string RemoveAllLanguageLabel = "删除全部语言";
        private const string RemoveLabel = "删除";
        private const string FirstLabel = "First";
        private const string UpMark = "<-";
        private const string NextMark = "->";
        private const string LastLabel = "Last";
        private const string FontsCountLabel = "Fonts Count";
        private const string Missing = "Missing";
        private const string DeleteLanguageMessage = "这是危险操作,如果开启了{0},将会删除语言配置";
        private const string CancelLabel = "cancel";
        #endregion

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
                EditorGUILayout.HelpBox(
                    string.Format(NullLocalizationCreateTypesHelp,
                        typeof(LocalizationCreateAndParseBase).AssemblyQualifiedName),
                    MessageType.Warning);

                return;
            }

            _createTypeSelectIndex = EditorGUILayout.Popup(new GUIContent(LocalizationCreateTypeLabel),
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

            //绘制语言表
            _drowLanguageList();

        }

        private void _test()
        {
            if (GUILayout.Button("Test"))
            {
                for (int i = 0; i < 5000; i++)
                {
                    _addItem(i.ToString());
                }
            }
        }

        private void _openFolder()
        {
            if (GUILayout.Button("Open Load Folder", GUILayout.Height(25f)))
            {
                OpenFolder.Open(_loadPath);
            }
        }

        private const string _fileNameKey = "Localization_fileNameKeyy";

        private void _setFileName()
        {
            _fileName = EditorGUILayout.TextField(FileNameLabel, _fileName);
            EditorPrefs.SetString(_fileNameKey, _fileName);
        }

        private void _drowDeleteFile()
        {
            _deleteFile = EditorGUILayout.Toggle(RemoveLanguageAndDeleteConfigFileLabel, _deleteFile);
            EditorPrefs.SetBool(_deleteFileKey, _deleteFile);
        }

        private void _drowAutoExport()
        {
            _autoExport = EditorGUILayout.Toggle(AutoExportLabel, _autoExport);

            if (GUI.changed)
            {
                EditorPrefs.SetBool(_autoExportKey, _autoExport);
            }
        }

        /// <summary>
        /// 同步条目
        /// </summary>
        private void _syncItem()
        {
            foreach (var language in _languages)
            {
                foreach (var pair in language.Value)
                {
                    if (!_keys.Exists(x => x == pair.Key))
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
                        language.Value.Add(new KeyValuePair<string, string>(key, string.Empty));
                    }

                    if (language.Value[i].Key != key)
                    {
                        language.Value.Insert(i, new KeyValuePair<string, string>(key, string.Empty));
                        continue;
                    }

                    language.Value[i] = new KeyValuePair<string, string>(key, language.Value[i].Value);
                }
            }
        }

        private void _drowAddLanguage()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _languageSelect = (Language) EditorGUILayout.EnumPopup(LanguageLabel, _languageSelect);
                if (GUILayout.Button(AddLanguageLabel))
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
                EditorGUILayout.HelpBox(NoSelectLanguageHelp, MessageType.Warning);
            }
        }

        private const string _childDirectoryNameKey = "Localization_childDirectoryNameKey";

        private void _setChildDirectory()
        {
            _childDirectoryName = EditorGUILayout.TextField(ExportDirLabel, _childDirectoryName);
            _childFontsName = EditorGUILayout.TextField(ExportFontsLabel, _childFontsName);
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
                    _readFontsExPortRecording(Utility.Path.GetCombinePath(_loadPath, FontsExPortRecordingXmlName));
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
                    _key = EditorGUILayout.TextField(AddItemLabel, _key, GUILayout.Height(20f));
                    if (GUILayout.Button("+") || _isClickEnter())
                    {
                        if (string.IsNullOrEmpty(_key))
                        {
                            return;
                        }

                        var result = _addItem(_key);

                        if (!result)
                        {
                            return;    
                        }
                        
                        _syncItem();

                        _autoExportUpdate();

                        _posAllLanguagPage();

                        _changeAllShowState(true);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        
        /// <summary>
        /// 改变所有显示状态
        /// </summary>
        /// <param name="state"></param>
        private void _changeAllShowState(bool state)
        {
            foreach (var language in _languages)
            {
                _showState[language.Key] = true;
            }
        }

        /// <summary>
        /// 定位所有语言到最后一页
        /// </summary>
        private void _posAllLanguagPage()
        {
            foreach (var languagesKey in _languages.Keys)
            {
                _posLastPage(languagesKey);
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
            if (GUILayout.Button(ReadLabel, GUILayout.Height(25)))
            {
                var path = _openSelectFolderPath(ReadDir, true);
                _loadPath = path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }

                _readDirectory(path);

                _readFontsExPortRecording(Path.Combine(path, FontsExPortRecordingXmlName));
            }

            if (GUILayout.Button(RefreshFontsXmlLabel, GUILayout.Height(25)))
            {
                var path = EditorUtility.OpenFilePanel(SelectXmlFileLabel, EditorPrefs.GetString(_pathKey), "xml");
                _loadPath = path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }

                _readFontsExPortRecording(path);
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

                Dictionary<string, string> dict = new Dictionary<string, string>();

                _localizationCreate.Parse(fileContent, dict, null);

                foreach (var pair in dict)
                {
                    _languages[languageName].Add(new KeyValuePair<string, string>(pair.Key, pair.Value));
                }

                _syncItem();
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

        private bool _addItem(string key)
        {
            if (!_keys.Exists(x => x == key))
            {
                _keys.Add(key);
                return true;
            }

            return false;
        }

        private void _clear()
        {
            _languages.Clear();
            _keys.Clear();
            _showState.Clear();
            _fonts.Clear();
            _languageCurrentPage.Clear();
        }

        private string _pathKey = "saveAndLoadPath-Window";
        private int _createTypeSelectIndex;
        private int _helperTypeSelectIndex;
        private string _childDirectoryName = "Dictionaries";
        private string _childFontsName = "Fonts";

        private void _export()
        {
            if (GUILayout.Button(ExportLabel, GUILayout.Height(25)))
            {
                var path = _openSelectFolderPath(SelectExportLabel);
                _export(path);
                _createFontsExPortRecording(path);
            }
        }

        private const string FontsExPortRecordingXmlName = "FontsExPortRecording.xml";
        private const string FontsExPortRecordingXmlRootName = "LocalizationFontAssetsPaths";
        private const string FontsExPortRecordingXmlFontPathsName = "FontPaths";
        private const string FontsExPortRecordingXmlFontPathsOfLanguageAttributeName = "LanguageName";
        private const string FontsExPortRecordingXmlFontPathName = "FontPath";
        
        /// <summary>
        /// 创建语言字体记录文件
        /// </summary>
        /// <param name="path"></param>
        private void _createFontsExPortRecording(string path)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement xmlRoot = xmlDocument.CreateElement(FontsExPortRecordingXmlRootName);
            xmlDocument.AppendChild(xmlRoot);

            foreach (var language in _fonts)
            {
                XmlElement xmlLanguage = xmlDocument.CreateElement(FontsExPortRecordingXmlFontPathsName);
                var languageNameAttr =
                    xmlDocument.CreateAttribute(FontsExPortRecordingXmlFontPathsOfLanguageAttributeName);
                languageNameAttr.Value = language.Key;
                xmlLanguage.Attributes.SetNamedItem(languageNameAttr);
                xmlRoot.AppendChild(xmlLanguage);

                foreach (var font in language.Value)
                {
                    XmlElement xmlFontPath = xmlDocument.CreateElement(FontsExPortRecordingXmlFontPathName);
                    xmlFontPath.InnerText = AssetDatabase.GetAssetPath(font);
                    xmlLanguage.AppendChild(xmlFontPath);
                }
            }

            xmlDocument.Save(Utility.Path.GetCombinePath(path, FontsExPortRecordingXmlName));
        }

        private void _readFontsExPortRecording(string path)
        {
            if (!File.Exists(path))
            {
                EditorUtility.DisplayDialog(ErrorTitle, FontsExportXmlNoExistsMessage, ConfirmLabel);
                return;
            }

            _fonts.Clear();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            XmlNode xmlRoot = xmlDocument.SelectSingleNode(FontsExPortRecordingXmlRootName);

            foreach (XmlElement fontPathsNode in xmlRoot.ChildNodes)
            {
                XmlAttribute languageNameAttribute =
                    fontPathsNode.GetAttributeNode(FontsExPortRecordingXmlFontPathsOfLanguageAttributeName);
                var languageName = languageNameAttribute.Value;
                if (!_fonts.ContainsKey(languageName))
                {
                    _fonts.Add(languageName, new List<Font>());
                }

                foreach (XmlElement fontPathNode in fontPathsNode.ChildNodes)
                {
                    _fonts[languageName].Add(AssetDatabase.LoadAssetAtPath<Font>(fontPathNode.InnerText));
                }
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

                _localizationCreate.CreateFile(pair.Value.ToDictionary(x => x.Key, x => x.Value),
                    GameFramework.Utility.Path.GetCombinePath(dir, _fileName));

                _copyFonts(path, pair.Key);
            }
        }


        private void _copyFonts(string outPath, string languageName)
        {
            if (_fonts.ContainsKey(languageName))
            {
                var fonts = _fonts[languageName];

                foreach (var font in fonts)
                {
                    var path = AssetDatabase.GetAssetPath(font);

                    if (path.Contains("Library"))
                    {
                        Debug.LogError(ExportFontErrorMessage);
                        continue;
                    }

                    var assetsPath = GameFramework.Utility.Path.GetCombinePath(
                        Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")), path);
                    Debug.LogError(assetsPath);

                    var targetDirPath =
                        GameFramework.Utility.Path.GetCombinePath(outPath, languageName, _childFontsName);

                    if (!Directory.Exists(targetDirPath))
                    {
                        Directory.CreateDirectory(targetDirPath);
                    }

                    var targetFilePath =
                        GameFramework.Utility.Path.GetCombinePath(targetDirPath, Path.GetFileName(assetsPath));

                    if (File.Exists(targetFilePath))
                    {
                        var code = EditorUtility.DisplayDialogComplex(WarningTitle, FileExistsMessage, OverwriteLabel,
                            SkipLabel, ReservedLabel);
                        switch (code)
                        {
                            case 0:
                                File.Copy(assetsPath, targetFilePath, true);
                                continue;
                            case 1:
                                continue;
                            case 2:
                                int counter = 1;
                                string newTargetFilePath = targetFilePath;

                                while (System.IO.File.Exists(newTargetFilePath))
                                {
                                    newTargetFilePath =
                                        targetFilePath.Insert(targetFilePath.IndexOf('.'), $"_{counter}");
                                    counter++;
                                }

                                targetFilePath = newTargetFilePath;
                                break;
                        }
                    }

                    File.Copy(assetsPath, targetFilePath);
                }
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
                if (GUILayout.Button(RemoveAllLanguageLabel))
                {
                    if (EditorUtility.DisplayDialog(WarningTitle, string.Format(DeleteLanguageMessage,RemoveLanguageAndDeleteConfigFileLabel), ConfirmLabel, CancelLabel))
                    {
                        _deleteAllLanguageFile();
                        _clear();
                    }
                }
            }

            _pos = EditorGUILayout.BeginScrollView(_pos);
            {
                foreach (var language in _languages.ToDictionary(k => k.Key, v => v.Value))
                {
                    var languageName = language.Key;
                    if (!_showState.ContainsKey(languageName))
                    {
                        _showState.Add(languageName, false);
                        //初始化Page
                        _languageCurrentPage.Add(languageName, 0);
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        _showState[languageName] =
                            EditorGUILayout.Foldout(_showState[languageName], languageName, true);
                        if (GUILayout.Button(RemoveLabel))
                        {
                            if (EditorUtility.DisplayDialog(WarningTitle, string.Format(DeleteLanguageMessage,RemoveLanguageAndDeleteConfigFileLabel), ConfirmLabel,
                                CancelLabel))
                            {
                                _showState.Remove(languageName);

                                _languages.Remove(languageName);
                                _deleteLanguageFile(languageName);
                            }
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

        //每页最大数量
        private int _pageMaxItemCount = 20;

        //当前页
        private Dictionary<string, int> _languageCurrentPage = new Dictionary<string, int>();

        private void _drowLanguageTable(string languageName)
        {
            EditorGUI.indentLevel += 1;
            {
                var tempTable = _languages[languageName].ToArray();
                _i = (_languageCurrentPage[languageName] * _pageMaxItemCount);
                _drowLanguageFontsList(languageName);
                foreach (var pair in tempTable)
                {
                    if (_i >= ((_languageCurrentPage[languageName] + 1) * _pageMaxItemCount) || _i >= _keys.Count)
                    {
                        break;
                    }

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
                            _languages[languageName][_i] = new KeyValuePair<string, string>(_keys[_i], newValue);
                        }

                        if (GUILayout.Button(RemoveLabel))
                        {
                            _removeItem(pair);
                            _autoExportUpdate();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    _i++;
                }

                _drowChangePage(languageName);
            }
            EditorGUI.indentLevel -= 1;
            EditorGUIUtility.labelWidth = 0;
        }

        private void _drowChangePage(string languageName)
        {
            EditorGUILayout.BeginHorizontal();
            {
                //当前页面大于0
                GUI.enabled = _languageCurrentPage[languageName] > 0;

                if (GUILayout.Button(FirstLabel))
                {
                    _languageCurrentPage[languageName] = 0;
                }

                if (GUILayout.Button(UpMark))
                {
                    _languageCurrentPage[languageName]--;
                }

                //还有下一页
                GUI.enabled = (_languageCurrentPage[languageName] + 1) * _pageMaxItemCount < _keys.Count;

                if (GUILayout.Button(NextMark))
                {
                    _languageCurrentPage[languageName]++;
                }

                if (GUILayout.Button(LastLabel))
                {
                    _posLastPage(languageName);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        void _posLastPage(string languageName)
        {
            var page = _keys.Count / _pageMaxItemCount;

            if (page * _pageMaxItemCount >= _keys.Count)
            {
                page--;
            }

            _languageCurrentPage[languageName] = page;
        }

        private Dictionary<string, List<Font>> _fonts = new Dictionary<string, List<Font>>();

        /// <summary>
        /// 绘制语言字体列表
        /// </summary>
        /// <param name="languageName"></param>
        private void _drowLanguageFontsList(string languageName)
        {
            if (!_fonts.ContainsKey(languageName))
            {
                _fonts.Add(languageName, new List<Font>());
            }

            var fonts = _fonts[languageName];

            int count = 0;

            count = EditorGUILayout.IntField(FontsCountLabel, fonts.Count);

            //增加
            if (fonts.Count < count)
            {
                for (int i = fonts.Count; i < count; i++)
                {
                    fonts.Add(null);
                }
            }
            //从尾巴移除
            else if (fonts.Count > count)
            {
                for (int i = fonts.Count - count; i > 0; i--)
                {
                    fonts.RemoveAt(fonts.Count - 1);
                }
            }

            EditorGUI.indentLevel++;
            {
                for (var index = 0; index < fonts.Count; index++)
                {
                    var font = fonts[index];

                    fonts[index] = (Font) EditorGUILayout.ObjectField(font, typeof(Font));
                }
            }
            EditorGUI.indentLevel--;
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
                languageName = Missing + _missingNameCount;
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