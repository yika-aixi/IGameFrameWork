//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月20日-12:52
//Icarus.GameFramework

namespace IGameFrameWork.GameFramework.Config
{
    /// <summary>
    /// Json解析
    /// </summary>
    public interface IJsonConfigParse<T>
    {
        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        T Parse(string config);
    }
}