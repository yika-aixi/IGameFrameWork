//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月30日-01:19
//Icarus.UnityGameFramework.Bolt

using System.Text;
using Bolt;
using Ludiq;
using UnityEngine.UI;

namespace Icarus.UnityGameFramework.Bolt.Units.UGUI
{
    [UnitCategory("Icarus/Util/UGUI")]
    [UnitTitle("Text Content Append")]
    [UnitSubtitle("Text组件内容追加")]
    public class TextAppend:GameFrameWorkBaseUnit
    {

        [Serialize]
        [Inspectable, UnitHeaderInspectable("换行追加")]
        public bool LineAppend;

        [DoNotSerialize]
        [PortLabel("Text")]
        public ValueInput _textCom;

        [DoNotSerialize]
        [PortLabel("Content")]
        public ValueInput _content;

        protected override void Definition()
        {
            base.Definition();
            _textCom = ValueInput<Text>(nameof(_textCom));
            _content = ValueInput<object>(nameof(_content));

            Requirement(_textCom,_enter);
            Requirement(_content, _enter);
        }

        protected override ControlOutput Enter(Flow flow)
        {
            var text = flow.GetValue<Text>(_textCom);
            var content = flow.GetValue<object>(_content);
            StringBuilder sb = new StringBuilder(text.text);

            if (LineAppend)
            {
                sb.AppendLine(content.ToString());
            }
            else
            {
                sb.Append(content);
            }

            text.text = sb.ToString();

            return _exit;
        }
    }
}