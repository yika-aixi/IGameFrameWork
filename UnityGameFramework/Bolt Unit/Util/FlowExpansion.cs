//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月21日-10:14
//Icarus.UnityGameFramework.Bolt

using Bolt;
using Icarus.GameFramework;

namespace Icarus.UnityGameFramework.Bolt.Util
{
    public static class FlowExpansion
    {
        public static void EnterControl(this Flow flow, ControlOutput control)
        {
            if (flow != null)
            {
                if (control != null)
                {
                    flow.Invoke(control);
                }
                else
                {
                    Log.Error("Control is null, Enter Control Failure!");
                }
            }
            else
            {
                Log.Error("Flow is null, Enter Control Failure!");
            }
        }
    }
}