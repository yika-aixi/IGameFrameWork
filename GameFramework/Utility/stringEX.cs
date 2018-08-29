//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月30日-01:07
//Icarus.Chess

using System.Text.RegularExpressions;

namespace Icarus.GameFramework
{
    public static class StringEx
    {

        public static string EscapeReplace(this string str)
        {
            //暂时用正则
            return Regex.Unescape(str);
        }
    }
}