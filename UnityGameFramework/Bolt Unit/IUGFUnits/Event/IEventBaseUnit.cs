//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月30日-05:12
//Icarus.UnityGameFramework.Bolt

using System;
using System.Collections.Generic;
using Bolt;
using Ludiq;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    public interface IEventBaseUnit
    {
        EventTableScriptableObject EventTableAsset { get;}

        EventTable EventTable { get; }

        ValueInput EventId { get; }

        ValueInput EventName { get; }

        /// <summary>
        /// 参数列表
        /// key : 参数名
        /// value : 参数类型
        /// </summary>
        List<KeyValuePair<string, Type>> ArgList { get; }
        
        int EventArgCount { get; }
    }
}