//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月01日-11:20
//Icarus.UnityGameFramework.Bolt

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [CreateAssetMenu(fileName = "New EventTable", menuName = "Event Table", order = 1)]
    public class EventTableScriptableObject : ScriptableObject, ISerializationCallbackReceiver
    {
        private readonly EventTable _table = new EventTable();

        private int _eventCount;

        [SerializeField]
        private EventEntity[] _events;
        
        public EventTable Table => _table;

        public void OnBeforeSerialize()
        {
            _init();
            int i = 0;
            foreach (var @event in _table.Events)
            {
                _events[i] = @event;

                i++;
            }
        }

        private void _init()
        {
            _eventCount = _table.Events.Count;
            _events = new EventEntity[_eventCount];
        }

        public void OnAfterDeserialize()
        {
            _table.Events.Clear();

            if (_events != null)
            {
                foreach (var @event in _events)
                {
                    if (@event.Args != null && @event.Args.Count > 0)
                    {
                        for (var i = 0; i < @event.Args.Count; i++)
                        {
                            if (@event.Args[i].ArgType == null)
                            {
                                @event.Args[i] = new ArgEntity();
                            }
                        }
                    }
                    _table.Events.Add(@event);
                }
            }
            
        }

        public IEnumerable<string> GetEventNames()
        {
            return _table.Events.Select(x=>x.EventName);
        }

        public IEnumerable<int> GetEventIDs()
        {
            return _table.Events.Select(x => x.EventID); ;
        }
    }
}