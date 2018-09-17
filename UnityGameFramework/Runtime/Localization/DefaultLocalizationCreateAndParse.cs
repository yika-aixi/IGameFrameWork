//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2018年09月16日-12:03
//Icarus.GameFramework

using System.Collections.Generic;
using System.IO;
using System.Text;
using Icarus.GameFramework;
using IGameFrameWork.UnityGameFramework.Runtime;
using UnityEngine;

namespace Icarus.UnityGameFramework.Runtime
{
    public class DefaultLocalizationCreateAndParse : LocalizationCreateAndParseBase
    {
        public static readonly char ColumnSplit = '\t';
        public const int ColumnCount = 2;

        public override string ExtensionName { get; } = "txt";

        public override string CreateStr(Dictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pair in dict)
            {
                sb.AppendLine($"{pair.Key}{ColumnSplit}{pair.Value}");
            }

            return sb.ToString();
        }

        public override void CreateFile(Dictionary<string, string> dict, string path)
        {
            var str = CreateStr(dict);
            File.WriteAllText($"{path}.{ExtensionName}", str);
        }

        public override bool Parse(string text, Dictionary<string, string> outDict, object userData)
        {
            outDict?.Clear();
            string[] rowTexts = GameFramework.Utility.Text.SplitToLines(text);
            for (var i = 0; i < rowTexts.Length; i++)
            {
                var csv = rowTexts[i].Split(DefaultLocalizationCreateAndParse.ColumnSplit);
                string key = csv[0];
                string value = csv[1];

                if (outDict == null) continue;

                if (outDict.ContainsKey(key))
                {
#if UNITY_EDITOR
                    Debug.LogWarning("已存在相同的字典:" + key);
#else
                    Log.Warning("已存在相同的字典:"+key);
#endif
                    return false;
                }

                outDict.Add(key, value);
            }

            return true;
        }
    }
}