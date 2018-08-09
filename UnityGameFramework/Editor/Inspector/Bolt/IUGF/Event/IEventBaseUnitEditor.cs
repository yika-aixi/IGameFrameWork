//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月30日-05:25
//Icarus.UnityGameFramework.Editor

using Bolt;
using Ludiq;
using System;
using System.Linq;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [Editor(typeof(IEventBaseUnit))]
    public class IEventBaseUnitEditor : UnitEditor
    {
        protected Metadata TableScriptableObject => metadata[nameof(IEventBaseUnit.EventTableAsset)];
        protected Metadata Table => metadata[nameof(IEventBaseUnit.EventTable)];

        protected Metadata EventId => metadata[nameof(IEventBaseUnit.EventId)];

        protected Metadata EventName => metadata[nameof(IEventBaseUnit.EventName)];

        public IEventBaseUnitEditor(Metadata metadata) : base(metadata)
        {
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            base.OnGUI(position, label);

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

                    EventEntity[] entities = new EventEntity[tableAsset.Table.Events.Count];
                    tableAsset.Table.Events.CopyTo(entities, 0);
                    table.Events = entities.ToList();
                    var tableAssetName = Table[nameof(EventTable.TableAssetName)];
                    tableAssetName.value = tableAsset.name;
                }

                if (EventId.value != null)
                {
                    var idInput = (ValueInput)EventId.value;

                    if (table.SelectEvent != null)
                    {
                        idInput.SetDefaultValue(table.SelectEvent.EventID);
                    }
                }

                if (EventName.value != null)
                {
                    var nameInput = (ValueInput)EventName.value;
                    if (table.SelectEvent != null)
                    {
                        nameInput.SetDefaultValue(table.SelectEvent.EventName);
                    }
                }
            }
            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
            }
        }
    }
}