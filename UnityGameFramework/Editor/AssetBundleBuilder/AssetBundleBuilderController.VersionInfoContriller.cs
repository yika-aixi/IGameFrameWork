//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System;
using Icarus.GameFramework.Version;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Icarus.UnityGameFramework.Editor.AssetBundleTools
{
    internal partial class AssetBundleBuilderController
    {
        private VersionInfo _version;
        void _initVersion(string version,int minAppVersion)
        {
            _version = new VersionInfo(version, minAppVersion);
            try
            {
                var i = int.Parse(Application.version.Split('.').Last());
            }
            catch (Exception)
            {
                Debug.LogError(
                    $"使用'DefaultVersionCheckCompontent'时,请确保 Edit-->" +
                    $"Project Settings-->Player --> " +
                    $"Other Setting 下的 Version " +
                    $"字段‘.’分割的" +
                    $"最后一位是int值，如：0.1.1s.2,‘2’就是'DefaultVersionCheckCompontent'组件" +
                    $"用来判断APP版本是否满足最低版本要求的值");
            }
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
