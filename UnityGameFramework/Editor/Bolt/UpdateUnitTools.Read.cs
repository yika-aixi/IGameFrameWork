//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月15日-05:21
//Icarus.UnityGameFramework.Editor

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using MiscUtil.IO;
using UnityEditor;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace IGameFrameWork.UnityGameFramework.Editor.Bolt
{

    public partial class UpdateUnitTools
    {
        void _forFilesPaths()
        {
            int count = 0;
            int maxCount = filesPaths.Count;

            try
            {
                foreach (var filesPath in filesPaths)
                {
                    EditorUtility.DisplayProgressBar(
                        "Update Flow Asset",
                        $"Update Flow File Name:{Path.GetFileName(filesPath)}  {count}-{maxCount}",
                        count / (float)maxCount);

                    var assetContent = File.ReadAllText(filesPath);
                    _getHead(assetContent);
                    var json = _serializerYAMLAndGetJsonData(assetContent);
                    var newJson = _jsonHadnle(json);
                    _updateYamlJsonData1(filesPath, json, newJson);
                    count++;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Update Failure. Error:{e.Message}\n Stack{e.StackTrace}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

        }

        private string _jsonHadnle(string json)
        {
            string newJson;

            if (_isDelete)
            {
                _parseJson(json);
                newJson = _jObject.ToString(Formatting.None);
            }
            else
            {
                newJson = _replaceNameSpaces(json);
            }

            return newJson;
        }

        private YamlStream _yaml;
        private YamlScalarNode _jsonNode;
        private const string RootName = "MonoBehaviour";
        private string _head;
        private string _serializerYAMLAndGetJsonData(string assetContent)
        {
            var input = new StringReader(assetContent);
            _yaml = new YamlStream();
            _yaml.Load(input);

            _jsonNode = (YamlScalarNode)_yaml.Documents[0].RootNode["MonoBehaviour"]["_data"]["_json"];
            return _jsonNode.ToString();
        }

        private void _getHead(string assetContent)
        {
            if (string.IsNullOrWhiteSpace(assetContent))
            {
                throw new Exception("Asset is null or WhiteSpace");
            }

            var index = assetContent.IndexOf(RootName);

            if (index == -1)
            {
                throw new Exception($"error no find Root {RootName}");
            }

            _head = assetContent.Remove(index);
        }

        private const string BackName = "_Back";
        private void _updateYamlJsonData1(string filePath, string oldJson, string newJson)
        {
            if (oldJson == newJson)
            {
                return;
            }

            _jsonNode.Value = newJson;
            
            TextWriter writer = new StringWriterWithEncoding(Encoding.UTF8);

            _yaml.Save(writer, false);

            var yaml = writer.ToString();

            writer.Close();
            writer.Dispose();
            StringBuilder sb = new StringBuilder();
            using (TextReader reader = new StringReader(yaml))
            {
                int i = 0;
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();

                    //跳过第一行
                    if (i == 0)
                    {
                        i ++;
                        continue;
                    }


                    //跳过结尾
                    if (line.StartsWith("..."))
                    {
                        continue;
                    }

                    sb.Append(line + "\n");
                }

                //删除换行
                sb.Remove(sb.Length - 1, 1);
            }

            var assetContent = _head + sb;

            File.Move(filePath, $"{filePath}{BackName}{DateTime.Now.Ticks}");

            File.WriteAllText(filePath, assetContent);
        }


        private void _updateYamlJsonData(string filePath, string assetContent, string oldJson, string newJson)
        {
            if (oldJson == newJson)
            {
                return;
            }
            File.Move(filePath, $"{filePath}{BackName}{DateTime.Now.Ticks}");
            assetContent = assetContent.Replace(oldJson, newJson);
            File.WriteAllText(filePath, assetContent);
        }

        /// <summary>
        /// 分析资源
        /// </summary>
        /// <param name="json"></param>
        private string _replaceNameSpaces(string json)
        {
            return json.Replace(_oldNameSpace, _newNameSpace);
        }

    }
}