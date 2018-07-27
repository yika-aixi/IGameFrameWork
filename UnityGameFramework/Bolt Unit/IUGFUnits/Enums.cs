//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月24日-10:53
//Icarus.UnityGameFramework.Runtime

namespace Icarus.UnityGameFramework.Bolt
{
    public enum AssetManagerCallType
    {
        初始化,
        加载单个资源,
        加载多个资源,
        加载场景,
        卸载资源,
        卸载场景,
        判断资源是否存在,
        获取资源包资源列表,
        获取资源组资源包列表,
        获取所有资源组,
        强制释放未被使用资源,
        预订执行释放未被使用资源
    }

    public enum VersionCheckCallType
    {
        检查更新,
        获取服务器版本信息,
        获取本地最新版本信息,
        获取持久化目录版本信息,
        判断资源组是否需要更新,
        获取资源组更新列表
    }

    public enum EventCallType
    {
        单次事件,
        注册事件,
        释放事件,
        触发事件
    }
}