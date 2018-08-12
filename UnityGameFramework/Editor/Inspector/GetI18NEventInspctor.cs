//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月12日-10:40
//Icarus.UnityGameFramework.Editor

using System;
using System.Collections.Generic;
using Assets.IGameFrameWork.UnityGameFramework.Runtime.I18N;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Icarus.UnityGameFramework.Editor
{
    [CustomEditor(typeof(GetI18NComponent))]
    public class GetI18NEventInspctor : GameFrameworkInspector
    {
        protected override bool CallBaseOnInspectorGUI { get; set; } = true;
        private GetI18NComponent _component;
        private const string MethodName = "set_text";
        void OnEnable()
        {
            _component = (GetI18NComponent)target;
        }

        private void _initCache()
        {
            _textCache.Clear();

            for (var i = 0; i < _component.OnGetI18NEvent.GetPersistentEventCount(); i++)
            {
                var text = _component.OnGetI18NEvent.GetPersistentTarget(i) as Text;
                var methodName = _component.OnGetI18NEvent.GetPersistentMethodName(i);

                if (text)
                {
                    if (methodName == MethodName)
                    {
                        _textCache.Add(text);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            _initCache();
            serializedObject.Update();
            if (GUILayout.Button("Add UGUI Text Event"))
            {
                var text = _getText();

                if (text)
                {
                    _addEvent(text);
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
            }

            base.OnInspectorGUI();

            Repaint();
        }

        private readonly List<Text> _textCache = new List<Text>();

        private void _addEvent(Text text)
        {
            if (_check(text)) return;
           
            var method = typeof(Text).GetMethod(MethodName);

            UnityEditor.Events.UnityEventTools
                .AddPersistentListener(_component.OnGetI18NEvent, (UnityAction<string>)method.CreateDelegate(typeof(UnityAction<string>), text));
        }

        private Text _getText()
        {
            var text = _component.GetComponent<Text>();
            return text;
        }
        
        private bool _check(Text text)
        {
            return _textCache.Contains(text);
        }
    }
}