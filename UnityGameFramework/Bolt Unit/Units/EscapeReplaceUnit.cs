//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月30日-01:05
//Icarus.Chess.Bolt

using Bolt;
using Icarus.GameFramework;
using Ludiq;

namespace Icarus.UnityGameFramework.Bolt.Units
{
    [UnitCategory("Icarus/Util")]
    [UnitTitle("转义符转换")]
    public class EscapeReplaceUnit:GameFrameWorkBaseUnit
    {
        [DoNotSerialize]
        [PortLabel("Str")]
        public ValueInput _str;

        [DoNotSerialize]
        [PortLabel("New Str")]
        public ValueOutput _result;


        protected override void Definition()
        {
            base.Definition();
            _str = ValueInput<string>(nameof(_str));
            _result = ValueOutput<string>(nameof(_result));

            Requirement(_str,_enter);

            Assignment(_enter,_result);
        }

        protected override ControlOutput Enter(Flow flow)
        {
            flow.SetValue(_result,flow.GetValue<string>(_str).EscapeReplace());

            return _exit;
        }
    }
}