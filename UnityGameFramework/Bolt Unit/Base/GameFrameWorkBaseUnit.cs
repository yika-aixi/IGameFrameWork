//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月28日-06:23
//Icarus.UnityGameFramework.Bolt

using Bolt;
using Icarus.GameFramework;
using Icarus.GameFramework.Event;
using Ludiq;

namespace Icarus.UnityGameFramework.Bolt
{
    public abstract class GameFrameWorkBaseUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput _enter;
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput _exit;

        protected override void Definition()
        {
            _enter = ControlInput(nameof(_enter), Enter);
            _exit = ControlOutput(nameof(_exit));
            Succession(_enter,_exit);
        }

        protected abstract ControlOutput Enter(Flow flow);

        protected int GetEventID<T>() where T : GameEventArgs, new()
        {
            var args = ReferencePool.Acquire<T>();
            var id = args.Id;
            ReferencePool.Release(args);
            return id;
        }
    }
}