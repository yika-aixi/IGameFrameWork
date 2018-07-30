//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月29日-07:52
//Icarus.UnityGameFramework.Bolt

using System.Collections.Generic;
using Ludiq;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [Inspectable]
    public class EventTable
    {
        public readonly Dictionary<string, int> Events = new Dictionary<string, int>();
        public readonly Dictionary<int, int> ArgsTable = new Dictionary<int, int>();
        public string SelectEventName { get; set; } = "None Event";

        public int SelectEventID { get; set; }
        
        public override string ToString()
        {
            return $"Select Event Name:{SelectEventName},ID:{SelectEventID}";
        }

        public int GetArgCount()
        {
            if (!ArgsTable.ContainsKey(SelectEventID))
            {
                return 0;
            }

            return ArgsTable[SelectEventID];
        }
    }

    [CreateAssetMenu(fileName = "New EventTable", menuName = "Event Table", order = 1)]
    public class EventTableScriptableObject : ScriptableObject, ISerializationCallbackReceiver
    {
        private readonly EventTable _table = new EventTable();
        private int _eventCount;
        [SerializeField]
        private string[] _eventNames;
        [SerializeField]
        private int[] _eventIds;
        [SerializeField]
        private int[] _eventArgs;

        public EventTable Table => _table;

        public void OnBeforeSerialize()
        {
            _eventCount = _table.Events.Count;
            _eventNames = new string[_eventCount];
            _eventIds = new int[_eventCount];
            _eventArgs = new int[_eventCount];

            int i = 0;
            foreach (var @event in _table.Events)
            {
                _eventNames[i] = @event.Key;
                _eventIds[i] = @event.Value;
                _eventArgs[i] = _getArgs(@event.Value);
                i++;
            }
        }

        private int _getArgs(int eventId)
        {
            if (!_table.ArgsTable.ContainsKey(eventId))
            {
                return 0;
            }

            return _table.ArgsTable[eventId];
        }

        public void OnAfterDeserialize()
        {
            _table.Events.Clear();
            _table.ArgsTable.Clear();
            for (var i = 0; i < _eventNames.Length; i++)
            {
                _table.Events.Add(_eventNames[i], _eventIds[i]);
                _table.ArgsTable.Add(_eventIds[i], _eventArgs[i]);
            }
        }

        public IEnumerable<string> GetEventNames()
        {
            return _table.Events.Keys;
        }

        public IEnumerable<int> GetEventIDs()
        {
            return _table.Events.Values;
        }

    }
}