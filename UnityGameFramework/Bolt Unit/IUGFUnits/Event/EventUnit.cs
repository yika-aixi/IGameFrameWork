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
    [UnitSubtitle("参数个数设置为负数或改变数值,将会使用输入的'EventID'去查找或修改记录的参数个数")]
    public class EventUnit : Unit
    {
        [Serialize]
        [Inspectable, UnitHeaderInspectable("Event Call Type:")]
        [InspectorToggleLeft]
        public EventCallType _eventCallType;

        [Serialize]
        [Inspectable, UnitHeaderInspectable("参数个数")]
        [InspectorToggleLeft]
        public int _argCount = -1;

        [Serialize]
        [Inspectable, UnitHeaderInspectable("EventID")]
        [InspectorToggleLeft]
        public int _eventID;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput _enter;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput _exit;

        [DoNotSerialize]
        [PortLabel("Event Trigger")]
        public ControlOutput _triggerExit;

        [DoNotSerialize]
        [PortLabel("Event ID")]
        public ValueInput _eventIDIn;

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


        private static readonly Dictionary<int, int> _eventArgsCount = new Dictionary<int, int>();
        private bool _isAuto = false;
        protected override void Definition()
        {
            _enter = ControlInput(nameof(_enter), __enter);

            _exit = ControlOutput(nameof(_exit));

            _eventIDIn = ValueInput(nameof(_eventIDIn), _eventID);

            if (_argCount < 0)
            {
                _argCount = _getEventArgsCount();
                _isAuto = true;
            }
            else
            {
                if (!_isAuto)
                {
                    _addOrUpdateEventArgsCountItem(_eventID);
                }
                else
                {
                    _isAuto = false;
                }
                
            }

            switch (_eventCallType)
            {
                case EventCallType.单次事件:
                case EventCallType.注册事件:
                case EventCallType.触发事件:
                    _args = new object[_argCount];
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

                    _argsOut = new ValueOutput[_argCount];
                    for (var i = 0; i < _argsOut.Length; i++)
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
                    _argsIn = new ValueInput[_argCount];
                    for (var i = 0; i < _argsIn.Length; i++)
                    {
                        _argsIn[i] = ValueInput<object>($"{nameof(_argsIn)}_{i}");
                        Requirement(_argsIn[i], _enter);
                    }
                    break;
            }

            Succession(_enter, _exit);
        }

        private Flow _flow;
        private ControlOutput __enter(Flow flow)
        {
            _eventID = flow.GetValue<int>(_eventIDIn);

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
                    eventC.Subscribe(_eventID, _handleD);
                    break;
                case EventCallType.注册事件:
                    eventC.Subscribe(_eventID, _handle);
                    _handler = _handle;
                    break;
                case EventCallType.释放事件:
                    _unSubscribe(_eventID, _handle);
                    break;
                case EventCallType.触发事件:

                    if (!eventC.Check(_eventID))
                    {
                        break;
                    }

                    var atOnce = flow.GetValue<bool>(_atOnceTrigger);
                    var eventArgs = new BoltEventArgs(_eventID);
                    
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

        private void _unSubscribe(int id,EventHandler<GameEventArgs> handle)
        {
            var eventC = GameEntry.GetComponent<EventComponent>();
            eventC.Unsubscribe(id, _handleD);
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

        int _getEventArgsCount()
        {
            if (!_eventArgsCount.ContainsKey(_eventID))
            {
                return 0;
            }

            return _eventArgsCount[_eventID];
        }

        void _addOrUpdateEventArgsCountItem(int id)
        {
            if (!_eventArgsCount.ContainsKey(id))
            {
                _eventArgsCount.Add(id,_argCount);
            }
            else
            {
                if(!Equals(_eventArgsCount[id], _argCount))
                {
                    _eventArgsCount[id] = _argCount;
                }
            }
        }
        
        private void _handle(object sender, GameEventArgs e)
        {
            _args = ((BoltEventArgs)e).Args;

            _flow.Invoke(_triggerExit);
        }
    }
}