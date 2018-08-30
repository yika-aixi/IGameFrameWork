//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月30日-11:50
//Icarus.UnityGameFramework.Editor

using Icarus.GameFramework.DataTable;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Icarus.UnityGameFramework.Editor.DataTable
{
    public class CreateDataTableEditor:EditorWindow
    {
        private string[] _dataRowCreateTypes;

        [MenuItem("Icarus/Game Framework/DataTable/Create DataTable File", false, 32)]
        static void _open()
        {
            CreateDataTableEditor table = new CreateDataTableEditor();
            table.titleContent = new GUIContent("数据表创建");
            table.Show();
        }


        private void OnEnable()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var result = assemblies.Where(x => !x.GetName().FullName.Contains("System"))
                .Select(x=>x.GetName().FullName).ToArray();
            _dataRowCreateTypes = Type.GetTypeNames(typeof(IDataRowCreate), result);
            _initFolder();
        }

        private const string _createTableRowKey = "CreateTableRow";
        string _folder;
        private void _initFolder()
        {
            _folder = EditorPrefs.GetString(_createTableRowKey);

            if (!EditorPrefs.HasKey(_createTableRowKey) || string.IsNullOrWhiteSpace(_folder))
            {
                _folder = Application.dataPath;
            }
            
        }

        private int _selectIndex;
        private string _extensionName = "txt";
        private void OnGUI()
        {
            if (_dataRowCreateTypes == null || _dataRowCreateTypes.Length == 0)
            {
                EditorGUILayout.HelpBox($"没有找到数据行创建类型的实现,请增加{typeof(IDataRowCreate).AssemblyQualifiedName}的实现", MessageType.Warning);

                return;
            }

            _selectIndex = EditorGUILayout.Popup(new GUIContent("DataRowType:"), _selectIndex, _dataRowCreateTypes);
            _extensionName = EditorGUILayout.TextField("Extension:", _extensionName);
            if (GUILayout.Button("Create",GUILayout.Height(50)))
            {
                if (_selectIndex < 0)
                {
                    Debug.LogError("创建失败,没有选择需要创建的DataRow");
                    return;
                }
                var folderPath = EditorUtility.OpenFolderPanel("保存路径", _folder,"");

                if (string.IsNullOrEmpty(folderPath))
                {
                    Debug.LogWarning("创建失败,取消了创建");
                    return;
                }

                EditorPrefs.SetString(_createTableRowKey, folderPath);

                System.Type createType = GameFramework.Utility.Assembly.GetType(_dataRowCreateTypes[_selectIndex]);
                var create = (IDataRowCreate)Activator.CreateInstance(createType);
                var fileName = _getFileName(create);
                File.WriteAllText(Path.Combine(folderPath,$"{fileName}.{_extensionName}"), create.Create());
                AssetDatabase.Refresh();
            }
        }

        private string _getFileName(IDataRowCreate create)
        {
            //删除DR
            return create.GetType().Name.Remove(0,2);
        }
    }
}