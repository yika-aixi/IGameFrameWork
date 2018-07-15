//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月15日-03:16
//Icarus.UnityGameFramework.Editor

using System.Collections.Generic;
using System.IO;
using Bolt;
using Ludiq;
using UnityEditor;
using UnityEngine;

namespace IGameFrameWork.UnityGameFramework.Editor.Bolt
{
    public partial class UpdateUnitTools:EditorWindow
    {
        [MenuItem("Icarus/Util/Bolt/更新Unit")]
        static void _open()
        {
            var window = GetWindow<UpdateUnitTools>("Unit Update");
            window.minSize = new Vector2(600f, 110f);
        }

        private string _oldNameSpace;
        private string _newNameSpace;
        private bool _isDelete;
        void OnGUI()
        {
            EditorGUIUtility.labelWidth = 200;
            _createTextField(ref _oldNameSpace,"old Or Delete NameSpace.name:");
            EditorGUIUtility.labelWidth = 0;
            _createTextField(ref _newNameSpace,"new NameSpace.name:");
            _isDelete = EditorGUILayout.Toggle("delete", _isDelete);

            if (GUILayout.Button("Start Update",GUILayout.Height(50)))
            {
                if (!_selectFolder())
                {
                    return;
                }
                _startUpdate();

                FlowGraph.WithStartUpdate();
            }
        }

        List<string> filesPaths = new List<string>();
        private void _startUpdate()
        {
            filesPaths.Clear();
            filesPaths.AddRange(Directory.GetFiles(_filePath, "*.asset",SearchOption.AllDirectories));
            _forFilesPaths();

            AssetDatabase.Refresh();

            Debug.Log($"All {(_isDelete ? "Delete" : "Update")} Complete.");
        }

        private string _filePath;
        private string _filePathKey;
        private bool _selectFolder()
        {
            _checkPath();

            _filePath = EditorUtility.OpenFolderPanel("Select Units Folder", _filePath, "");
            if (string.IsNullOrEmpty(_filePath))
            {
                return false;
            }
            _updateEditorPrefsString(_filePathKey, _filePath);
            return true;
        }

        private void _updateEditorPrefsString(string key, string value)
        {
            EditorPrefs.SetString(key,value);
        }

        private void _checkPath()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                _filePath = EditorPrefs.GetString(_filePathKey);
            }

            if (string.IsNullOrEmpty(_filePath))
            {
                _filePath = Application.dataPath;
            }
        }

        private static void _createTextField(ref string vlaue,string lable)
        {
            vlaue = EditorGUILayout.TextField(lable, vlaue);
        }
    }

    
}