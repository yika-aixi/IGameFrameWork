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
        private string[] _eventNames;
        [SerializeField]
        private int[] _eventIds;
        [SerializeField]
        private int[] _eventArgCount;

        [SerializeField]
        private List<string> _eventArgNames;
        [SerializeField]
        private List<string> _eventArgTypeNames;


        public EventTable Table => _table;

        /// <summary>
        /// 参数组
        /// key : 事件ID
        /// value : 开始位置,结束位置
        /// </summary>
        public Dictionary<int, int[]> ArgGroup;

        public void OnBeforeSerialize()
        {
            _init();
            int i = 0;
            int startIndex = 0;
            foreach (var @event in _table.Events)
            {
                ArgGroup.Add(@event.Value, new int[2]);
                _eventNames[i] = @event.Key;
                _eventIds[i] = @event.Value;
                _eventArgCount[i] = _getArgCount(@event.Value);
                var args = _getArgs(@event.Value);

                _setGroup(args, startIndex, @event.Value, args.Count);

                foreach (var type in args)
                {
                    _eventArgNames.Add(type.Key);
                    var value = type.Value;
                    if (value == null)
                    {
                        value = typeof(object);
                    }
                    _eventArgTypeNames.Add(value.AssemblyQualifiedName);
                    startIndex++;
                }

                if (_eventArgCount[i] == 0 || _eventArgNames.Count > 0 && _eventArgNames.Last() != NullStr)
                {
                    //在后面加入一个null
                    _eventArgNames.Add(NullStr);
                    _eventArgTypeNames.Add(NullStr);
                    startIndex++;
                }

                i++;
            }
        }

        private void _setGroup(List<KeyValuePair<string, Type>> index, int startIndex, int eventID, int argCount)
        {
            ArgGroup[eventID][0] = startIndex;
            ArgGroup[eventID][1] = startIndex + argCount;
        }

        public const string NullStr = "Null";
        private void _init()
        {
            _eventCount = _table.Events.Count;
            _eventNames = new string[_eventCount];
            _eventIds = new int[_eventCount];
            _eventArgCount = new int[_eventCount];
            _eventArgNames = new List<string>();
            _eventArgTypeNames = new List<string>();
            ArgGroup = new Dictionary<int, int[]>();
        }

        private int _getArgCount(int eventId)
        {
            return _table.ArgsTable[eventId].Count;
        }

        private List<KeyValuePair<string, Type>> _getArgs(int eventId)
        {
            return _table.ArgsTable[eventId];
        }

        public void OnAfterDeserialize()
        {
            _table.Events.Clear();
            _table.ArgsTable.Clear();

            //初始化字典
            for (int i = 0; i < _eventNames.Length; i++)
            {
                //将事件加入字典
                _table.Events.Add(_eventNames[i], _eventIds[i]);
                _table.ArgsTable.Add(_eventIds[i], new KeyValuePair<string, Type>[_eventArgCount[i]].ToList());
            }

            int argCount = 0;
            for (int i = 0; i < _eventNames.Length; i++)
            {
                //获取事件参数个数
                var count = _eventArgCount[i];

                for (var j = argCount; j < count + argCount; j++)
                {
                    if (_eventArgNames[j] == NullStr)
                    {
                        _eventArgNames.Insert(j, String.Empty);
                        _eventArgTypeNames.Insert(j, typeof(object).AssemblyQualifiedName);
                    }
                }

                argCount += count + 1;
            }

            int index = 0;

            foreach (var @event in _table.ArgsTable)
            {
                var args = @event.Value;
                for (var i = 0; i < args.Count; i++)
                {
                    var argName = _eventArgNames[index];
                    var argTypeName = _eventArgTypeNames[index];

                    args[i] = new KeyValuePair<string, Type>(argName, Type.GetType(argTypeName));
                    index++;
                }
                index++;
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