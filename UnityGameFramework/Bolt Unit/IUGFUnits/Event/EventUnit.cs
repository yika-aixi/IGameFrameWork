﻿//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月27日-05:56
//Icarus.UnityGameFramework.Bolt

using Bolt;
using Icarus.GameFramework;
using Icarus.GameFramework.Event;
using Icarus.UnityGameFramework.Bolt.Event;
using Icarus.UnityGameFramework.Runtime;
using Ludiq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Icarus.UnityGameFramework.Bolt
{
    [UnitCategory("Icarus/IUGF")]
    [UnitTitle("Event")]
    [UnitSubtitle("选择了EventTableAsset后将可以进行快速设置")]
    public class EventUnit : GameFrameWorkBaseUnit, IEventBaseUnit
    {
        [DoNotSerialize]
        private static EventTableScriptableObject _oldEventTableAsset;
        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("TableAsset")]
        public EventTableScriptableObject EventTableAsset { get; private set; }

        [Serialize]
        [Inspectable, UnitHeaderInspectable("Events")]
        public EventTable EventTable { get; private set; }
        
        [DoNotSerialize]
        [PortLabel("Event ID")]
        public ValueInput EventId { get; private set; }

        [DoNotSerialize]
        public ValueInput EventName { get; private set; }
        [Serialize]
        public List<ArgEntity> ArgList { get; private set; }

        [Serialize]
        [Inspectable, UnitHeaderInspectable("ArgCount")]
        public int EventArgCount { get; private set; }

        [Serialize]
        [Inspectable, UnitHeaderInspectable("Event Call Type:")]
        [InspectorToggleLeft]
        public EventCallType _eventCallType;


        [DoNotSerialize]
        [PortLabel("Event Trigger")]
        public ControlOutput _triggerExit;

        [DoNotSerialize]
        [PortLabel("立刻触发")]
        public ValueInput _atOnceTrigger;

        [DoNotSerialize]
        [PortLabel("Event Handle")]
        public ValueInput _handleIn;

        [DoNotSerialize]
        [PortLabel("Event Handle")]
        public ValueOutput _handleOut;

        [DoNotSerialize]
        private EventHandler<GameEventArgs> _handler;
        [DoNotSerialize]
        private ValueOutput[] _argsOut;
        [DoNotSerialize]
        private ValueInput[] _argsIn;
        [DoNotSerialize]
        private object[] _args;

        public EventUnit()
        {
            //备份现在所选事件表或设置事件表
            _setBackOrAsset();
        }

        protected override void Definition()
        {
            base.Definition();

            EventId = ValueInput(nameof(EventId), 0);

            switch (_eventCallType)
            {
                case EventCallType.单次事件:
                case EventCallType.注册事件:
                case EventCallType.触发事件:
                    _setEventArgCountAndArgList();
                    _args = new object[EventArgCount];
                    break;
            }

            switch (_eventCallType)
            {
                case EventCallType.单次事件:
                case EventCallType.注册事件:
                    _triggerExit = ControlOutput(nameof(_triggerExit));

                    if (_eventCallType == EventCallType.注册事件)
                    {
                        _handleOut = ValueOutput(nameof(_handleOut), x => _handler);
                    }
                    _argsOut = new ValueOutput[EventArgCount];
                    _setArgList(true);

                    Succession(_enter, _triggerExit);
                    break;
                case EventCallType.释放事件:
                    _handleIn = ValueInput<EventHandler<GameEventArgs>>(nameof(_handleIn));
                    break;
                case EventCallType.触发事件:
                    _atOnceTrigger = ValueInput(nameof(_atOnceTrigger), false);
                    _argsIn = new ValueInput[EventArgCount];
                    _setArgList(false);
                    break;
            }

            Succession(_enter, _exit);
        }

        void _setArgList(bool isOut)
        {
            if (ArgList != null && ArgList.Count == EventArgCount)
            {
                for (var i = 0; i < ArgList.Count; i++)
                {
                    var arg = ArgList[i];
                    var argName = arg.ArgName;
                    var index = i;

                    if (string.IsNullOrWhiteSpace(argName))
                    {
                        argName = $"Arg {i}";
                    }

                    if (isOut)
                    {
                        _setArgOut(argName, arg.ArgType, index);
                    }
                    else
                    {
                        _setArgIn(argName, arg.ArgType, index);
                        if (arg.NotNull)
                        {
                            Requirement(_argsIn[index], _enter);
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < EventArgCount; i++)
                {
                    var index = i;
                    if (isOut)
                    {
                        _setArgOut($"{nameof(_argsOut)}_{i}", typeof(object), index);
                    }
                    else
                    {
                        _setArgIn($"{nameof(_argsIn)}_{i}", typeof(object), index);
                        Requirement(_argsIn[index], _enter);
                    }
                }
            }
        }

        private void _setArgIn(string argName, Type argValue, int index)
        {
            _argsIn[index] = ValueInput(argValue, argName);
        }

        private void _setArgOut(string argName, Type argValue, int index)
        {
            _argsOut[index] = ValueOutput(argValue, argName, x => _args[index]);
        }

        private void _setBackOrAsset()
        {
            if (EventTableAsset != null)
            {
                _oldEventTableAsset = EventTableAsset;
            }
            else
            {
                if (_oldEventTableAsset != null)
                {
                    EventTableAsset = _oldEventTableAsset;
                }
            }
        }

        private void _setEventArgCountAndArgList()
        {
            //没有事件表资源初始化
            if (EventTable == null || EventTable.Events.Count == 0)
            {
                return;
            }

            EventArgCount = EventTable.GetArgCount();
            
            ArgList = EventTable.GetArgList();
        }

        private void _checkArgCount()
        {
            if (EventArgCount < 0)
            {
                throw new GameFrameworkException("事件的参数个数不能为负数");
            }
        }

        private Flow _flow;
        protected override ControlOutput Enter(Flow flow)
        {
            _checkArgCount();

            var eventID = flow.GetValue<int>(EventId);

            var eventC = GameEntry.GetComponent<EventComponent>();

            if (!eventC)
            {
                throw new GameFrameworkException("EventComponent 没有注册到 GameEntry");
            }

            switch (_eventCallType)
            {
                case EventCallType.单次事件:
                case EventCallType.注册事件:
                    _flow = Flow.New(flow.stack.ToReference());
                    break;
            }

            switch (_eventCallType)
            {
                case EventCallType.单次事件:
                    eventC.Subscribe(eventID, _handleD);
                    break;
                case EventCallType.注册事件:
                    eventC.Subscribe(eventID, _handle);
                    _handler = _handle;
                    break;
                case EventCallType.释放事件:
                    var handle = flow.GetValue<EventHandler<GameEventArgs>>(_handleIn);
                    _unSubscribe(eventID, handle);
                    break;
                case EventCallType.触发事件:

                    if (!eventC.Check(eventID))
                    {
                        break;
                    }

                    var atOnce = flow.GetValue<bool>(_atOnceTrigger);
                    var eventArgs = new BoltEventArgs(eventID);

                    for (var i = 0; i < _argsIn.Length; i++)
                    {
                        _args[i] = flow.GetValue<object>(_argsIn[i]);
                    }
                    eventArgs.SetArgs(_args);
                    if (atOnce)
                    {
                        eventC.FireNow(null, eventArgs);
                    }
                    else
                    {
                        eventC.Fire(null, eventArgs);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return _exit;
        }

        private void _unSubscribe(int id, EventHandler<GameEventArgs> handle)
        {
            var eventC = GameEntry.GetComponent<EventComponent>();
            eventC.Unsubscribe(id, handle);
        }

        //单次事件
        private void _handleD(object sender, GameEventArgs e)
        {
            _handle(sender, e);
            _unSubscribe(e.Id, _handleD);
            _display(e.Id);
        }

        private void _display(int Id)
        {
            _flow.Dispose();
            _flow = null;
        }

        private void _handle(object sender, GameEventArgs e)
        {
            _args = ((BoltEventArgs)e).Args;

            _flow.Invoke(_triggerExit);
        }

    }
}