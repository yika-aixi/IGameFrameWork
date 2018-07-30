//创建者:Icarus
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
using UnityEngine;

namespace Icarus.UnityGameFramework.Bolt
{
    [UnitCategory("Icarus/IUGF")]
    [UnitTitle("Event")]
    [UnitSubtitle("选择了EventTableAsset后将可以进行快速设置")]
    public class EventUnit : GameFrameWorkBaseUnit, IEventBaseUnit
    {
        private static EventTableScriptableObject _oldEventTableAsset;

        [Inspectable, UnitHeaderInspectable]
        public EventTableScriptableObject EventTableAsset { get; private set; }

        [Inspectable, UnitHeaderInspectable("Events")]
        public EventTable EventTable { get; private set; }

        [DoNotSerialize]
        [PortLabel("Event ID")]
        public ValueInput EventId { get; private set; }

        [DoNotSerialize]
        public ValueInput EventName { get; private set; }

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

        private EventHandler<GameEventArgs> _handler;

        private ValueOutput[] _argsOut;

        private ValueInput[] _argsIn;

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
                    _setEventArgCount();
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
                    for (var i = 0; i < EventArgCount; i++)
                    {
                        var index = i;
                        _argsOut[i] = ValueOutput($"{nameof(_argsOut)}_{i}", x => _args[index]);
                    }

                    Succession(_enter, _triggerExit);
                    break;
                case EventCallType.释放事件:
                    _handleIn = ValueInput<EventHandler<GameEventArgs>>(nameof(_handleIn));
                    break;
                case EventCallType.触发事件:
                    _atOnceTrigger = ValueInput(nameof(_atOnceTrigger), false);
                    _argsIn = new ValueInput[EventArgCount];
                    for (var i = 0; i < EventArgCount; i++)
                    {
                        _argsIn[i] = ValueInput<object>($"{nameof(_argsIn)}_{i}");
                        Requirement(_argsIn[i], _enter);
                    }
                    break;
            }

            Succession(_enter, _exit);

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

        private void _setEventArgCount()
        {
            if (EventTableAsset == null || EventTable == null)
            {
                return;
            }

            EventArgCount = EventTable.GetArgCount();
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