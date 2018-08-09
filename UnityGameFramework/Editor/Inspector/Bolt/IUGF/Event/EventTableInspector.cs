//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月29日-11:20
//Icarus.UnityGameFramework.Editor

using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using Icarus.GameFramework;
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

        protected Metadata Events => metadata["Events"];

        protected Metadata SelectEvent => metadata[nameof(EventTable.SelectEvent)];

        protected Metadata SelectEventOfAssetName => metadata[nameof(EventTable.SelectEventOfAssetName)];

        private List<string> _names;
        private List<int> _ids;
        private int _selectIndex;
        private const string NoTable = "No Table";
        private const string NoEvent = "No Event";

        protected override void OnGUI(Rect position, GUIContent label)
        {
            var table = (EventTable)metadata.value;
            _names = new List<string>();
            _ids = new List<int>();
            var popRect = new Rect(position.x + 50, position.y, position.width, position.height);

            if (table.SelectEvent != null)
            {
                if (table.Events == null)
                {
                    _names.Add(table.SelectEvent.EventName);
                    _ids.Add(table.SelectEvent.EventID);
                    _selectIndex = 0;
                }
                else
                {
                    var nowEvent = table.GetEvent(table.SelectEvent.EventID);
                    _names.AddRange(table.Events.Select(x => x.EventName));
                    _ids.AddRange(table.Events.Select(x => x.EventID));
                    if (nowEvent == null || table.SelectEventOfAssetName != table.TableAssetName)
                    {
                        _names.Insert(0, table.SelectEvent.EventName);
                        _ids.Insert(0, table.SelectEvent.EventID);
                        _selectIndex = 0;
                    }
                    else
                    {
                        _initIndex(table.SelectEvent.EventID);
                    }
                }

            }
            else if (table.Events != null)
            {
                _names.AddRange(table.Events.Select(x => x.EventName));
                _ids.AddRange(table.Events.Select(x => x.EventID));
                _selectIndex = -1;
            }
            else
            {
                _names.Add(NoTable);
                _ids.Add(0);
                _selectIndex = 0;
            }

            if (_names.Count == 0)
            {
                _names.Add(NoEvent);
                _ids.Add(0);
                _selectIndex = 0;
            }

            _selectIndex = EditorGUI.Popup(popRect, _selectIndex, _names.ToArray());
            
            if (_selectIndex < 0 || 
                _names[_selectIndex] == NoTable ||
                _names[_selectIndex] == NoEvent)
            {
                return;
            }

            if (_selectIndex == 0)
            {
                if (!string.IsNullOrEmpty(table.SelectEventOfAssetName) && 
                    table.SelectEventOfAssetName != table.TableAssetName)
                {
                    return;
                }
            }

            if (table.SelectEvent != null)
            {
                if (table.SelectEventOfAssetName == table.TableAssetName)
                {
                    if (_ids[_selectIndex] == table.SelectEvent.EventID)
                    {
                        return;
                    }
                }
            }
            
            BeginBlock(metadata, position);
            {
                Debug.Log("设置");
                SelectEvent.value = table.GetEvent(_ids[_selectIndex]);
                var selectEventName = SelectEvent[nameof(EventEntity.EventName)];
                var selectEventId = SelectEvent[nameof(EventEntity.EventID)];
                selectEventName.value = _names[_selectIndex];
                selectEventId.value = _ids[_selectIndex];
                SelectEventOfAssetName.value = table.TableAssetName;
            }
            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
            }

        }

        private void _initIndex(int eventEventId)
        {
            for (var i = 0; i < _ids.Count; i++)
            {
                if (_ids[i] == eventEventId)
                {
                    _selectIndex = i;
                }
            }
        }
    }
}