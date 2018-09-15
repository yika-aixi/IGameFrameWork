//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月29日-07:57
//Icarus.UnityGameFramework.Bolt

using Icarus.UnityGameFramework.Editor;
using Ludiq;
using System;
using System.Collections.Generic;
using System.Linq;
using Icarus.GameFramework;
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
        private SerializedProperty _events;
        private string[] _names;
        private int[] _ids;

        private void OnEnable()
        {
            _tableAsset = (EventTableScriptableObject)target;
            _events = serializedObject.FindProperty("_events");
        }

        private bool _addMode;
        private bool _autoSave;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var path = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
            if (oldEventTableUpdateTool.IsCanUpdate(path))
            {
                if (GUILayout.Button("Update EventTable"))
                {
                    if (!oldEventTableUpdateTool.Update(path,serializedObject))
                    {
                        throw new GameFrameworkException("Update EventTable Failure");
                    }
                }
            }

            serializedObject.Update();
            _names = _tableAsset.GetEventNames()?.ToArray();
            _ids = _tableAsset.GetEventIDs()?.ToArray();

            EditorGUILayout.LabelField($"Event Count:{_events.arraySize}");
            _autoSave = EditorGUILayout.Toggle("Auto Save", _autoSave);
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

            if (GUI.changed && _autoSave)
            {
                AssetDatabase.SaveAssets();
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            Repaint();
        }

        private void _showEventTable()
        {
            for (var i = 0; i < _events.arraySize; i++)
            {
                var @event = _events.GetArrayElementAtIndex(i);
                var eventName = @event.FindPropertyRelative("_eventName");
                var eventID = @event.FindPropertyRelative("_eventId");
                var eventArgs = @event.FindPropertyRelative("_args");
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(eventName, GUIContent.none);
                    EditorGUILayout.PropertyField(eventID, GUIContent.none);
                    var argCount = EditorGUILayout.IntField(eventArgs.arraySize);
                    if (GUI.changed)
                    {
                        eventArgs.arraySize = argCount;
                    }
                    if (GUILayout.Button("Remove"))
                    {
                        _events.DeleteArrayElementAtIndex(i);
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();

                _showArgs(i, eventArgs);

                if (i < _events.arraySize - 1)
                {
                    DrawUILine(Color.black);
                }
            }
        }

        private void _showArgs(int index, SerializedProperty eventArgs)
        {
            if (!_foldout(index, eventArgs.arraySize))
            {
                return;
            }

            for (int i = 0; i < eventArgs.arraySize; i++)
            {
                var arg = eventArgs.GetArrayElementAtIndex(i);
                var argName = arg.FindPropertyRelative("_argName");
                var argTypeStr = arg.FindPropertyRelative("_argTypeStr");
                var argDesc = arg.FindPropertyRelative("_argDesc");
                var argNotNull = arg.FindPropertyRelative("_notNull");
//                var isDefault = arg.FindPropertyRelative("_isDefault");
               // var @default = arg.FindPropertyRelative("_default");
                EditorGUI.indentLevel++;
                {
                    if (i != 0)
                    {
                        DrawUILine(Color.white);
                    }

                    EditorGUIUtility.labelWidth = 80f;
                    EditorGUILayout.PropertyField(argNotNull, new GUIContent("NotNull:"));
                    EditorGUIUtility.labelWidth = 60f;
                    EditorGUILayout.PropertyField(argName, new GUIContent("Name:"));
                    var position = EditorGUILayout.BeginHorizontal();
                    {
                        GUIStyle fontStyle = new GUIStyle
                        {
                            normal = { textColor = Color.red },
                            fontSize = EditorStyles.label.fontSize
                        };

                        EditorGUILayout.LabelField($"Type:{argTypeStr.stringValue.Split(',').First()}", fontStyle);
                        _selectType(argTypeStr, position);
                    }
                    EditorGUILayout.EndHorizontal();

//                    EditorGUIUtility.labelWidth = 80f;
//                    EditorGUILayout.PropertyField(isDefault, new GUIContent("Is Default:"));

//                    if (isDefault.boolValue)
//                    {
//                        EditorGUIUtility.labelWidth = 60f;
//                        EditorGUILayout.PropertyField(@default, new GUIContent("Default:"));
//                    }

                    EditorGUIUtility.labelWidth = 60f;
                    EditorGUILayout.PropertyField(argDesc, new GUIContent("Desc:"));
                }
                EditorGUI.indentLevel--;
            }

        }

        readonly List<bool> _foldoutState = new List<bool>();

        private bool _foldout(int index, int argCount)
        {
            if (_foldoutState.Count <= index)
            {
                _foldoutState.Add(false);
            }

            _foldoutState[index] = EditorGUILayout.Foldout(_foldoutState[index],
                $"EventArgCount:{argCount}", true);

            return _foldoutState[index];
        }

        private bool _removeConfirm = false;
        private void _removeAll()
        {
            if (_events.arraySize > 0)
            {
                if (_removeConfirm)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Confirm Remove All Event") || IsEnterClick())
                        {
                            _events.arraySize = 0;
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
                    if (_names != null && _names.Contains(_eventName))
                    {
                        return;
                    }


                    if (_ids != null && _ids.Contains(_id))
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

                    _addElement(_eventName, _id, _argCount);
                    //                    _addElement(_events, _eventName, false);
                    //                    _addElement(_eventIDs, _id);
                    //                    _addElement(_eventArgCount, _argCount);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void _addElement(string eventName, int eventID, int argCount)
        {
            _events.arraySize++;
            var @event = _events.GetArrayElementAtIndex(_events.arraySize - 1);
            var eventNameSer = @event.FindPropertyRelative("_eventName");
            eventNameSer.stringValue = eventName;
            var eventIDSer = @event.FindPropertyRelative("_eventId");
            eventIDSer.intValue = eventID;
            var eventArgsSer = @event.FindPropertyRelative("_args");
            eventArgsSer.arraySize = argCount;
            _initEventArgs(eventArgsSer);
        }
        /// <summary>
        /// 增加了arraySize后他会复制最后一个元素的值,所以在这里重置一下
        /// </summary>
        /// <param name="eventArgsSer"></param>
        private void _initEventArgs(SerializedProperty eventArgsSer)
        {
            for (var i = 0; i < eventArgsSer.arraySize; i++)
            {
                var arg = eventArgsSer.GetArrayElementAtIndex(i);
                var argName = arg.FindPropertyRelative("_argName");
                var _argTypeStr = arg.FindPropertyRelative("_argTypeStr");
                var _argDesc = arg.FindPropertyRelative("_argDesc");
                var _notNull = arg.FindPropertyRelative("_notNull");

                argName.stringValue = string.Empty;
                _argTypeStr.stringValue = typeof(object).AssemblyQualifiedName;
                _argDesc.stringValue = string.Empty;
                _notNull.boolValue = true;
            }
        }

        private void _selectType(SerializedProperty argTypeStr, Rect position)
        {
            if (GUILayout.Button("Select Type"))
            {
                var rect = GUILayoutUtility.GetLastRect();
                rect = new Rect(position.width, position.yMax / 2, rect.width, rect.height);
                FuzzyWindow.Show(rect,
                    _getOptionTree(_getCurrentType(argTypeStr.stringValue)), (option) =>
                    {
                        argTypeStr.stringValue = ((Type)option.value).AssemblyQualifiedName;

                        serializedObject.ApplyModifiedProperties();

                        FuzzyWindow.instance.Close();
                        InternalEditorUtility.RepaintAllViews();
                    });
            }
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