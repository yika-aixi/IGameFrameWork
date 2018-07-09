using System;
using System.IO;
using System.Security.Cryptography;

namespace Icarus.GameFramework
{
    public static partial class Utility
    {
        public class MD5Util
        {
            public static string GetFileMd5(string filePath)
            {
                var md5 = new MD5Cng();
                var file = File.Open(filePath, FileMode.Open);
                var by = md5.ComputeHash(file);
                file.Close();
                file.Dispose();
                return BitConverter.ToString(by);
            }
        }
    }
}