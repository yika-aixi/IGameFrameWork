//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月15日-05:21
//Icarus.UnityGameFramework.Editor

using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace IGameFrameWork.UnityGameFramework.Editor.Bolt
{
    public partial class UpdateUnitTools
    {
        void _forFilesPaths()
        {
            foreach (var filesPath in filesPaths)
            {
                var assetContents = File.ReadAllLines(filesPath);
                _analyzeAsset(filesPath,assetContents);
            }
        }

		/// <summary>
        /// json
        /// </summary>
        private const string JsonKey = "_json";
		/// <summary>
        /// 分隔符
        /// </summary>
        private const string SeparatorKey = ": ";

        /// <summary>
        /// 分隔符
        /// </summary>
        private const string @char = "'";
        private const string _re = "(\'.*\')";

        /// <summary>
        /// 分析资源
        /// </summary>
        /// <param name="assetContents"></param>
        private void _analyzeAsset(string filePath,string[] assetContents)
		{
		    foreach (var content in assetContents)
		    {
		        if (content.Contains(JsonKey))
		        {
                    var json = Regex.Match(content, _re).Value;
		            json = json.Replace(@char, "");
		            _parseJson(json);
		            _updateFile(filePath, assetContents);
		        }
		    }
		}

        private void _updateFile(string filePath, string[] assetContents)
        {
            for (var i = 0; i < assetContents.Length; i++)
            {
                var content = assetContents[i];
                if (content.Contains(JsonKey))
                {
                    assetContents[i] = Regex.Replace(assetContents[i], _re, $"'{_jObject.ToString(Formatting.None)}'");
                }
            }

            File.Delete(filePath);
            File.WriteAllLines(filePath, assetContents); 
        }
    }
}