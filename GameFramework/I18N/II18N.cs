using System;
using System.Collections.Generic;

namespace Icarus.GameFramework.I18N
{
    public interface II18N
    {
        /// <summary>
        /// 语言更改事件。
        /// </summary>
        event EventHandler<LanguageChangeEventArgs> LanguageChange;

        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="language"></param>
        void SetCurrentLanguage(string language);

        /// <summary>
        /// 添加语言表
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="table">语言表</param>
        void AddLanguageTable(string language, Dictionary<string, string> table);

        /// <summary>
        /// 获取key的value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetValue(string key);

        /// <summary>
        /// 获取指定语言key的value
        /// </summary>
        /// <param name="languageName">语言名字</param>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetValue(string languageName,string key);

        /// <summary>
        /// 获取当前支持的语言
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetLanguges();

        /// <summary>
        /// 当前语言
        /// </summary>
        string CurrentLanguage { get; }
    }
}