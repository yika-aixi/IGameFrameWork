//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月15日-05:33
//Icarus.UnityGameFramework.Editor

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace IGameFrameWork.UnityGameFramework.Editor.Bolt
{
    public partial class UpdateUnitTools
    {
        private const string RootKey = "graph";
        private const string ElementsKey = "elements";
        private JObject _jObject;
        private JToken _root;
        private JArray _elements;
        private void _parseJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new Exception($"Json Error. json is null or WhiteSpace.");
            }

            try
            {
                _jObject = JObject.Parse(json);
                _root = _jObject[RootKey];
                var arrayJson = _root[ElementsKey].ToString();
                _elements = JArray.Parse(arrayJson);
            }
            catch (Exception ex)
            {
                throw new Exception($"Json Error:{ex}\n Json:{json}");               
            }

            _forElements();
            _root[ElementsKey] = _elements;
            _jObject[RootKey] = _root;
        }

        private const string TypeKey1 = "type";
        private const string TypeKey2 = "$type";
        private const string IDKey = "$id";
        private void _forElements()
        {
            if (_elements.Count == 0)
            {
                Debug.Log("error json. elements is null.");
                return;
            }

            for (int i = 0; i < _elements.Count; i++)
            {
                var element = _elements[i];
                var type1 = element[TypeKey1];
                var type2 = element[TypeKey2];
                var type1Str = type1 == null ? "" : type1.Value<string>();
                var type2Str = type2 == null ? "" : type2.Value<string>();

                if (type1Str == _oldNameSpace || type2Str == _oldNameSpace)
                {
                    if (_isDelete)
                    {
                        var idElement = element[IDKey];

                        if (idElement == null)
                        {
                            Debug.Log($"error Json. no ID Element! Delete stop. \n" +
                                      $"{TypeKey1}:{type1}\n" +
                                      $"{TypeKey2}:{type2}");
                            return;
                        }

                        _deleteElement(element, idElement.Value<string>());
                    }
                }
            }
        }
        
        private const string SourceUnitKey = "sourceUnit";
        private const string DestinationUnitKey = "destinationUnit";
        private void _deleteElement(JToken element, string id)
        {
            //删除该元素
            element.Remove();
            Debug.Log($"Delete Id：{id}");
            for (var i = 0; i < _elements.Count; i++)
            {
                var value = _elements[i];
                //得到入口引用
                var sourceUnit = value[SourceUnitKey];
                //得到出口引用
                var destinationUnit = value[DestinationUnitKey];

                _deleteUnitRef(sourceUnit, id);

                _deleteUnitRef(destinationUnit, id);
            }
        }
        private const string RefKey = "$ref";
        private void _deleteUnitRef(JToken unit, string id)
        {
            if (unit == null)
            {
                return;
            }
            //获取引用id
            var refId = unit[RefKey];
            //等于被删除元素的id
            if (refId.Value<string>() == id)
            {
                //删除unit
                unit.Parent.Remove();
            }
        }
    }
}