//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月30日-05:25
//Icarus.UnityGameFramework.Editor

using Bolt;
using Ludiq;
using System;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [Editor(typeof(IEventBaseUnit))]
    public class IEventBaseUnitEditor : UnitEditor
    {
        protected Metadata TableScriptableObject => metadata["EventTableAsset"];
        protected Metadata Table => metadata["EventTable"];

        protected Metadata EventId => metadata["EventId"];

        protected Metadata EventName => metadata["EventName"];

        public IEventBaseUnitEditor(Metadata metadata) : base(metadata)
        {
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            BeginBlock(metadata, position);
            {
                if (Table.value == null)
                {
                    Table.value = new EventTable();
                    return;
                }
                var table = (EventTable)Table.value;

                if (TableScriptableObject.value != null)
                {
                    var tableAsset = ((EventTableScriptableObject)
                        TableScriptableObject.value);


                    table.Events.Clear();
                    table.Events.AddRange(tableAsset.Table.Events);
                }

                if (EventId.value != null)
                {
                    var idInput = (ValueInput)EventId.value;
                    idInput.SetDefaultValue(table.SelectEventID);
                }

                if (EventName.value != null)
                {
                    var nameInput = (ValueInput)EventName.value;
                    nameInput.SetDefaultValue(table.SelectEventName);
                }
            }
            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
            }

            base.OnGUI(position, label);

        }
    }
}