//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月20日 20:30:37
//Assembly-CSharp-Editor


using Icarus.UnityGameFramework.Editor.AssetBundleTools;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Icarus.UnityGameFramework.Editor
{
    public class CreateXml
    {
        static string _getXmlPath()
        {
            return Type.GetConfigurationPath<AssetBundleEditorConfigPathAttribute>() ?? Icarus.GameFramework.Utility.Path.GetCombinePath(Application.dataPath, "GameFramework/Configs/AssetBundleEditor.xml");
        }

        private const string XML =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!--文档 http://gameframework.cn/archives/320 -->
<UnityGameFramework>
    <AssetBundleEditor>
        <Settings>
            <SourceAssetRootPath>Assets</SourceAssetRootPath>
            <SourceAssetSearchPaths>
                <SourceAssetSearchPath RelativePath="""" />
            </SourceAssetSearchPaths>
            <SourceAssetUnionTypeFilter>t:Scene t:Prefab t:Shader t:Model t:Material t:Texture t:AudioClip t:AnimationClip t:AnimatorController t:Font t:TextAsset t:ScriptableObject</SourceAssetUnionTypeFilter>
            <SourceAssetUnionLabelFilter>l:AssetBundleInclusive</SourceAssetUnionLabelFilter>
            <SourceAssetExceptTypeFilter>t:Script</SourceAssetExceptTypeFilter>
            <SourceAssetExceptLabelFilter>l:AssetBundleExclusive</SourceAssetExceptLabelFilter>
            <AssetSorter>Name</AssetSorter>
        </Settings>
    </AssetBundleEditor>
</UnityGameFramework>
";
        [MenuItem("Icarus/Game Framework/AssetBundle Tools/Xml 模板生成")]
        public static void CreateDefaultXml()
        {
            var path = _getXmlPath();
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, XML);
            Debug.Log("创建完成.路径:"+path);
            AssetDatabase.Refresh();
        }
    }
}