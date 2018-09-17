//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2018年09月16日-05:37
//Icarus.UnityGameFramework.Runtime

using System.Collections.Generic;
using Icarus.GameFramework.Localization;

namespace IGameFrameWork.UnityGameFramework.Runtime
{
    public abstract class LocalizationCreateAndParseBase:ILocalizationCreate,ILocalizationParse
    {
        public abstract string ExtensionName { get; }
        public abstract string CreateStr(Dictionary<string, string> dict);

        public abstract void CreateFile(Dictionary<string, string> dict, string path);

        public abstract bool Parse(string text, Dictionary<string, string> outDict, object userData);
    }
}