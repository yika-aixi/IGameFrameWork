//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2018年09月16日-12:01
//Icarus.GameFramework

using System;
using System.Collections.Generic;

namespace Icarus.GameFramework.Localization
{
    public interface ILocalizationCreate
    {
        string ExtensionName { get; }

        /// <summary>
        /// 本地语言文件创建
        /// </summary>
        /// <param name="dict">语言字典</param>
        /// <returns>文件内容</returns>
        string CreateStr(Dictionary<string, string> dict);

        /// <summary>
        /// 本地语言文件创建
        /// </summary>
        /// <param name="dict">语言字典</param>
        /// <param name="path">保存路径</param>
        void CreateFile(Dictionary<string, string> dict, string path);
    }
}