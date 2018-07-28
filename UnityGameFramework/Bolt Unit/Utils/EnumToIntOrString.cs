//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月28日-06:15
//Icarus.UnityGameFramework.Bolt

using System;
using Bolt;
using Ludiq;

namespace Icarus.UnityGameFramework.Bolt.Units
{
    [UnitCategory("Icarus/Util")]
    [UnitTitle("枚举转Int或String")]
    [UnitSubtitle("To Int 为false为string")]
    public class EnumToIntOrString: GameFrameWorkBaseUnit
    {
        [Serialize]
        [Inspectable, UnitHeaderInspectable("To Int:")]
        public bool _isInt;

        [DoNotSerialize]
        [PortLabel("Value")]
        public ValueInput _value;

        [DoNotSerialize]
        [PortLabel("结果")]
        public ValueOutput _result;

        protected override void Definition()
        {
            base.Definition();
            _value = ValueInput<Enum>(nameof(_value));

            if(_isInt)
            {
                _result = ValueOutput<int>(nameof(_result));
            }
            else
            {
                _result = ValueOutput<string>(nameof(_result));
            }

            Assignment(_enter,_result);
            Requirement(_value,_enter);
        }

        protected override ControlOutput Enter(Flow flow)
        {
            var value = flow.GetValue<Enum>(_value);
            if (_isInt)
            {
                flow.SetValue(_result, Convert.ToInt32(value));
            }
            else
            {
                flow.SetValue(_result, value.ToString());
            }
            return _exit;
        }
    }

}