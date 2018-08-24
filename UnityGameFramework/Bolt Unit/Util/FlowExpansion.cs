//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月21日-10:14
//Icarus.UnityGameFramework.Bolt

using Bolt;
using Icarus.GameFramework;
using Ludiq;

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

        public static void EnterControlAndDisplay(this Flow flow, ControlOutput control)
        {
            flow.EnterControl(control);
            flow.Dispose();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="flow"></param>
        /// <exception cref="GameFrameworkException">Flow  或 GraphStack 是空的</exception>
        /// <returns></returns>
        public static Flow GetNewFlow(this Flow flow)
        {
            if (flow == null)
            {
                throw new GameFrameworkException("Flow is null, Get New Flow Failure!");
            }

            if (flow.stack == null)
            {
                throw new GameFrameworkException("Flow of GraphStack is null, Get New Flow Failure!");
            }

            return GetFlow(flow.stack.AsReference());
        }


        public static Flow GetFlow(GraphReference reference)
        {
            return Flow.New(reference);
        }
    }
}