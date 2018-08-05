//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月02日-01:58
//Icarus.UnityGameFramework.Bolt

using System;
using System.Collections.Generic;
using Bolt;
using Icarus.UnityGameFramework.Bolt.Event;
using Ludiq;
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt.Units
{
    //[UnitCategory("Icarus/Util/Events")]
    [UnitCategory("Events")]
    [UnitTitle("NewCustomEvent")]
    [UnitOrder(0)]
    public class NewCustomEventUnit : GameObjectEventUnit<CustomEventArgs>, IEventBaseUnit
    {
        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("TableAsset")]
        public EventTableScriptableObject EventTableAsset { get; private set; }
        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Events")]
        public EventTable EventTable { get; private set; }
        [DoNotSerialize]
        public ValueInput EventId { get; private set; }
        [DoNotSerialize]
        [PortLabel("Event Name")]
        public ValueInput EventName { get; private set; }
        [Serialize]
        public List<ArgEntity> ArgList { get; private set; }

        [Serialize]
        [Inspectable, UnitHeaderInspectable("ArgCount")]
        public int EventArgCount { get; private set; }

        protected override string hookName => EventHooks.Custom;

        [DoNotSerialize]
        public List<ValueOutput> argumentPorts { get; } = new List<ValueOutput>();

        protected override void Definition()
        {
            base.Definition();

            EventName = ValueInput(nameof(EventName), string.Empty);

            argumentPorts.Clear();
            _setEventArgCountAndArgList();
            if (ArgList != null && ArgList.Count == EventArgCount)
            {
                for (var i = 0; i < ArgList.Count; i++)
                {
                    var arg = ArgList[i];
                    var argName = arg.ArgName;

                    if (string.IsNullOrWhiteSpace(argName))
                    {
                        argName = $"argument_{i}";
                    }

                    argumentPorts.Add(ValueOutput(arg.ArgType, argName));
                }
            }
            else
            {
                for (var i = 0; i < EventArgCount; i++)
                {

                    argumentPorts.Add(ValueOutput<object>("argument_" + i));
                }
            }
        }
        private void _setEventArgCountAndArgList()
        {
            if (EventTableAsset == null || EventTable == null)
            {
                return;
            }

            EventArgCount = EventTable.GetArgCount();
            ArgList = EventTable.GetArgList();
        }

        protected override bool ShouldTrigger(Flow flow, CustomEventArgs args)
        {
            return CompareNames(flow, EventName, args.name);
        }

        protected override void AssignArguments(Flow flow, CustomEventArgs args)
        {
            for (var i = 0; i < EventArgCount; i++)
            {
                flow.SetValue(argumentPorts[i], args.arguments[i]);
            }
        }

        public static void Trigger(GameObject target, string name, params object[] args)
        {
            EventBus.Trigger(EventHooks.Custom, target, new CustomEventArgs(name, args));
        }
    }
}