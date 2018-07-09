using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Icarus.GameFramework.Version
{
    public class VersionInfo
    {
        //Version.info 的版本
        [JsonProperty("Version")]
        public string Version { get; private set; }
        [JsonProperty("AssetBundleInfos")]
        //当前app中所有的资源包信息:持久化目录及StreamingAssets目录中所有的ab包信息
        //并集 -> 以持久化为主
        public List<AssetBundleInfo> AssetBundleInfos { get; private set; }

        public VersionInfo():this("")
        { 
        }
        public VersionInfo(string version):this(version,new List<AssetBundleInfo>())
        {
        }
        
        [JsonConstructor]
        public VersionInfo(string version, List<AssetBundleInfo> assetBundleInfos)
        {
            Version = version;
            AssetBundleInfos = assetBundleInfos;
        }
        
        /// <summary>
        /// 是否存在该资源包
        /// </summary>
        /// <param name="abFilePath">资源包相对路径</param>
        /// <returns></returns>
        public bool HasAssetBundle(string abFilePath)
        {
            abFilePath = GameFramework.Utility.Path.GetRegularPath(abFilePath);
            return AssetBundleInfos.Any(x => x.PackFullName == abFilePath);
        }
        
        /// <summary>
        /// 获取某资源包信息
        /// </summary>
        /// <param name="abFilePath">资源包相对路径</param>
        /// <returns>不存在返回null</returns>
        public AssetBundleInfo GetAssetBundleInfo(string abFilePath)
        {
            if (!HasAssetBundle(abFilePath))
            {
                return null;
            }
            abFilePath = GameFramework.Utility.Path.GetRegularPath(abFilePath);
            return AssetBundleInfos.First(x => x.PackFullName == abFilePath);
        }

        /// <summary>
        /// 设置资源包列表
        /// </summary>
        /// <param name="infos"></param>
        public void SetAssetBundleInfos(List<AssetBundleInfo> infos)
        {
            AssetBundleInfos = infos;
        }

        public void SetVersion(string version)
        {
            Version = version;
        }

        /// <summary>
        /// 添加或更新资源列表
        /// </summary>
        /// <param name="infos"></param>
        public void AddOrUpdateRanageAssetBundleInfo(IEnumerable<AssetBundleInfo> infos)
        {
            foreach (var info in infos)
            {
                AddOrUpdateAssetBundleInfo(info);
            }
        }
        public void AddOrUpdateAssetBundleInfo(AssetBundleInfo info)
        {
            var abInfo = GetAssetBundleInfo(info.PackFullName);
            if (abInfo != null)
            {
                abInfo.MD5 = info.MD5;
                abInfo.GroupTag = info.GroupTag;
                abInfo.Optional = info.Optional;
                return;
            }
            AssetBundleInfos.Add(info);
        }

        public void Remove(AssetBundleInfo info)
        {
            AssetBundleInfos.Remove(info);
        }

        public void Remove(int index)
        {
            AssetBundleInfos.RemoveAt(index);
        }

        public void Clear()
        {
            AssetBundleInfos.Clear();
        }

        /// <summary>
        /// 获取资源组资源包列表
        /// </summary>
        /// <param name="tag">默认为:Empty</param>
        /// <returns></returns>
        public List<AssetBundleInfo> GetGroupAssetBundleInfos(string tag = "")
        {
            List<AssetBundleInfo> result = new List<AssetBundleInfo>();
            foreach (var info in AssetBundleInfos)
            {
                var tags = info.GroupTag.Split(',');
                foreach (var t in tags)
                {
                    //指定tag 不是空的
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        //tag是空的
                        if (string.IsNullOrWhiteSpace(t))
                        {
                            continue;
                        }
                        //tag不一样
                        if (tag != t)
                        {
                            continue;
                        }
                    }
                    else //指定tag是空的
                    {
                        //tag不是空的
                        if (!string.IsNullOrWhiteSpace(t))
                        {
                            continue;
                        }
                    }

                    result.Add(info);
                }
            }
            return result;
        }

        public byte[] JiaMiSerialize()
        {
            List<byte> bytes = new List<byte>();
            byte[] encryptBytes = new byte[4];
            Icarus.GameFramework.Utility.Random.GetRandomBytes(encryptBytes);
            bytes.AddRange(encryptBytes);
            var json = this.SerializeJson();
            var jsonBytes = _getXorBytes(Icarus.GameFramework.Utility.Converter.GetBytes(json), encryptBytes);
            bytes.AddRange(jsonBytes);
            return bytes.ToArray();
        }

        public VersionInfo JieMiDeserialize(byte[] bytes)
        {
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream(bytes);
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    memoryStream = null;
                    byte[] encryptBytes = binaryReader.ReadBytes(4);
                    byte[] jiaMiJson = binaryReader.ReadBytes(bytes.Length - 4);
                    var json = Icarus.GameFramework.Utility.Converter.GetString(
                        Icarus.GameFramework.Utility.Encryption.GetXorBytes(jiaMiJson, encryptBytes));
                    return this.JsonDeserialize(json);
                }
            }
            catch (Exception exception)
            {
                if (exception is GameFrameworkException)
                {
                    throw;
                }

                throw new Exception(string.Format("解密反序列化失败. '{0}'.", exception.Message), exception);
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                    memoryStream = null;
                }
            }
        }

        private byte[] _getXorBytes(byte[] bytes, byte[] code, int length = 0)
        {
            if (bytes == null)
            {
                return null;
            }

            int codeLength = code.Length;
            if (code == null || codeLength <= 0)
            {
                throw new Exception("Code is invalid.");
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

    public static class VersionInfoEx
    {
        public static string SerializeJson(this VersionInfo info)
        {
            return JsonConvert.SerializeObject(info);
        }
        
        public static VersionInfo JsonDeserialize(this VersionInfo info,string json)
        {
            return JsonConvert.DeserializeObject<VersionInfo>(json);
        }
    }
}