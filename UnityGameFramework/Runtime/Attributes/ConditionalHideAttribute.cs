//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2018年09月16日-12:33
//Icarus.UnityGameFramework.Runtime

using System;
using UnityEngine;

namespace IGameFrameWork.UnityGameFramework.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public string ConditionalSourceField = "";
        public string ConditionalSourceField2 = "";
        public bool HideInInspector = false;
        public bool Inverse = false;

        // Use this for initialization
        public ConditionalHideAttribute(string conditionalSourceField)
        {
            this.ConditionalSourceField = conditionalSourceField;
            this.HideInInspector = false;
            this.Inverse = false;
        }

        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
        {
            this.ConditionalSourceField = conditionalSourceField;
            this.HideInInspector = hideInInspector;
            this.Inverse = false;
        }

        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector, bool inverse)
        {
            this.ConditionalSourceField = conditionalSourceField;
            this.HideInInspector = hideInInspector;
            this.Inverse = inverse;
        }
   
    }

}