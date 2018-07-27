//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月27日-07:00
//Icarus.UnityGameFramework.Bolt

using Icarus.GameFramework.Event;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    public class BoltEventArgs: GameEventArgs
    {
        private int _id;

        public override int Id => _id;

        public BoltEventArgs()
        {
            _id = typeof(BoltEventArgs).GetHashCode();
        }

        public BoltEventArgs(int id)
        {
            _id = id;
        }

        public object[] Args { get; private set; }

        public void SetArgsPar(params object[] args)
        {
            Args = args;
        }

        public void SetArgs(object[] args)
        {
            Args = args;
        }


        public override void Clear()
        {
            Args = null;
        }
    }
}