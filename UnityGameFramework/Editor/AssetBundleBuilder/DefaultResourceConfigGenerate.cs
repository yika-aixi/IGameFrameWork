//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月20日-09:17
//Icarus.UnityGameFramework.Editor

using System;
using System.Collections.Generic;
using System.IO;
using Icarus.GameFramework;
using IGameFrameWork.UnityGameFramework.Runtime.Config;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Icarus.UnityGameFramework.Editor.AssetBundleTools
{
    public class DefaultResourceConfigGenerate : IBuildEventHandler
    {
        public void PreProcessBuildAll(string productName, string companyName, string gameIdentifier, string applicableGameVersion,
            int internalResourceVersion, string unityVersion, BuildAssetBundleOptions buildOptions, string outputDirectory,
            string workingPath, string outputPackagePath, string outputZipPath, string buildReportPath)
        {

        }

        public void PostProcessBuildAll(string productName, string companyName, string gameIdentifier, string applicableGameVersion,
            int internalResourceVersion, string unityVersion, BuildAssetBundleOptions buildOptions, string outputDirectory,
            string workingPath, string outputPackagePath, string outputZipPath, string buildReportPath)
        {
        }

        public void PreProcessBuild(BuildTarget buildTarget, string workingPath, string outputPackagePath, string outputZipPath)
        {
        }

        public void PostProcessBuild(BuildTarget buildTarget, string workingPath, string outputPackagePath, string outputZipPath)
        {
        }

        private const string ConfigFileName = "ResourceConfig.reso";
        public void BuildComplete(string outputPackagePath, Dictionary<string, List<string>> assetBundleAssetPaths)
        {
            if (assetBundleAssetPaths.Count == 0)
            {
                throw new Exception("资源配置文件生成失败,资源数量 = 0,打包应该失败了,请查看'BuildLog.txt'文件.");
            }

            List<AssetConfigEntity> assetConfig = new List<AssetConfigEntity>();

            foreach (var ab in assetBundleAssetPaths)
            {
                foreach (var asset in ab.Value)
                {
                    var name = Path.GetFileName(asset);
                    var path = Path.GetDirectoryName(asset);
                    assetConfig.Add(new AssetConfigEntity()
                    {
                        AssetBundleName = ab.Key,
                        AssetName = name,
                        AssetPath = path
                    });
                }
            }

            byte[] encryptBytes = new byte[4];

            Icarus.GameFramework.Utility.Random.GetRandomBytes(encryptBytes);

            var json = JsonConvert.SerializeObject(assetConfig);
            var jsonBy = Icarus.GameFramework.Utility.Converter.GetBytes(json);
            using (FileStream fileStream = new FileStream(Utility.Path.GetCombinePath(outputPackagePath, ConfigFileName), FileMode.CreateNew, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(encryptBytes);
                    binaryWriter.Write(GetXorBytes(jsonBy, encryptBytes));
                }
            }

            using (FileStream fileStream = new FileStream(Utility.Path.GetCombinePath(outputPackagePath, $"未加密{ConfigFileName}"), FileMode.CreateNew, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(jsonBy);
                }
            }
        }

        private static byte[] GetXorBytes(byte[] bytes, byte[] code, int length = 0)
        {
            if (bytes == null)
            {
                return null;
            }

            int codeLength = code.Length;
            if (code == null || codeLength <= 0)
            {
                throw new GameFrameworkException("Code is invalid.");
            }

            int codeIndex = 0;
            int bytesLength = bytes.Length;
            if (length <= 0 || length > bytesLength)
            {
                length = bytesLength;
            }

            byte[] result = new byte[bytesLength];
            System.Buffer.BlockCopy(bytes, 0, result, 0, bytesLength);

            for (int i = 0; i < length; i++)
            {
                result[i] ^= code[codeIndex++];
                codeIndex = codeIndex % codeLength;
            }

            return result;
        }
    }
}