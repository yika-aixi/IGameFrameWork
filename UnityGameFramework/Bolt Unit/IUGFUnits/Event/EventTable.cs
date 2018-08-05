//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月29日-07:52
//Icarus.UnityGameFramework.Bolt

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [System.Serializable]
    public class ArgEntity
    {
        [SerializeField]
        private string _argName;
        [SerializeField]
        private string _argTypeStr;
        [TextArea(2,5,order = 20)]
        [SerializeField]
        private string _argDesc;
        [SerializeField]
        private bool _notNull;

        public ArgEntity(ArgEntity arg)
        {
            _argName = arg.ArgName;
            _argTypeStr = arg.ArgTypeStr;
            _argDesc = arg.ArgDesc;
            _notNull = arg.NotNull;
        }

        public ArgEntity():this(string.Empty,typeof(object).AssemblyQualifiedName,string.Empty,true)
        {
        }

        public ArgEntity(string argName, string argTypeStr, string argDesc, bool notNull)
        {
            _argName = argName;
            _argTypeStr = argTypeStr;
            _argDesc = argDesc;
            _notNull = notNull;
        }

        public ArgEntity(string argName, string argAssemblyQualifiedName)
        {
            _argName = argName;
            _argTypeStr = argAssemblyQualifiedName;
        }

        /// <summary>
        /// 参数名
        /// </summary>
        public string ArgName => _argName;

        /// <summary>
        /// 参数类型
        /// </summary>
        public Type ArgType => Type.GetType(_argTypeStr);

        /// <summary>
        /// 参数描述
        /// </summary>
        public string ArgDesc => _argDesc;

        /// <summary>
        /// 参数不能为空
        /// </summary>
        public bool NotNull => _notNull;

        public string ArgTypeStr => _argTypeStr;

        public override string ToString()
        {
            return ArgName;
        }

    }

    [System.Serializable]
    public class EventEntity
    {
        [SerializeField]
        private string _eventName;
        [SerializeField]
        private int _eventId;
        [SerializeField]
        private List<ArgEntity> _args;

        public EventEntity(string eventName, int eventID)
        {
            this._eventName = eventName;
            this._eventId = eventID;
        }

        public string EventName => _eventName;

        public int EventID => _eventId;

        public List<ArgEntity> Args => _args;

        public override string ToString()
        {
            return $"EventName:{EventName},EventID:{EventID}";
        }
    }

    public class EventTable
    {
        public readonly List<EventEntity> Events = new List<EventEntity>();
        public string SelectEventName { get; set; } = "None Event";
        public int SelectEventID { get; set; }

        public override string ToString()
        {
            return $"Select Event Name:{SelectEventName},ID:{SelectEventID}";
        }

        public EventEntity GetEvent(int eventID)
        {
            var @event = Events.FirstOrDefault(x => x.EventID == SelectEventID);

            return @event;
        }

        public int GetArgCount()
        {
            var @event = GetEvent(SelectEventID);
            if (@event == null)
            {
                return 0;
            }

            return @event.Args.Count;
        }

        public List<ArgEntity> GetArgList()
        {
            var @event = GetEvent(SelectEventID);

            return @event?.Args;
        }
    }
}