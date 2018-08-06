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

        private string[] _names;
        private int[] _ids;
        private int _selectId;
        private int _nowSelectId;
        private bool _eror;
        private const string NoTable = "No Table";
        protected override void OnGUI(Rect position, GUIContent label)
        {
            if (!string.IsNullOrEmpty((string)_selectEventName.value))
            {
                _names = new[] { (string)_selectEventName.value };

                _ids = new[] { (int)_selectEventID.value };

                _initIndex();

            }
            else
            {
                _names = new[] { NoTable };

                _ids = new[] { 0 };
            }

            if (_events.value != null)
            {
                var events = (List<EventEntity>)_events.value;

                if (events.Count != 0)
                {
                    _names = events.Select(x => x.EventName).ToArray();
                    _ids = events.Select(x => x.EventID).ToArray(); ;
                }
                _eror = false;
            }
            else
            {
                _eror = true;
            }
            
            BeginBlock(metadata, position);
            {
                var popRect = new Rect(position.x + 50, position.y, position.width, position.height);

                _nowSelectId = EditorGUI.IntPopup(popRect, _nowSelectId, _names, _ids);


                #region 避免选择不同得事件表没有找到事件重置的问题

                bool hit = false;
                foreach (var id in _ids)
                {
                    if (id == _selectId)
                    {
                        _selectId = _nowSelectId;
                        hit = true;
                    }
                }

                #endregion
                
                if (_eror || !hit)
                {
                    return;
                }

                int index = 0;

                for (var i = 0; i < _ids.Length; i++)
                {
                    if (_ids[i] == _selectId)
                    {
                        index = i;
                    }
                }

                //没有事件表,不赋值
                if(_names[index] == NoTable)
                {
                    return;
                }

                _selectEventName.value = _names[index];
                _selectEventID.value = _ids[index];
            }
            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
            }

        }

        private void _initIndex()
        {
            _selectId = (int)_selectEventID.value;
            _nowSelectId = _selectId;
        }
    }
}