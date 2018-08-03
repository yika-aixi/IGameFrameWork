//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using SysAssembly = System.Reflection.Assembly;
namespace Icarus.GameFramework
{
    public static partial class Utility
    {
        /// <summary>
        /// 程序集相关的实用函数。
        /// </summary>
        public static class Assembly
        {
            private static readonly SysAssembly[] s_Assemblies = null;
            private static readonly Dictionary<string, Type> s_CachedTypes = new Dictionary<string, Type>();

            static Assembly()
            {
                s_Assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            /// <summary>
            /// 获取已加载的程序集。
            /// </summary>
            /// <returns>已加载的程序集。</returns>
            public static SysAssembly[] GetAssemblies()
            {
                return s_Assemblies;
            }

            /// <summary>
            /// 获取已加载的程序集中的所有类型。
            /// </summary>
            /// <returns>已加载的程序集中的所有类型。</returns>
            public static Type[] GetTypes()
            {
                List<Type> allTypes = new List<Type>();
                for (int i = 0; i < s_Assemblies.Length; i++)
                {
                    allTypes.AddRange(s_Assemblies[i].GetTypes());
                }

                return allTypes.ToArray();
            }

            /// <summary>
            /// 获取已加载的程序集中的指定类型。
            /// </summary>
            /// <param name="typeName">要获取的类型名。</param>
            /// <returns>已加载的程序集中的指定类型。</returns>
            public static Type GetType(string typeName)
            {
                if (string.IsNullOrEmpty(typeName))
                {
                    throw new GameFrameworkException("Type name is invalid.");
                }

                Type type = null;
                if (s_CachedTypes.TryGetValue(typeName, out type))
                {
                    return type;
                }

                type = Type.GetType(typeName);
                if (type != null)
                {
                    s_CachedTypes.Add(typeName, type);
                    return type;
                }

                foreach (System.Reflection.Assembly assembly in s_Assemblies)
                {
                    type = Type.GetType(string.Format("{0}, {1}", typeName, assembly.FullName));
                    if (type != null)
                    {
                        s_CachedTypes.Add(typeName, type);
                        return type;
                    }
                }

                return null;
            }

            /// <summary>
            /// 获取所有运行时Type
            /// </summary>
            /// <returns></returns>
            public static Type[] GetRuntimeType()
            {
                List<Type> runtimeTypes = new List<Type>();
                foreach (var assembly in s_Assemblies)
                {
                    if (IsRuntimeAssembly(assembly))
                    {
                        runtimeTypes.AddRange(assembly.GetTypes());
                    }
                }

                return runtimeTypes.ToArray();
            }

            private static bool IsRuntimeAssembly(SysAssembly assembly)
            {
                // User assemblies refer to the editor when they include
                // a using UnityEditor / #if UNITY_EDITOR, but they should still
                // be considered runtime.
                return IsUserAssembly(assembly) || !IsEditorDependentAssembly(assembly);
            }

            /// <summary>
            /// 判断程序集是否是用户自己的
            /// </summary>
            /// <param name="assembly"></param>
            /// <returns></returns>
            public static bool IsUserAssembly(SysAssembly assembly)
            {
                return IsUserAssembly(assembly.GetName().Name);
            }

            /// <summary>
            /// 判断程序集是否是用户自己的
            /// </summary>
            /// <param name="assemblyName"></param>
            /// <returns></returns>
            public static bool IsUserAssembly(string assemblyName)
            {
                return
                    assemblyName == "Assembly-CSharp" ||
                    assemblyName == "Assembly-CSharp-firstpass";
            }

            /// <summary>
            /// 判断程序集是否是Editor的
            /// </summary>
            /// <param name="assembly"></param>
            /// <returns></returns>
            public static bool IsEditorDependentAssembly(SysAssembly assembly)
            {
                if (IsEditorAssembly(assembly))
                {
                    return true;
                }

                foreach (var dependency in assembly.GetReferencedAssemblies())
                {
                    if (IsEditorAssembly(dependency))
                    {
                        return true;
                    }
                }

                return false;
            }

            private static bool IsEditorAssembly(SysAssembly assembly)
            {
                if (Attribute.IsDefined(assembly, typeof(AssemblyIsEditorAssembly)))
                {
                    return true;
                }

                return IsEditorAssembly(assembly.GetName());
            }

            private static bool IsEditorAssembly(AssemblyName assemblyName)
            {
                var name = assemblyName.Name;

                return
                    name == "Assembly-CSharp-Editor" ||
                    name == "Assembly-CSharp-Editor-firstpass" ||
                    name == "UnityEditor";
            }


        }
    }
}
