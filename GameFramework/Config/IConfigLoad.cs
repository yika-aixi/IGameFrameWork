//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月20日-12:48
//Icarus.GameFramework

using Icarus.GameFramework;

namespace IGameFrameWork.GameFramework.Config
{
    /// <summary>
    /// 配置加载
    /// </summary>
    public interface IConfigLoad
    {
        string LoadConfig(string configPath);

        void LoadConfigAsyn(string configPath,GameFrameworkAction<string> loadCompleteHandle = null,GameFrameworkAction<string> loadErrorHandle = null);
    }
}