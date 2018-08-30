//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月17日-02:31
//Icarus.UnityGameFramework.Runtime

using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    public class GameFrameworkComponent:MonoBehaviour
    {
        protected virtual void Awake()
        {
            GameEntry.RegisterComponent(this);
        }
    }
}