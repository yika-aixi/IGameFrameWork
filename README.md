# IGameFramework

# zh：`Icarus/Util/Bolt/Update Flow Unit`工具暂时不要使用，我马上加备份功能，避免意外

# en：`Icarus/Util/Bolt/Update Flow Unit` tool is temporarily Do not use it, I immediately add backup function to avoid accidents.


基于GameFrameWork修改的个人框架. 源框架:[UGF](https://github.com/EllanJiang/UnityGameFramework)

`例子运行报空指针问题,是因为脚本执行顺序问题,在 Editor -> project settings -> script execution order 里设置test脚本一个较大的值,如10000,就可以了`

资源组的使用及说明:[Goto](http://www.xn--qoqr9hxvue5g.com:9555/Blog/read/14)

多语言组件的介绍:[Goto](http://www.xn--qoqr9hxvue5g.com:9555/Blog/read/15)

新的version名为:

	ab包名_变体名_~version.dat

新的加载逻辑为:

	先加载StreamingAssets目录下的version.dat文件,然后解析加载StreamingAssets下的ab包version,加载完成后加载持久化目录下的
	所有ab包version文件,如果资源路径一样的将会被覆盖
	

版本更新的逻辑:

	现在只会生成Package目录下的ab包,该目录可以被复制到StreamingAssets目录(可以手动选择复制,也可以在打包时勾选CopyStreamingAssets选项),
	新增了一个Zip目录.目前的规划是:将Zip下对应平台文件夹下所有文件夹及文件复制到远程,然后游戏中会进行资源包对比是否需要更新,可选
	的资源如果本地version.info中没有存在记录就不比较,存在才会进行比较是否需要更新
	
目前已经完成,使用非常简单,打包完成后,只需将Zip文件夹中对应平台文件下的所有文件复制到服务器中就好了,然后:
```
DefaultVersionCheckComPontent.Url = "服务器的version.info地址";
DefaultVersionCheckComPontent.Check()
DefaultUpdateAssetBundle.UpdateAssetBundle()

```
具体的使用参考[Example\test.cs](https://github.com/yika-aixi/GameFrameworkAssetBundleEditor/blob/%E9%AD%94%E6%94%B9/Example/test.cs)

和E大的AssetBundleTool不同的地方有2个地方以及删除了一些东西:

	1.AssetBundle Editor中的:packed选项变为了标记改资源包是否可选(默认是必须的)
	2.AssetBundle Builder中:增加了一个CopyStreamingAssets的功能
	3.删除了zip的压缩相关
	
如果手动增删改过StreamingAssets目录下的资源,请执行一下 ` Icarus/Game Framework/AssetBundle Tools/StreamingAssets Version 生成 `
以便更新资源记录文件和当前修改的同步

框架地址:https://github.com/EllanJiang/UnityGameFramework

框架文档:http://gameframework.cn/

使用 AssetBundle 编辑工具教程:http://gameframework.cn/archives/320

使用 AssetBundle 构建工具教程:http://gameframework.cn/archives/356
