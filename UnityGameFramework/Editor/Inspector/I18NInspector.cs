using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Icarus.UnityGameFramework.Runtime;
using Icarus.UnityGameFramework.Runtime.I18N;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Icarus.UnityGameFramework.Editor
{
    [CustomEditor(typeof(I18NComponent))]
    public class I18NInspector : GameFrameworkInspector
    {
        private I18NComponent _i18N;
        private SerializedProperty _defaultLanguageSer;
        private SerializedProperty _currenLanguageSer;
        private SerializedProperty _languageNamesSer;
        private Dictionary<string, bool> _showState = new Dictionary<string, bool>();
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(_defaultLanguageSer, new GUIContent("默认语言:"));

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.PropertyField(_currenLanguageSer, new GUIContent("当前语言:"));
            }

            _findLocal();

            _suffx();
            
            _createLanguage();

            _addItem();

            _fileLoadAndExport();

            _drowLanguageList();

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        private void _findLocal()
        {
            EditorGUILayout.PropertyField(_isFindLocalSer, new GUIContent("本地查找:"));
            if (_isFindLocalSer.boolValue)
            {
                EditorGUILayout.PropertyField(_directoryNameSer, new GUIContent("查找目录名(运行时):"));
                if (string.IsNullOrWhiteSpace(_directoryNameSer.stringValue))
                {
                    _directoryNameSer.stringValue = ConstTable.DefaultDirectoryName;
                }
            }
        }

        private void _suffx()
        {
            EditorGUILayout.PropertyField(_suffixNameSer, new GUIContent("扩展名:"));

            if (string.IsNullOrWhiteSpace(_suffixNameSer.stringValue))
            {
                _suffixNameSer.stringValue = ConstTable.SuffixName;
            }
        }

        private void _addItem()
        {
            if (_languageNamesSer.arraySize > 0)
            {
                //添加数据
                EditorGUILayout.BeginHorizontal();
                {
                    _key = EditorGUILayout.TextField("添加条目：", _key);
                    if (GUILayout.Button("+"))
                    {
                        for (int i = 0; i < _keysSer.arraySize; i++)
                        {
                            var item = _keysSer.GetArrayElementAtIndex(i);
                            if (item.stringValue == _key)
                            {
                                return;
                            }
                        }


                        var result = _add(_keysSer, _key, $"添加Key:{_key},失败！", false);

                        if (!result)
                        {
                            return;
                        }

                        _insertValue();

                        _key = String.Empty;
                        _isAllExpand(true);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void _isAllExpand(bool value)
        {
            for (int i = 0; i < _languageNamesSer.arraySize; i++)
            {
                var lanName = _languageNamesSer.GetArrayElementAtIndex(i);
                _showState[lanName.stringValue] = value;
            }
        }

        private void _insertValue(bool update = true)
        {
            for (int i = 0; i < _languageNamesSer.arraySize; i++)
            {
                var index = _getValueIndex(_keysSer.arraySize - 1, i);
                _valuesSer.InsertArrayElementAtIndex(index);
                _valuesSer.GetArrayElementAtIndex(index).stringValue = String.Empty;
            }

            if (!update) return;

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

        }

        private void _fileLoadAndExport()
        {

            _load();

            if (_languageNamesSer.arraySize > 0)
            {
                _export();
            }

        }

        private void _load()
        {
            if (GUILayout.Button("读取", GUILayout.Height(25)))
            {
                var path = _getPath("读取目录");
                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }
                var filePaths = Directory.GetFiles(path, $"*.{_suffixNameSer.stringValue}", SearchOption.AllDirectories);
                _clearSer();
                foreach (var filePath in filePaths)
                {
                    var fileContent = File.ReadAllLines(filePath);
                    var languageName = fileContent[0];

                    var result = _add(_languageNamesSer, languageName, "读取失败!", false);

                    if (!result)
                    {
                        return;
                    }

                    var languageIndex = _languageNamesSer.arraySize - 1;

                    var keys = fileContent.Skip(1).Select(x => x.Split('\t')[0]).ToList();
                    var values = fileContent.Skip(1).Select(x => x.Split('\t')[1]).ToList();
                    var nextIndex = 0;

                    foreach (var key in keys)
                    {
                        bool isHit = false;
                        for (int i = 0; i < _keysSer.arraySize; i++)
                        {
                            var item = _keysSer.GetArrayElementAtIndex(i);
                            if (item.stringValue == key)
                            {
                                isHit = true;
                                break;
                            }
                        }

                        if (!isHit)
                        {
                            result = _add(_keysSer, key, "读取失败!", false);
                            if (!result)
                            {
                                return;
                            }
                            var keyIndex = _keysSer.arraySize - 1;
                            _insertValue(false);
                            var valueIndex = _getValueIndex(keyIndex, languageIndex);

                            _valuesSer.GetArrayElementAtIndex(valueIndex).stringValue = values[nextIndex];
                        }
                        else
                        {
                            result = _add(_valuesSer, values[nextIndex], "读取失败!", false);
                            if (!result)
                            {
                                return;
                            }
                        }

                        nextIndex++;
                    }
                }
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }

        private void _clearSer()
        {
            _languageNamesSer.ClearArray();
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private string _pathKey = "saveAndLoadPath";

        private void _export()
        {
            if (GUILayout.Button("导出", GUILayout.Height(25)))
            {
                var path = _getPath("导出目录");
                foreach (var pair in _i18N.ConvenienceLanguageTable)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(pair.Key);
                    foreach (var table in pair.Value)
                    {
                        var line = $"{table.Key}\t{table.Value}\n";
                        sb.Append(line);
                    }

                    sb.Remove(sb.Length - 1, 1);
                    File.WriteAllText(Path.Combine(path, $"{pair.Key}.{_suffixNameSer.stringValue}"), sb.ToString());
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
            if (_languageNamesSer.arraySize > 0)
            {
                if (GUILayout.Button("删除全部语言"))
                {
                    _clearSer();
                }
            }
            for (var i = 0; i < _languageNamesSer.arraySize; i++)
            {
                var languageName = _languageNamesSer.GetArrayElementAtIndex(i).stringValue;
                if (!_showState.ContainsKey(languageName))
                {
                    _showState.Add(languageName, false);
                }

                EditorGUILayout.BeginHorizontal();
                {
                    _showState[languageName] = EditorGUILayout.Foldout(_showState[languageName], languageName, true);
                    if (GUILayout.Button("删除"))
                    {
                        _showState.Remove(languageName);

                        _languageNamesSer.DeleteArrayElementAtIndex(i);

                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (!_showState.ContainsKey(languageName) || !_showState[languageName]) continue;
                _drowLanguageTable(i);
            }
        }

        private string _key;
        private void _drowLanguageTable(int languageIndex)
        {
            EditorGUI.indentLevel += 1;
            {
                for (int i = 0; i < _keysSer.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        var key = _keysSer.GetArrayElementAtIndex(i);
                        var index = _getValueIndex(i, languageIndex);
                        var value = _valuesSer.GetArrayElementAtIndex(index);
                        var keyRect =
                            GUILayoutUtility.GetRect(new GUIContent(key.stringValue), EditorStyles.objectField);
                        var valueRect =
                            GUILayoutUtility.GetRect(new GUIContent(value.stringValue), EditorStyles.objectField);
                        EditorGUIUtility.labelWidth = 50;
                        EditorGUI.PropertyField(keyRect, key, new GUIContent("Key:"), false);
                        EditorGUIUtility.labelWidth = 60;
                        EditorGUI.PropertyField(valueRect, value, new GUIContent("Value:"), false);
                        if (GUILayout.Button("删除"))
                        {
                            _remove(i);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                }
            }
            EditorGUI.indentLevel -= 1;
            EditorGUIUtility.labelWidth = 0;
        }

        private void _remove(int keyIndex)
        {
            for (int i = 0; i < _languageNamesSer.arraySize; i++)
            {
                _valuesSer.DeleteArrayElementAtIndex(_getValueIndex(keyIndex, i) - i);
            }
            _keysSer.DeleteArrayElementAtIndex(keyIndex);

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private int _getValueIndex(int keyIndex, int languageIndex)
        {
            var index = keyIndex + languageIndex * _keysSer.arraySize + 1 - 1;
            return index;
        }

        private string _languageName;
        private SerializedProperty _keysSer;
        private SerializedProperty _valuesSer;
        private SerializedProperty _directoryNameSer;
        private SerializedProperty _suffixNameSer;
        private SerializedProperty _isFindLocalSer;

        private void _createLanguage()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _languageName = EditorGUILayout.TextField("语言名称:", _languageName);
                if (GUILayout.Button("创建") || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    if (string.IsNullOrWhiteSpace(_languageName) || _languageNamesExists(_languageName))
                    {
                        return;
                    }

                    if (!_add(_languageNamesSer, _languageName, $"创建{_languageName}失败！"))
                    {
                        return;
                    }
                    _showState.Add(_languageName, true);

                    _languageName = string.Empty;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool _languageNamesExists(string languageName)
        {
            for (int i = 0; i < _languageNamesSer.arraySize; i++)
            {
                var item = _languageNamesSer.GetArrayElementAtIndex(i);
                if (item.stringValue == languageName)
                {
                    return true;
                }
            }

            return false;
        }

        bool _add(SerializedProperty serialized, string value, string errorMeeage, bool update = true)
        {
            if (serialized == null)
            {
                Debug.LogError(errorMeeage);
                return false;
            }
            serialized.arraySize += 1;
            var pro = serialized.GetArrayElementAtIndex(serialized.arraySize - 1);
            pro.stringValue = value;
            if (update)
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }

            return true;
        }


        private void OnEnable()
        {
            _i18N = (I18NComponent)target;
            //            _languageNames.AddRange(_i18N.GetLanguges());
            _defaultLanguageSer = serializedObject.FindProperty("_defaultLanguage");
            _languageNamesSer = serializedObject.FindProperty("_languageNames");
            _keysSer = serializedObject.FindProperty("_keys");
            _valuesSer = serializedObject.FindProperty("_values");
            _isFindLocalSer = serializedObject.FindProperty("_isFindLocal");
            _directoryNameSer = serializedObject.FindProperty("_directoryName");
            _suffixNameSer = serializedObject.FindProperty("_suffixName");
        }
    }
}