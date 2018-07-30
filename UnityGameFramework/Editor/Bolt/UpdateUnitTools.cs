//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月15日-03:16
//Icarus.UnityGameFramework.Editor
using Bolt;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Icarus.GameFramework.I18N;
using Ludiq;
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
            window.minSize = new Vector2(600f, 250f);
        }

        [UnityEditor.MenuItem("Icarus/Util/Bolt/Update Flow Graph")]
        static void _updateFlowGraph()
        {
            FlowGraph.WithStartUpdate();
        }
        
        private string _oldNameSpace;
        private string _newNameSpace;
        private bool _isDelete;
        private bool _isStrict = true;
        internal enum HelpType
        {
            en = 0,
            中文 = 1
        }

        private HelpType _helpType = (HelpType) (-1);
        void OnGUI()
        {
            _initHelpType();
            _helpType = (HelpType)EditorGUILayout.EnumPopup("Luanage:", _helpType);
            _updateHelpIndex((int)_helpType);

            EditorGUIUtility.labelWidth = 200;
            _createTextField(ref _oldNameSpace, _getI18NValue(ConstTable.OldNameSpacesKey));
            EditorGUIUtility.labelWidth = 0;
            _createTextField(ref _newNameSpace, _getI18NValue(ConstTable.NewNameSpacesKey));
            _isDelete = EditorGUILayout.Toggle("delete", _isDelete);
            
            if (!_isDelete)
            {
                _isStrict = EditorGUILayout.Toggle(_getI18NValue(ConstTable.StrictKey), _isStrict);
                if (!_isStrict)
                {
                    var message = _getMessage(ConstTable.HelpKey);
                    EditorGUILayout.HelpBox(message, MessageType.Warning);
                }
            }
            
            if (GUILayout.Button(_getI18NValue(ConstTable.SelectDirectoryKey), GUILayout.Height(50)))
            {
                if (!_selectFolder())
                {
                    return;
                }
                _startUpdate(Directory.GetFiles(_filePath, "*.asset", SearchOption.AllDirectories));
            }

            if (GUILayout.Button(_getI18NValue(ConstTable.SelectFileKey), GUILayout.Height(50)))
            {
                var filePaths = _selectFile();

                if (filePaths == null)
                {
                    return;
                }

                _startUpdate(filePaths);

            }
        }

        private readonly I18NManager _i18NManager = new I18NManager();

        void OnEnable()
        {
            _initLanage();
        }

        void _initLanage()
        {
            _i18NManager.AddLanguageTable(HelpType.en.ToString(),new Dictionary<string, string>()
            {
                { ConstTable.HelpKey,"In non-strict mode, all \'Old NameSpace.name\' " +
                                     "in json will be replaced with " +
                                     "\'new NameSpace.name\'. In strict " +
                                     "mode,Currently only the following will be updated: " +
                                     "\"type\", \"$type\", \"targetType\", " +
                                     "\"targetTypeName\",\"_type\", " +
                                     "default to strict mode," +
                                     "If you know an element,You can add the element key to the " +
                                     "StrictModeTable variable in the " +
                                     "\"IGameFrameWork.UnityGameFramework.Editor.Bolt.ConstTable\" class."},
                { ConstTable.OldNameSpacesKey,"old or Delete NameSpace.name:"},
                { ConstTable.NewNameSpacesKey,"new NameSpace.name:"},
                { ConstTable.DeleteKey,"Delete Mode"},
                { ConstTable.StrictKey,"Strict Mode"},
                { ConstTable.SelectDirectoryKey,"Start Directory Update"},
                { ConstTable.SelectFileKey,"Start Select File Update"},
            });

            _i18NManager.AddLanguageTable(HelpType.中文.ToString(), new Dictionary<string, string>()
            {
                { ConstTable.HelpKey,"非严格模式下,将会把json中所有\'Old NameSpace.name\'" +
                                     "替换为\'new NameSpace.name\',严格模式下目前只" +
                                     "会更新以下:\"type\",\"$type\",\"targetType\"," +
                                     "\"targetTypeName\",\"_type\"的value,默认为严格模式,如果你知道某个元素," +
                                     "可以到\"IGameFrameWork.UnityGameFramework.Editor.Bolt.ConstTable类中" +
                                     "将元素key添加到StrictModeTable中\""},

                { ConstTable.OldNameSpacesKey,"需要更新或删除的'nameSpaces.name:'"},
                { ConstTable.NewNameSpacesKey,"新的'nameSpaces.name:'"},
                { ConstTable.DeleteKey,"删除模式"},
                { ConstTable.StrictKey,"严格模式"},
                { ConstTable.SelectDirectoryKey,"选择需要更新得目录"},
                { ConstTable.SelectFileKey,"选择需要更新得文件"},
            });
        }


        private string _getMessage(string key)
        {
            switch (_helpType)
            {
                case HelpType.en:
                case HelpType.中文:
                    return _getI18NValue(key);
                default:
                    return _i18NManager.GetValue(HelpType.en.ToString(), key);
            }
        }

        string _getI18NValue(string  key)
        {
            return _i18NManager.GetValue(_helpType.ToString(), key);
        }

        private string _helpIndexKey = "HelpIndex";
        private void _initHelpType()
        {
            if (_helpType < 0)
            {
                _helpType = (HelpType) EditorPrefs.GetInt(_helpIndexKey);
            }
        }

        private void _updateHelpIndex(int index)
        {
            if (index < 0)
            {
                index = 0;
            }
            
            EditorPrefs.SetInt(_helpIndexKey,index);
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