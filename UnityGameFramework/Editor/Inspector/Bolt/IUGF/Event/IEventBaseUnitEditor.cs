//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月30日-05:25
//Icarus.UnityGameFramework.Editor

using System;
using Bolt;
using Ludiq;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [Editor(typeof(IEventBaseUnit))]
    public class IEventBaseUnitEditor : UnitEditor
    {

        protected Metadata _tableScriptableObject => metadata["EventTableAsset"];
        protected Metadata _table => metadata["EventTable"];

        protected Metadata _eventID => metadata["EventId"];

        protected Metadata _eventName => metadata["EventName"];

        public IEventBaseUnitEditor(Metadata metadata) : base(metadata)
        {
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            base.OnGUI(position, label);

            BeginBlock(metadata, position);
            {
                try
                {
                    if (_tableScriptableObject.value == null)
                    {
                        _table.value = new EventTable();
                        return;
                    }
                    
                    _table.value = ((EventTableScriptableObject)_tableScriptableObject.value).Table;
                    
                    if (_eventID.value != null)
                    {
                        var idInput = (ValueInput)_eventID.value;
                        idInput.SetDefaultValue(((EventTable)_table.value).SelectEventID);
                    }
                    
                    if (_eventName.value != null)
                    {
                        var nameInput = (ValueInput)_eventName.value;
                        nameInput.SetDefaultValue(((EventTable)_table.value).SelectEventName);
                    }
                }
                catch (Exception e1)
                {
                }

            }
            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
            }
        }
    }
}