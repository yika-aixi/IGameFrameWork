using System.Collections;
using UnityEngine;

//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年04月26日 0:00:00
//Assembly-CSharp

namespace Icarus.UnityGameFramework.Runtime
{
    [AddComponentMenu("Icarus/Game Framework/Coroutine Manager")]
    public class CoroutineManager : MonoBehaviour
    {
        
        public Coroutine Start_Coroutine(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        public void Stop_Coroutine(IEnumerator coroutine)
        {
            StopCoroutine(coroutine);
        }
        public void StopAll_Coroutine()
        {
            StopAllCoroutines();
        }
    }
}
