//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月29日-11:30
//Icarus.UnityGameFramework.Editor

using Bolt;
using Ludiq;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [Editor(typeof(EventTableUnit))]
    public class EventTableUnitEditor : UnitEditor
    {

        public EventTableUnitEditor(Metadata metadata) : base(metadata)
        {
        }

        private Metadata _tableScriptableObject => metadata["_tableScriptableObject"];
        private Metadata _table => metadata["_table"];

        private Metadata _eventID => metadata["_eventID"];


        protected override void OnGUI(Rect position, GUIContent label)
        {
            BeginBlock(metadata, position);
            {
                if (_tableScriptableObject.value == null)
                {
                    _table.value = new EventTable();
                    return;
                }

                _table.value = ((EventTableScriptableObject)_tableScriptableObject.value).Table;

                var idInput = (ValueInput)_eventID.value;

                idInput.SetDefaultValue(((EventTable)_table.value).SelectEventID);
            }
            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
            }

            base.OnGUI(position, label);
        }
    }
}