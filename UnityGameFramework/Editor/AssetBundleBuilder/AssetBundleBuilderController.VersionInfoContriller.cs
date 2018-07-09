//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using Icarus.GameFramework.Version;
using System.IO;
using System.Linq;

namespace Icarus.UnityGameFramework.Editor.AssetBundleTools
{
    internal partial class AssetBundleBuilderController
    {
        private VersionInfo _version;
        void _initVersion(string version)
        {
            _version = new VersionInfo(version);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputPackPath">xx.dat 或 xx.xx.dat的父目录</param>
        /// <param name="abData"></param>
        void _addOrUpdateAsssetBundle(string outputPackPath,AssetBundleData abData)
        {
            var packName = GetAssetBundleFullName(abData.Name,abData.Variant);
            packName = packName.Split('\\', '/').Last();
            packName = GameFramework.Utility.Path.GetResourceNameWithSuffix(packName);
            var dir = Path.GetDirectoryName(abData.Name);

            var info = new AssetBundleInfo()
            {
                PackName = packName,
                PackPath = dir,
                Optional = abData.Optional,
                GroupTag = abData.GroupTag
            };

            info.MD5 = GameFramework.Utility.MD5Util.GetFileMd5(Path.Combine(outputPackPath, info.PackFullName));

            _version.AddOrUpdateAssetBundleInfo(info);
        }

        void _writeVersionInfoFile(string outputZipPath)
        {
            var by = _version.JiaMiSerialize();
            File.WriteAllBytes(Path.Combine(outputZipPath, ConstTable.VersionFileName),
                by);
        }

        
    }
}
