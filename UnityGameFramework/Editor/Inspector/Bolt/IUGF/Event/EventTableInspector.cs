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

        private Metadata _argsTable => metadata["ArgsTable"];

        private Metadata _selectEventName => metadata["SelectEventName"];

        private Metadata _selectEventID => metadata["SelectEventID"];
        
        private string[] _names = { "No Table" };
        private int[] _ids;
        private int _index;
        private bool _eror;
        protected override void OnGUI(Rect position, GUIContent label)
        {
            _names = new[] { "No Table" };

            _ids = new[] { 0 };
            try
            {
                var table = (Dictionary<string, int>)_events.value;

                if (table.Count != 0)
                {
                    _names = table.Keys.ToArray();
                    _ids = table.Values.ToArray();
                }

                _eror = false;
            }
            catch (Exception e)
            {
                _eror = true;
            }

            BeginBlock(metadata, position);
            var popRect = new Rect(position.x + 50, position.y, position.width, position.height);
            _index = EditorGUI.IntPopup(popRect, _index, _names, _ids);

            if (_eror)
            {
                return;
            }

            try
            {
                _selectEventName.value = _names[_index];

                _selectEventID.value = _ids[_index];
                
                if (EndBlock(metadata))
                {
                    metadata.RecordUndo();
                }
            }
            catch (Exception e)
            {
                _index = 0;
            }

        }
    }
}