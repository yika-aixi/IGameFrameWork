//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月19日 0:40:17
//Assembly-CSharp

using System;
using UnityEngine.Networking;

namespace Icarus.UnityGameFramework.Runtime
{
    public class StreamDownloadHandle:DownloadHandlerScript
    {
        public string FilePath;
        public Action<string> ErrorHandle;

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            string error;
            if (!GameFramework.Utility.FileUtil.WritingFile(data, FilePath, out error, count: dataLength))
            {
                ErrorHandle?.Invoke(error);
            }

            return base.ReceiveData(data, dataLength);
        }
    }
}