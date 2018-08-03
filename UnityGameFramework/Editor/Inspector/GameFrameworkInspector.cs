//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Icarus.UnityGameFramework.Editor
{
    /// <summary>
    /// 游戏框架 Inspector 抽象类。
    /// </summary>
    public abstract class GameFrameworkInspector : UnityEditor.Editor
    {
        private bool m_IsCompiling = false;

        /// <summary>
        /// 绘制事件。
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (m_IsCompiling && !EditorApplication.isCompiling)
            {
                m_IsCompiling = false;
                OnCompileComplete();
            }
            else if (!m_IsCompiling && EditorApplication.isCompiling)
            {
                m_IsCompiling = true;
                OnCompileStart();
            }
        }

        /// <summary>
        /// 编译开始事件。
        /// </summary>
        protected virtual void OnCompileStart()
        {

        }

        /// <summary>
        /// 编译完成事件。
        /// </summary>
        protected virtual void OnCompileComplete()
        {

        }

        protected bool IsEnterClick(bool isUsed = true)
        {
            if (!IsKeyClick(KeyCode.Return, isUsed) && !IsKeyClick(KeyCode.KeypadEnter, isUsed)) return false;

            GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
            if (isUsed)
            {
                Event.current.Use();    // Ignore event, otherwise there will be control name conflicts!
            }
            return true;
        }

        protected void OnUpdate()
        {

        }

        protected bool IsEscClick(bool isUsed = true)
        {
            if (!IsKeyClick(KeyCode.Escape, isUsed)) return false;

            GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
            if (isUsed)
            {
                Event.current.Use();    // Ignore event, otherwise there will be control name conflicts!
            }
            return true;
        }

        protected bool IsKeyClick(KeyCode code,bool isUsed = true)
        {
            if (!Event.current.isKey)
                return false;

            if (Event.current.type != EventType.KeyUp)
            {
                if (isUsed)
                {
                    if (Event.current.type != EventType.Used)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return Event.current.keyCode == code;
        }
    }
}
