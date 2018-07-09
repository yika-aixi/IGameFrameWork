//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月11日 17:41:44
//Assembly-CSharp

using System.Collections.Generic;

namespace Icarus.GameFramework.Download
{
    public enum DownloadMessageCodeTable
    {
        开始确认远程 = 1001,
        确认结束_重新下载 = 1002,
        确认结束_继续下载 = 1003,
        确认结束_开始下载 = 1004,
        下载中 = 1005,
        下载错误 = 1006,
        下载完成 = 1007,
        无法继续下载 = 1008
    }
    public class DownloadMessageCode
    {
        protected static readonly Dictionary<int, string> CodeMessageTable = new Dictionary<int, string>
        {
            {1001, "开始确认远程文件."},
            {1002, "远程文件确认完成,需要重新下载"},
            {1003, "远程文件确认完成,继续下载"},
            {1004, "远程文件确认完成,下载开始"},
            {1005, "下载中"},
            {1006, "下载出错"},
            {1007, "下载完成"},
            {1008, "无法继续下载,重新将重新下载"}
        };
        public static string GetMessage(int code)
        {
            if (CodeMessageTable.ContainsKey(code))
            {
                return CodeMessageTable[code];
            }
            return null;
        }
    }
}