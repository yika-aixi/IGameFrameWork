//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月09日-10:57
//Icarus.UnityGameFramework.Editor

using System;
using System.IO;
using UnityEditor;
using YamlDotNet.RepresentationModel;

namespace Icarus.UnityGameFramework.Bolt.Event
{
    public static class oldEventTableUpdateTool
    {
        private static string NullStr = "Null";

        public static bool Update(string assetPath, SerializedObject target)
        {
            if (target == null)
            {
                return false;
            }
            
            if (!IsCanUpdate(assetPath))
            {
                return false;
            }

            var _events = target.FindProperty("_events");

            int i = 0;
            int j = 0;

            int currentIndex = 0;
            _events.arraySize = _eventNames.Children.Count;
            foreach (YamlScalarNode eventName in _eventNames)
            {
                var @event = _events.GetArrayElementAtIndex(i);
                @event.FindPropertyRelative("_eventName").stringValue = eventName.Value;
                @event.FindPropertyRelative("_eventId").intValue = i + 1;
                int count = 0;
                for (; j < _eventArgNames.Children.Count; j++)
                {
                    YamlScalarNode argName = (YamlScalarNode)_eventArgNames[j];
                    if (argName.Value != NullStr)
                    {
                        count++;
                    }
                    else
                    {
                        j++;
                        break;
                    }
                }

                var args = @event.FindPropertyRelative("_args");
                args.arraySize = count;

                for (var k = 0; k < args.arraySize; k++, currentIndex++)
                {
                    var argNameNode = ((YamlScalarNode)_eventArgNames[currentIndex]);
                    var argTypeNode = ((YamlScalarNode)_eventArgTypeNames[currentIndex]);
                    var arg = args.GetArrayElementAtIndex(k);
                    arg.FindPropertyRelative("_argName").stringValue = argNameNode.Value;
                    arg.FindPropertyRelative("_argTypeStr").stringValue = argTypeNode.Value;
                }
                currentIndex++;
                i++;
            }

            target.ApplyModifiedProperties();
            target.Update();

            return true;
        }

        /// <summary>
        /// 判断该事件表是否支持更新
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static bool IsCanUpdate(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }
            
            _serializerYAML(assetPath);

            try
            {
                if (_eventNames == null ||
                    _eventArgNames == null ||
                    _eventArgTypeNames == null) { }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static YamlStream _yaml;
        private static YamlDocument _document => _yaml.Documents[0];
        private static YamlNode _root => _document.RootNode;
        private static YamlNode _base => _root["MonoBehaviour"];
        private static YamlSequenceNode _eventNames => (YamlSequenceNode)_base["_eventNames"];
        private static YamlSequenceNode _eventIds => (YamlSequenceNode)_base["_eventIds"];
        private static YamlSequenceNode _eventArgCount => (YamlSequenceNode)_base["_eventArgCount"];
        private static YamlSequenceNode _eventArgNames => (YamlSequenceNode)_base["_eventArgNames"];
        private static YamlSequenceNode _eventArgTypeNames => (YamlSequenceNode)_base["_eventArgTypeNames"];

        private static void _serializerYAML(string assetPath)
        {
            var assetContent = File.ReadAllText(assetPath);
            var input = new StringReader(assetContent);
            _yaml = new YamlStream();
            _yaml.Load(input);
        }
    }
}