using System.IO;

namespace Icarus.GameFramework.Version
{
    public class AssetBundleInfo
    {
        //所属资源组,多组 ',' 分割
        public string GroupTag;
        //资源包名 : xxx.dat 或 xx.xx.dat
        public string PackName;
        //资源包相对路径 : xxx/xxxx
        public string PackPath;
        //资源包路径 : PackPath + "/" + PackName
        public string PackFullName => GameFramework.Utility.Path.GetRegularPath(Path.Combine(PackPath, PackName));
        //资源包MD5
        public string MD5;
        //是否可选,false的话就会一定被下载,true的话可以在游戏中让玩家决定是否下载
        public bool Optional;

        public override string ToString()
        {
            return string.Format("资源组:{0}\n" +
                                 "资源包名:{1}\n" +
                                 "资源包路径:{2}\n" +
                                 "资源包相对路径:{3}\n" +
                                 "资源包Md5:{4}\n" +
                                 "可选资源包:{5}", GroupTag, PackName, PackPath, PackFullName, MD5, Optional);
        }
    }
}