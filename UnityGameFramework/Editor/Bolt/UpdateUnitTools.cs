//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月15日-03:16
//Icarus.UnityGameFramework.Editor
using Bolt;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;


namespace IGameFrameWork.UnityGameFramework.Editor.Bolt
{
    public partial class UpdateUnitTools:EditorWindow
    {
        [UnityEditor.MenuItem("Icarus/Util/Bolt/Update Flow Unit")]
        static void _open()
        {
            var window = GetWindow<UpdateUnitTools>("Unit Update");
            window.minSize = new Vector2(600f, 180f);
        }

        [UnityEditor.MenuItem("Icarus/Util/Bolt/Update Flow Graph")]
        static void _updateFlowGraph()
        {
            FlowGraph.WithStartUpdate();
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
            if (GUILayout.Button("Start Directory Update", GUILayout.Height(50)))
            {
                if (!_selectFolder())
                {
                    return;
                }
                _startUpdate(Directory.GetFiles(_filePath, "*.asset", SearchOption.AllDirectories));
            }

            if (GUILayout.Button("Start Select File Update", GUILayout.Height(50)))
            {
                var filePaths = _selectFile();

                if (filePaths == null)
                {
                    return;
                }

                _startUpdate(filePaths);

            }
        }

        readonly List<string> filesPaths = new List<string>();
        private void _startUpdate(IEnumerable<string> filePath)
        {
            filesPaths.Clear();

            filesPaths.AddRange(filePath);
            _forFilesPaths();

            AssetDatabase.Refresh();
            _updateFlowGraph();
        }

        private string _filePath;
        private const string FilePathKey = "FindFolder";
        private bool _selectFolder()
        {
            _checkPath();

            _filePath = EditorUtility.OpenFolderPanel("Select Units Folder", _filePath, "");
            if (string.IsNullOrEmpty(_filePath))
            {
                return false;
            }
            _updateEditorPrefsString(FilePathKey, _filePath);
            return true;
        }

        private string[] _selectFile()
        {
            _checkPath();
            var filePath = EditorUtility.OpenFilePanel("Select Need Update of Flow Asset", _filePath, "asset");
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }
            return new []{ filePath };
        }

        private void _updateEditorPrefsString(string key, string value)
        {
            EditorPrefs.SetString(key,value);
        }

        private void _checkPath()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                _filePath = EditorPrefs.GetString(FilePathKey);
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