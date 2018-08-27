//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using Icarus.UnityGameFramework.Runtime;
using UnityEngine;

namespace Icarus.UnityGameFramework.Editor
{
    [CustomEditor(typeof(ResourceComponent))]
    internal sealed class ResourceComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty m_ReadWritePathType = null;
        private SerializedProperty m_UnloadUnusedAssetsInterval = null;
        private SerializedProperty m_AssetAutoReleaseInterval = null;
        private SerializedProperty m_AssetCapacity = null;
        private SerializedProperty m_AssetExpireTime = null;
        private SerializedProperty m_AssetPriority = null;
        private SerializedProperty m_ResourceAutoReleaseInterval = null;
        private SerializedProperty m_ResourceCapacity = null;
        private SerializedProperty m_ResourceExpireTime = null;
        private SerializedProperty m_ResourcePriority = null;
        private SerializedProperty m_InstanceRoot = null;
        private SerializedProperty m_LoadResourceAgentHelperCount = null;

        private FieldInfo m_EditorResourceModeFieldInfo = null;

        private HelperInfo<ResourceHelperBase> m_ResourceHelperInfo = new HelperInfo<ResourceHelperBase>("Resource");
        private HelperInfo<LoadResourceAgentHelperBase> m_LoadResourceAgentHelperInfo = new HelperInfo<LoadResourceAgentHelperBase>("LoadResourceAgent");
        private bool isEditorResourceMode;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            ResourceComponent t = (ResourceComponent)target;

            isEditorResourceMode = (bool)m_EditorResourceModeFieldInfo.GetValue(target);

