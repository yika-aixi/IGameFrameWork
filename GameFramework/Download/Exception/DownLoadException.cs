//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月11日 1:06:22
//Assembly-CSharp

namespace Icarus.GameFramework.Download
{
    public class DownLoadException:System.Exception
    {
        public int Code;
        public DownLoadException(string message):base(message)
        {
            
        }

        public DownLoadException(int code,string message) : base(message)
        {
            Code = code;
        }
    }
}