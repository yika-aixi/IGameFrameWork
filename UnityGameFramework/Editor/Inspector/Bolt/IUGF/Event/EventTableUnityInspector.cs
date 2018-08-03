//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月29日-07:57
//Icarus.UnityGameFramework.Bolt

using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using Icarus.GameFramework;
using Icarus.UnityGameFramework.Editor;
using Ludiq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Type = System.Type;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [CustomEditor(typeof(EventTableScriptableObject))]
    public class EventTableUnityInspector : GameFrameworkInspector
    {
        private EventTableScriptableObject _tableAsset;
        private SerializedProperty _eventNames;
        private SerializedProperty _eventIDs;
        private SerializedProperty _eventArgCount;
        private SerializedProperty _eventArgNames;
        private SerializedProperty _eventArgTypeNames;
        private string[] _names;
        private int[] _ids;

        private void OnEnable()
        {
            _tableAsset = (EventTableScriptableObject)target;
            _eventNames = serializedObject.FindProperty("_eventNames");
            _eventIDs = serializedObject.FindProperty("_eventIds");
            _eventArgCount = serializedObject.FindProperty("_eventArgCount");
            _eventArgNames = serializedObject.FindProperty("_eventArgNames");
            _eventArgTypeNames = serializedObject.FindProperty("_eventArgTypeNames");
        }

        private bool _addMode;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            _names = _tableAsset.GetEventNames().ToArray();
            _ids = _tableAsset.GetEventIDs().ToArray();

            EditorGUILayout.LabelField($"Event Count:{_eventNames.arraySize}");

            _addMode = EditorGUILayout.Toggle("Auto Add EventID", _addMode);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = 15f;
                EditorGUILayout.LabelField("Event Name");
                EditorGUILayout.LabelField("Event ID");
                EditorGUILayout.LabelField("Event Args");
                EditorGUIUtility.labelWidth = 0;
            }
            EditorGUILayout.EndHorizontal();
            _addEvent();
            _removeAll();

            _showEventTable();

            if (GUI.changed)
            {
                AssetDatabase.SaveAssets();
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            Repaint();
        }

        private void _showEventTable()
        {
            for (var i = 0; i < _eventNames.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(_eventNames.GetArrayElementAtIndex(i), GUIContent.none);
                    EditorGUILayout.PropertyField(_eventIDs.GetArrayElementAtIndex(i), GUIContent.none);
                    EditorGUILayout.PropertyField(_eventArgCount.GetArrayElementAtIndex(i), GUIContent.none);
                    if (GUILayout.Button("Remove"))
                    {
                        _eventNames.DeleteArrayElementAtIndex(i);
                        _eventIDs.DeleteArrayElementAtIndex(i);
                        _eventArgCount.DeleteArrayElementAtIndex(i);
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();

                _showArgs(i, _eventIDs.GetArrayElementAtIndex(i).intValue);

                EditorGUILayout.Space();
            }
        }

        private bool _removeConfirm = false;
        private void _removeAll()
        {
            if (_eventNames.arraySize > 0)
            {
                if (_removeConfirm)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Confirm Remove All Event") || IsEnterClick())
                        {
                            _eventNames.arraySize = 0;
                            _eventIDs.arraySize = 0;
                            _eventArgCount.arraySize = 0;
                            _removeConfirm = false;
                        }

                        if (GUILayout.Button("Cancel Remove All Event") || IsEscClick())
                        {
                            _removeConfirm = false;
                        }
                    }
                    EditorGUILayout.EndHorizontal();       
                }
                else
                {
                    if (GUILayout.Button("Remove All Event"))
                    {
                        _removeConfirm = true;
                    }
                }
                
            }
        }

        private string _eventName;
        private int _id;
        private int _argCount;

        private void _addEvent()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _eventName = EditorGUILayout.TextField(_eventName, GUILayout.Width(110));
                _id = EditorGUILayout.IntField(_id, GUILayout.Width(110));
                _argCount = EditorGUILayout.IntField(_argCount, GUILayout.Width(50));

                if (GUILayout.Button("ADD") || IsEnterClick())
                {
                    if (string.IsNullOrWhiteSpace(_eventName))
                    {
                        return;
                    }
                    if (_names.Contains(_eventName))
                    {
                        return;
                    }


                    if (_ids.Contains(_id))
                    {
                        if (!_addMode)
                        {
                            return;
                        }
                        else
                        {
                            var maxID = _ids.OrderBy(x => x).Last();
                            _id = maxID + 1;
                        }
                    }

                    _addElement(_eventNames, _eventName, false);
                    _addElement(_eventIDs, _id);
                    _addElement(_eventArgCount, _argCount);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void _addElement(SerializedProperty arraySer, object value, bool isInt = true)
        {
            var index = arraySer.arraySize;
            arraySer.InsertArrayElementAtIndex(index);
            if (isInt)
            {
                arraySer.GetArrayElementAtIndex(index).intValue = (int)value;
            }
            else
            {
                arraySer.GetArrayElementAtIndex(index).stringValue = (string)value;
            }
        }

        private void _showArray(SerializedProperty arraySer, bool showRemove = false, bool isArgs = false,
            params GUILayoutOption[] guiLayout)
        {
            EditorGUILayout.BeginVertical();
            {
                for (int i = 0; i < arraySer.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(arraySer.GetArrayElementAtIndex(i), new GUIContent(""),
                            guiLayout);
                        if (showRemove)
                        {
                            if (GUILayout.Button("Remove", guiLayout))
                            {
                                _eventNames.DeleteArrayElementAtIndex(i);
                                _eventIDs.DeleteArrayElementAtIndex(i);
                                _eventArgCount.DeleteArrayElementAtIndex(i);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    //                    if (isArgs)
                    //                    {
                    //                        _showArgs(i, arraySer.GetArrayElementAtIndex(i).intValue,
                    //                            guiLayout);
                    //                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        readonly List<bool> _foldoutState = new List<bool>();
        private void _showArgs(int index,int eventID,params GUILayoutOption[] guiLayout)
        {
            if (_foldoutState.Count <= index)
            {
                _foldoutState.Add(false);
            }

            _foldoutState[index] = EditorGUILayout.Foldout(_foldoutState[index],
                $"EventArgCount:{_eventArgCount.GetArrayElementAtIndex(index).intValue}", true);

            if (!_foldoutState[index])
            {
                return;
            }

            var args = _tableAsset.ArgGroup[eventID];

            EditorGUI.indentLevel++;
            {
                for (var i = args[0]; i < args[1]; i++)
                {
                    var argIndex = i;

                    var argName = _eventArgNames.GetArrayElementAtIndex(argIndex);
                    if (argName.stringValue == EventTableScriptableObject.NullStr)
                    {
                        continue;
                    }
                    var arg = _eventArgTypeNames.GetArrayElementAtIndex(argIndex);
                    var position = EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUIUtility.labelWidth = 80f;
                        EditorGUILayout.PropertyField(argName, new GUIContent("Arg Name:"));
                        GUIStyle fontStyle = new GUIStyle();
                        fontStyle.normal.background = null;    //设置背景填充  
                        fontStyle.normal.textColor = new Color(1, 0, 0);   //设置字体颜色  
                        fontStyle.fontSize = EditorStyles.label.fontSize;       //字体大小  
                        EditorGUILayout.LabelField($"Type:{arg.stringValue.Split(',').First()}", fontStyle);
                        if (GUILayout.Button("Select Type"))
                        {
                            var rect = GUILayoutUtility.GetLastRect();
                            rect = new Rect(position.width, position.yMax / 2, rect.width,rect.height);
                            FuzzyWindow.Show(rect, _getOptionTree(_getCurrentType(arg.stringValue)), (option) =>
                            {
                                arg.stringValue = ((Type)option.value).AssemblyQualifiedName;

                                serializedObject.ApplyModifiedProperties();


                                FuzzyWindow.instance.Close();
                                InternalEditorUtility.RepaintAllViews();
                            });
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel--;
        }

        private Type _getCurrentType(string argStringValue)
        {
            if (string.IsNullOrEmpty(argStringValue))
            {
                return typeof(object);
            }

            return Type.GetType(argStringValue);
        }

        private IFuzzyOptionTree _getOptionTree(object currentSelectType)
        {
            var optionTree = new TypeOptionTree(Codebase.GetTypeSetFromAttribute(Metadata.Root()));
            optionTree.selected.Clear();
            optionTree.selected.Add(currentSelectType);
            return optionTree;
        }
    }
}