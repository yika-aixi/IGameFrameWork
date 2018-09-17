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
using System.Text;
using Icarus.UnityGameFramework.Runtime.I18N;
using UnityEditor;
using UnityEngine;

namespace Icarus.UnityGameFramework.Editor
{
    public class I18NWindow : EditorWindow
    {
        [MenuItem("Icarus/Game Framework/Localization/I18N Tools", false, 31)]
        public static void Open()
        {
            var window = GetWindow<I18NWindow>("多语言管理工具");
            window.minSize = new Vector2(1400f, 600f);
        }

        private readonly Dictionary<string, Dictionary<string, string>> _languages =
            new Dictionary<string, Dictionary<string, string>>();

        private readonly Dictionary<string, bool> _showState = new Dictionary<string, bool>();

        public void OnGUI()
        {
            //文件操作,读取或导出
            _fileLoadAndExport();

            //创建语言GUI
            _createLanguageGUI();

            //添加条目
            _addItem();

            //绘制语言表
            _drowLanguageList();
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
                    _key = EditorGUILayout.TextField("添加条目：", _key);
                    if (GUILayout.Button("+") || _isClickEnter())
                    {
                        if (string.IsNullOrEmpty(_key))
                        {
                            return;
                        }

                        _addItem(_key);
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

        string _suffixName = ConstTable.SuffixName;

        private void _load()
        {
            if (GUILayout.Button("读取", GUILayout.Height(25)))
            {
                var path = _getPath("读取目录");
                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }

                var filePaths =
                    Directory.GetFiles(path, $"*.{_suffixName}", SearchOption.AllDirectories);
                if (filePaths.Length <= 0)
                {
                    return;
                }

                _clear();
                foreach (var filePath in filePaths)
                {
                    var fileContent = File.ReadAllLines(filePath);
                    var languageName = fileContent[0];

                    _addLanguage(languageName);

                    var keys = fileContent.Skip(1).Select(x => x.Split('\t')[0]).ToList();
                    var values = fileContent.Skip(1).Select(x => x.Split('\t')[1]).ToList();
                    var nextIndex = 0;

                    for (var i = 0; i < keys.Count; i++)
                    {
                        var key = keys[i];
                        bool isHit = false;
                        _addItem(languageName, key, values[i]);
                    }
                }
            }
        }

        private void _addItem(string key)
        {
            foreach (var language in _languages)
            {
                if (!_languages[language.Key].ContainsKey(key))
                {
                    _languages[language.Key].Add(key, string.Empty);
                }
            }
        }

        private void _addItem(string languageName, string key, string value)
        {
            if (!_languages[languageName].ContainsKey(key))
            {
                _languages[languageName].Add(key, value);
            }

            _addItem(key);
        }

        private void _clear()
        {
            _languages.Clear();
        }

        private string _pathKey = "saveAndLoadPath-Window";

        private void _export()
        {
            if (GUILayout.Button("导出", GUILayout.Height(25)))
            {
                var path = _getPath("导出目录");
                foreach (var pair in _languages)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(pair.Key);
                    foreach (var table in pair.Value)
                    {
                        var line = $"{table.Key}\t{table.Value}\n";
                        sb.Append(line);
                    }

                    sb.Remove(sb.Length - 1, 1);
                    File.WriteAllText(Path.Combine(path, $"{pair.Key}.{_suffixName}"), sb.ToString());
                }
            }
        }

        string _getPath(string title)
        {
            var path = EditorUtility.OpenFolderPanel(title, EditorPrefs.GetString(_pathKey), "");
            if (!string.IsNullOrWhiteSpace(path))
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
                    _clear();
                }
            }
            _pos = EditorGUILayout.BeginScrollView(_pos);
            {
                foreach (var language in _languages)
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
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (!_showState.ContainsKey(languageName) || !_showState[languageName]) continue;
                    _drowLanguageTable(languageName);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private string _key;

        private void _drowLanguageTable(string languageName)
        {
            EditorGUI.indentLevel += 1;
            {
                var language = _languages[languageName];
                foreach (var pair in language)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        var key = pair.Key;
                        var value = pair.Value;

                        EditorGUIUtility.labelWidth = 50;
                        EditorGUILayout.TextField(string.Empty, key);
                        EditorGUIUtility.labelWidth = 60;
                        EditorGUILayout.TextField(string.Empty, value);
                        if (GUILayout.Button("删除"))
                        {
                            _removeItem(key);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel -= 1;
            EditorGUIUtility.labelWidth = 0;
        }

        private void _removeItem(string key)
        {
            foreach (var language in _languages)
            {
                if (language.Value.ContainsKey(key))
                {
                    language.Value.Remove(key);
                }
            }
        }

        private string _languageName;
        private Vector2 _pos;

        private void _createLanguageGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _languageName = EditorGUILayout.TextField("语言名称:", _languageName);
                if (GUILayout.Button("创建") || _isClickEnter())
                {
                    _createLanguage(_languageName);

                    _languageName = string.Empty;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

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

        void _addLanguage(string languageName)
        {
            if (string.IsNullOrWhiteSpace(languageName))
            {
                languageName = "Missing" + _missingNameCount;
                _missingNameCount++;
            }

            _languages.Add(languageName, new Dictionary<string, string>());
        }
    }
}