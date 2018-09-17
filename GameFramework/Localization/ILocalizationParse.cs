//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2018年09月16日-05:31
//Icarus.GameFramework

using System.Collections.Generic;

namespace Icarus.GameFramework.Localization
{
    public interface ILocalizationParse
    {
        bool Parse(string text, Dictionary<string, string> outDict,object userData);
    }
}