//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月29日-11:20
//Icarus.UnityGameFramework.Editor

using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq;
using UnityEditor;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [Inspector(typeof(EventTable))]
    public class EventableInspector : Inspector
    {
        public EventableInspector(Metadata metadata) : base(metadata)
        {
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            return 20f;
        }

        private Metadata _events => metadata["Events"];
        
        private Metadata _selectEventName => metadata["SelectEventName"];

        private Metadata _selectEventID => metadata["SelectEventID"];

        private string[] _names = { "No Table" };
        private int[] _ids;
        private int _index = -1;
        private bool _eror;
        protected override void OnGUI(Rect position, GUIContent label)
        {
            _names = new[] { "No Table" };

            _ids = new[] { 0 };
            try
            {
                var events = (List<EventEntity>)_events.value;

                if (events.Count != 0)
                {
                    _names = events.Select(x => x.EventName).ToArray();
                    _ids = events.Select(x => x.EventID).ToArray(); ;
                    _initIndex();
                }

                _eror = false;
            }
            catch (Exception)
            {
                _eror = true;
            }

            BeginBlock(metadata, position);
            {
                var popRect = new Rect(position.x + 50, position.y, position.width, position.height);
                _index = EditorGUI.Popup(popRect, _index, _names);

                if (_eror)
                {
                    return;
                }

                try
                {
                    var index = _index;
                    _selectEventName.value = _names[index];
                    _selectEventID.value = _ids[index];
                }
                catch (Exception)
                {
                    _index = -1;
                }
            }
            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
            }

        }

        private void _initIndex()
        {
            string selectName = (string) _selectEventName.value;
            for (var i = 0; i < _names.Length; i++)
            {
                if (_names[i] == selectName)
                {
                    _index = i;
                }
            }
        }
    }
}