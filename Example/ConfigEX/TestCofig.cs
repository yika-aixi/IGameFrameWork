//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月20日-01:52
//Icarus.Chess

using IGameFrameWork.GameFramework.Config;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Test
{
    [ExecuteInEditMode]
    public class TestCofig : MonoBehaviour
    {
        public string FilePath;


        private readonly JsonConfig<GameObject> _jsonConfig = new JsonConfig<GameObject>();

        [ContextMenu("加载")]
        void Load()
        {
            _jsonConfig.LoadConfigAsyn(
                Icarus.GameFramework.Utility.Path.GetCombinePath
                    (Application.dataPath, FilePath), x =>
                {
                    Debug.Log("配置:"+x);
                    var jobj = _jsonConfig.ParseJObject(x);
                    Debug.Log(jobj.First);
                }, ex =>
                {
                    Debug.LogError("异常:"+ex);
                });
        }

    }
}