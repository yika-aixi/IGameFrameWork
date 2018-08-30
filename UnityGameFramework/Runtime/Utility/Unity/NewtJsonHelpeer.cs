//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月30日-09:57
//Icarus.UnityGameFramework.Runtime

using System;
using Icarus.GameFramework;
using Newtonsoft.Json;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// JSON 函数集辅助器 -- .NetJson 实现。
    /// </summary>
    public class NewtJsonHelpeer: Utility.Json.IJsonHelper
    {
        public string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T ToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public object ToObject(Type objectType, string json)
        {
            return JsonConvert.DeserializeObject(json, objectType);
        }
    }
}