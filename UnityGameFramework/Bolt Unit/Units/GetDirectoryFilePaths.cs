//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月25日-10:34
//Icarus.UnityGameFramework.Bolt

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bolt;
using Icarus.GameFramework;
using Ludiq;

namespace Icarus.UnityGameFramework.Bolt.Units
{
    [UnitCategory("Icarus/Util")]
    [UnitTitle("获取指定目录下所有文件路径")]
    [UnitSubtitle("如果路径错误将返回null")]
    public class GetDirectoryFilePaths : GameFrameWorkBaseUnit
    {
        [Serialize]
        [Inspectable, UnitHeaderInspectable("搜索选项")]
        public SearchOption SerOption = SearchOption.AllDirectories;

        [Serialize]
        [Inspectable, UnitHeaderInspectable("删除DataPath")]
        public bool isDelect = true;

        [Serialize]
        [Inspectable, UnitHeaderInspectable("过滤Meta文件")]
        public bool isFilter = true;

        [DoNotSerialize]
        [PortLabel("目录路径")]
        public ValueInput _directoryPath;

        [DoNotSerialize]
        [PortLabel("结果")]
        public ValueOutput _resultOut;

        protected override void Definition()
        {
            base.Definition();
            _directoryPath = ValueInput<string>(nameof(_directoryPath));
            _resultOut = ValueOutput<IEnumerable<string>>(nameof(_resultOut));
            Requirement(_directoryPath, _enter);
            Assignment(_enter, _resultOut);
        }

        private const string _meta = ".meta";

        protected override ControlOutput Enter(Flow flow)
        {
            var dir = flow.GetValue<string>(_directoryPath);

            if (!Directory.Exists(dir))
            {
                flow.SetValue(_resultOut, null);
            }
            else
            {
                IEnumerable<string> result = Directory.GetFiles(dir, "*", SerOption);

                if (isFilter)
                {
                    result = result.Where(str => !str.Contains(_meta));
                }

                if (isDelect)
                {
                    result = result.Select(x =>
                        Utility.Path.GetRegularPath(Utility.Path.GetRegularPath(x).
                            Replace(Utility.Path.GetRegularPath(UnityEngine.Application.dataPath), "Assets")));
                }
              
                flow.SetValue(_resultOut, result);
            }

            return _exit;
        }
    }
}