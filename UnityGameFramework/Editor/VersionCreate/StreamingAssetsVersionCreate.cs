using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Icarus.GameFramework;
using Icarus.GameFramework.Version;
using UnityEditor;
using UnityEngine;

namespace Icarus.UnityGameFramework.Editor
{
    public class StreamingAssetsVersionCreate
    {
        [MenuItem("Icarus/Game Framework/AssetBundle Tools/StreamingAssets Version 生成")]
        public static void Create()
        {
            var filePaths = Directory.GetFiles(Application.streamingAssetsPath, "*~version.dat",
                SearchOption.AllDirectories);

            if (filePaths.Length == 0)
            {
                throw new GameFrameworkException("在" + Application.streamingAssetsPath + "目录下,没有找到ab包的version文件");
            }

            string AbListPath =
                Icarus.GameFramework.Utility.Path.GetCombinePath(Application.streamingAssetsPath, "version");

            if (File.Exists(AbListPath))
            {
                File.Delete(AbListPath);
            }


            StringBuilder sb = new StringBuilder();

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath);
                fileName = fileName.Split('.')[0];
                sb.Append(fileName + "|");

            }
            sb.Remove(sb.Length - 1, 1);
            byte[] encryptBytes = new byte[4];
            Icarus.GameFramework.Utility.Random.GetRandomBytes(encryptBytes);
            using (FileStream fileStream = new FileStream(AbListPath, FileMode.CreateNew, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(encryptBytes);
                    var by = GetXorBytes(Icarus.GameFramework.Utility.Converter.GetBytes(sb.ToString()), encryptBytes);
                    binaryWriter.Write(by);
                }
            }

            var newAbListPath = Icarus.GameFramework.Utility.Path.GetResourceNameWithSuffix(AbListPath);
            if (File.Exists(newAbListPath))
            {
                File.Delete(newAbListPath);
            }
            File.Move(AbListPath, newAbListPath);

            _updateOrCreateVersionInfoFile();
        }

        private static void _updateOrCreateVersionInfoFile()
        {
            string[] filePaths = Directory.GetFiles(Application.streamingAssetsPath, "*.dat",
                SearchOption.AllDirectories);
            if (filePaths.Length == 0)
            {
                throw new GameFrameworkException("在" + Application.streamingAssetsPath + "目录下,没有找到ab包");
            }

            string versionInfoListPath =
                Icarus.GameFramework.Utility.Path.GetCombinePath(Application.streamingAssetsPath, "version.info");
            VersionInfo version = new VersionInfo();
            if (File.Exists(versionInfoListPath))
            {
                var by = File.ReadAllBytes(versionInfoListPath);
                version = version.JieMiDeserialize(by);
            }
            else
            {
                version = new VersionInfo(Application.version);
            }

            int i = 0;
            int j = 0;
            List<AssetBundleInfo> deleteList = new List<AssetBundleInfo>();
            foreach (var info in version.AssetBundleInfos)
            {
                bool hit = false;
                foreach (var filePath in filePaths)
                {
                    var relativePath = filePath.Replace(Application.streamingAssetsPath+"\\", "");
                    relativePath = GameFramework.Utility.Path.GetRegularPath(relativePath);
                    if (info.PackFullName == relativePath)
                    {
                        hit = true;
                        break;
                    }
                }

                if (!hit)
                {
                    deleteList.Add(info);
                }
            }

            j = deleteList.Count;
            foreach (var info in deleteList)
            {
                version.Remove(info);
            }

            foreach (var filePath in filePaths)
            {
                if (filePath.Contains("version"))
                {
                    continue;
                }

                var relativePath = filePath.Replace(Application.streamingAssetsPath + "\\", "");
                relativePath = GameFramework.Utility.Path.GetRegularPath(relativePath);
                if (version.HasAssetBundle(relativePath))
                {
                    var abInfo = version.GetAssetBundleInfo(relativePath);
                    var newMd5 = Icarus.GameFramework.Utility.MD5Util.GetFileMd5(filePath);
                    if (!abInfo.MD5.Equals(newMd5))
                    {
                        abInfo.MD5 = newMd5;
                        ++i;
                    }
                    continue;

                }

                ++i;
                var packName = relativePath.Split('/', '\\').Last();
                var packPath = Path.GetDirectoryName(relativePath);
                version.AddOrUpdateAssetBundleInfo(new AssetBundleInfo()
                {
                    PackName = packName,
                    PackPath = packPath,
                    Optional = false,
                    MD5 = Icarus.GameFramework.Utility.MD5Util.GetFileMd5(filePath)
                });
            }

            var bytes = version.JiaMiSerialize();
            File.WriteAllBytes(versionInfoListPath, bytes);

            foreach (var bundleInfo in deleteList)
            {
                Debug.Log("被删除的资源包信息:" + bundleInfo);
            }

            foreach (var bundleInfo in version.AssetBundleInfos)
            {
                Debug.Log("更新后的资源包信息:"+bundleInfo);
            }

            Debug.Log("更新version.info文件完成,更新了"+i+"个,删除了:"+j+".如果是新增加的资源包,那么他们都是必须的资源包,且都没资源组Tag");

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