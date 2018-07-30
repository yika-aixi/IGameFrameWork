//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月29日-07:57
//Icarus.UnityGameFramework.Bolt

using System;
using System.Linq;
using Icarus.UnityGameFramework.Editor;
using UnityEditor;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [CustomEditor(typeof(EventTableScriptableObject))]
    public class EventTableUnityInspector : GameFrameworkInspector
    {
        private EventTableScriptableObject _table;
        private SerializedProperty _eventNames;
        private SerializedProperty _eventIDs;
        private SerializedProperty _eventArgs;
        private string[] _names;
        private int[] _ids;
        private void OnEnable()
        {
            _table = (EventTableScriptableObject)target;
            _eventNames = serializedObject.FindProperty("_eventNames");
            _eventIDs = serializedObject.FindProperty("_eventIds");
            _eventArgs = serializedObject.FindProperty("_eventArgs");
        }

        private bool _addMode;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            _names = _table.GetEventNames().ToArray();
            _ids = _table.GetEventIDs().ToArray();

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
            EditorGUILayout.BeginHorizontal();
            {
                _showArray(_eventNames, guiLayout: GUILayout.Height(18));
                _showArray(_eventIDs,guiLayout: GUILayout.Height(18));
                _showArray(_eventArgs, true, GUILayout.Height(18));
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        private void _removeAll()
        {
            if (_eventNames.arraySize > 0)
            {
                if (GUILayout.Button("Remove All Event"))
                {
                    _eventNames.arraySize = 0;
                    _eventIDs.arraySize = 0;
                    _eventArgs.arraySize = 0;
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

                if (GUILayout.Button("ADD"))
                {
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
                            var maxID = _ids.OrderBy(x=>x).Last();
                            _id = maxID + 1;
                        }
                    }

                    _addElement(_eventNames, _eventName, false);
                    _addElement(_eventIDs, _id);
                    _addElement(_eventArgs, _argCount);
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

        private void _showArray(SerializedProperty arraySer, bool showRemove = false, params GUILayoutOption[] guiLayout)
        {
            EditorGUILayout.BeginVertical();
            {
                for (int i = 0; i < arraySer.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(arraySer.GetArrayElementAtIndex(i), new GUIContent(""), guiLayout);
                        if (showRemove)
                        {
                            if (GUILayout.Button("Remove", guiLayout))
                            {
                                _eventNames.DeleteArrayElementAtIndex(i);
                                _eventIDs.DeleteArrayElementAtIndex(i);
                                _eventArgs.DeleteArrayElementAtIndex(i);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}