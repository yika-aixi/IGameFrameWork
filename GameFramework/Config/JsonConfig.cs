//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月20日-12:54
//Icarus.GameFramework


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IGameFrameWork.GameFramework.Config
{
    /// <summary>
    /// json配置
    /// </summary>
    public class JsonConfig<T> : ConfigLoad, IJsonConfigParse<T>
    {
        public T Parse(string config)
        {
            return JsonConvert.DeserializeObject<T>(config);
        }

        public JObject ParseJObject(string config)
        {
            return JObject.Parse(config);
        }
    }
}