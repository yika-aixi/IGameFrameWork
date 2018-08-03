//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月29日-07:52
//Icarus.UnityGameFramework.Bolt

using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    [Inspectable]
    public class EventTable
    {
        public readonly Dictionary<string, int> Events = new Dictionary<string, int>();
        public readonly Dictionary<int, List<KeyValuePair<string, Type>>> ArgsTable = new Dictionary<int, List<KeyValuePair<string, Type>>>();
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

            return ArgsTable[SelectEventID].Count;
        }

        public List<KeyValuePair<string, Type>> GetArgList()
        {
            if (!ArgsTable.ContainsKey(SelectEventID))
            {
                return null;
            }

            return ArgsTable[SelectEventID];
        }
    } 
}