            if (isEditorResourceMode)
            {
                EditorGUILayout.HelpBox("Editor resource mode is enabled. Some options are disabled.", MessageType.Warning);
            }
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_baseComponent);
            EditorGUILayout.PropertyField(_eventComponent);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                //                if (EditorApplication.isPlaying && PrefabUtility.GetPrefabType(t.gameObject) != PrefabType.Prefab)
                //                {
                //                    EditorGUILayout.EnumPopup("Resource Mode", t.ResourceMode);
                //                }
                //                else
                //                {
                //                    int selectedIndex = EditorGUILayout.Popup("Resource Mode", m_ResourceModeIndex, m_ResourceModeNames);
                //                    if (selectedIndex != m_ResourceModeIndex)
                //                    {
                //                        m_ResourceModeIndex = selectedIndex;
                //                        m_ResourceMode.enumValueIndex = selectedIndex + 1;
                //                    }
                //                }

                m_ReadWritePathType.enumValueIndex = (int)(ReadWritePathType)EditorGUILayout.EnumPopup("Read Write Path Type", t.ReadWritePathType);
            }
            EditorGUI.EndDisabledGroup();

            float unloadUnusedAssetsInterval = EditorGUILayout.Slider("Unload Unused Assets Interval", m_UnloadUnusedAssetsInterval.floatValue, 0f, 3600f);
            if (unloadUnusedAssetsInterval != m_UnloadUnusedAssetsInterval.floatValue)
            {
                if (EditorApplication.isPlaying)
                {
                    t.UnloadUnusedAssetsInterval = unloadUnusedAssetsInterval;
                }
                else
                {
                    m_UnloadUnusedAssetsInterval.floatValue = unloadUnusedAssetsInterval;
                }
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying && isEditorResourceMode);
            {
                float assetAutoReleaseInterval = EditorGUILayout.DelayedFloatField("Asset Auto Release Interval", m_AssetAutoReleaseInterval.floatValue);
                if (assetAutoReleaseInterval != m_AssetAutoReleaseInterval.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetAutoReleaseInterval = assetAutoReleaseInterval;
                    }
                    else
                    {
                        m_AssetAutoReleaseInterval.floatValue = assetAutoReleaseInterval;
                    }
                }

                int assetCapacity = EditorGUILayout.DelayedIntField("Asset Capacity", m_AssetCapacity.intValue);
                if (assetCapacity != m_AssetCapacity.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetCapacity = assetCapacity;
                    }
                    else
                    {
                        m_AssetCapacity.intValue = assetCapacity;
                    }
                }

                float assetExpireTime = EditorGUILayout.DelayedFloatField("Asset Expire Time", m_AssetExpireTime.floatValue);
                if (assetExpireTime != m_AssetExpireTime.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetExpireTime = assetExpireTime;
                    }
                    else
                    {
                        m_AssetExpireTime.floatValue = assetExpireTime;
                    }
                }

                int assetPriority = EditorGUILayout.DelayedIntField("Asset Priority", m_AssetPriority.intValue);
                if (assetPriority != m_AssetPriority.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetPriority = assetPriority;
                    }
                    else
                    {
                        m_AssetPriority.intValue = assetPriority;
                    }
                }

                float resourceAutoReleaseInterval = EditorGUILayout.DelayedFloatField("Resource Auto Release Interval", m_ResourceAutoReleaseInterval.floatValue);
                if (resourceAutoReleaseInterval != m_ResourceAutoReleaseInterval.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.ResourceAutoReleaseInterval = resourceAutoReleaseInterval;
                    }
                    else
                    {
                        m_ResourceAutoReleaseInterval.floatValue = resourceAutoReleaseInterval;
                    }
                }

                int resourceCapacity = EditorGUILayout.DelayedIntField("Resource Capacity", m_ResourceCapacity.intValue);
                if (resourceCapacity != m_ResourceCapacity.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.ResourceCapacity = resourceCapacity;
                    }
                    else
                    {
                        m_ResourceCapacity.intValue = resourceCapacity;
                    }
                }

                float resourceExpireTime = EditorGUILayout.DelayedFloatField("Resource Expire Time", m_ResourceExpireTime.floatValue);
                if (resourceExpireTime != m_ResourceExpireTime.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.ResourceExpireTime = resourceExpireTime;
                    }
                    else
                    {
                        m_ResourceExpireTime.floatValue = resourceExpireTime;
                    }
                }

                int resourcePriority = EditorGUILayout.DelayedIntField("Resource Priority", m_ResourcePriority.intValue);
                if (resourcePriority != m_ResourcePriority.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.ResourcePriority = resourcePriority;
                    }
                    else
                    {
                        m_ResourcePriority.intValue = resourcePriority;
                    }
                }


            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_InstanceRoot);

                m_ResourceHelperInfo.Draw();
                m_LoadResourceAgentHelperInfo.Draw();
                m_LoadResourceAgentHelperCount.intValue = EditorGUILayout.IntSlider("Load Resource Agent Helper Count", m_LoadResourceAgentHelperCount.intValue, 1, 64);
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && PrefabUtility.GetPrefabType(t.gameObject) != PrefabType.Prefab)
            {
                EditorGUILayout.LabelField("Read Only Path", t.ReadOnlyPath.ToString());
                EditorGUILayout.LabelField("Read Write Path", t.ReadWritePath.ToString());
                EditorGUILayout.LabelField("Current Variant", t.CurrentVariant ?? "<Unknwon>");
                EditorGUILayout.LabelField("Applicable Game Version", isEditorResourceMode ? "N/A" : t.ApplicableGameVersion ?? "<Unknwon>");
                EditorGUILayout.LabelField("Internal Resource Version", isEditorResourceMode ? "N/A" : t.InternalResourceVersion.ToString());
                EditorGUILayout.LabelField("Asset Count", isEditorResourceMode ? "N/A" : t.AssetCount.ToString());
                EditorGUILayout.LabelField("Resource Count", isEditorResourceMode ? "N/A" : t.ResourceCount.ToString());

                EditorGUILayout.LabelField("Load Total Agent Count", isEditorResourceMode ? "N/A" : t.LoadTotalAgentCount.ToString());
                EditorGUILayout.LabelField("Load Free Agent Count", isEditorResourceMode ? "N/A" : t.LoadFreeAgentCount.ToString());
                EditorGUILayout.LabelField("Load Working Agent Count", isEditorResourceMode ? "N/A" : t.LoadWorkingAgentCount.ToString());
                EditorGUILayout.LabelField("Load Waiting Task Count", isEditorResourceMode ? "N/A" : t.LoadWaitingTaskCount.ToString());

                _showResourceGroup(t);
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }
        private Dictionary<string, bool> _showState = new Dictionary<string, bool>();
        private void _showResourceGroup(ResourceComponent resourceComponent)
        {
            if (isEditorResourceMode)
            {
                return;
            }

            var groups = resourceComponent.GetAllGroupList();

            if (groups == null)
            {
                return;
            }

            foreach (var @group in groups)
            {
                if (!_showState.ContainsKey(@group))
                {
                    _showState.Add(@group, false);
                }
                _showState[@group] = EditorGUILayout.Foldout(_showState[@group],
                    $"资源组:{(string.IsNullOrEmpty(@group) ? "<none>" : @group)}", true);
                if (_showState[@group])
                {
                    EditorGUI.indentLevel += 1;
                    {
                        var abList = resourceComponent.GetAssetGroupList(@group);
                        if (abList == null)
                        {
                            return;
                        }
                        foreach (var ab in abList)
                        {
                            var key = $"{@group}{ab}";
                            if (!_showState.ContainsKey(key))
                            {
                                _showState.Add(key, false);
                            }
                            var assets = resourceComponent.GetAssetsList(ab);
                            var assetCount = 0;

                            if (assets != null)
                            {
                                assetCount = assets.Count();
                            }

                            _showState[key] = EditorGUILayout.Foldout(_showState[key], $"资源包:{ab},资源个数:{assetCount}", true);

                            if (_showState[key])
                            {
                                if (assets == null)
                                {
                                    return;
                                }

                                foreach (var asset in assets)
                                {
                                    EditorGUI.indentLevel += 1;
                                    {
                                        EditorGUILayout.LabelField($"资源:{asset}");
                                    }
                                    EditorGUI.indentLevel -= 1;
                                }

                            }
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }

            }
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }
        private SerializedProperty _baseComponent = null;
        private SerializedProperty _eventComponent = null;
        private void OnEnable()
        {
            _baseComponent = serializedObject.FindProperty("_baseComponent");
            _eventComponent = serializedObject.FindProperty("_eventComponent");

            m_ReadWritePathType = serializedObject.FindProperty("m_ReadWritePathType");
            m_UnloadUnusedAssetsInterval = serializedObject.FindProperty("m_UnloadUnusedAssetsInterval");
            m_AssetAutoReleaseInterval = serializedObject.FindProperty("m_AssetAutoReleaseInterval");
            m_AssetCapacity = serializedObject.FindProperty("m_AssetCapacity");
            m_AssetExpireTime = serializedObject.FindProperty("m_AssetExpireTime");
            m_AssetPriority = serializedObject.FindProperty("m_AssetPriority");
            m_ResourceAutoReleaseInterval = serializedObject.FindProperty("m_ResourceAutoReleaseInterval");
            m_ResourceCapacity = serializedObject.FindProperty("m_ResourceCapacity");
            m_ResourceExpireTime = serializedObject.FindProperty("m_ResourceExpireTime");
            m_ResourcePriority = serializedObject.FindProperty("m_ResourcePriority");
            m_InstanceRoot = serializedObject.FindProperty("m_InstanceRoot");
            m_LoadResourceAgentHelperCount = serializedObject.FindProperty("m_LoadResourceAgentHelperCount");

            m_EditorResourceModeFieldInfo = target.GetType().GetField("m_EditorResourceMode", BindingFlags.NonPublic | BindingFlags.Instance);

            m_ResourceHelperInfo.Init(serializedObject);
            m_LoadResourceAgentHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_ResourceHelperInfo.Refresh();
            m_LoadResourceAgentHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
