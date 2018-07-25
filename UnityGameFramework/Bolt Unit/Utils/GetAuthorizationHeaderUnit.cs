//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月25日-10:54
//Icarus.UnityGameFramework.Bolt

using System.Collections.Generic;
using Bolt;
using Ludiq;

namespace Assets.IGameFrameWork.UnityGameFramework.Bolt_Unit.Utils
{
    [UnitCategory("Icarus/Util")]
    [UnitTitle("获取授权头")]
    [UnitSubtitle("HFS用的，不知道其他的是否可用")]
    public class GetAuthorizationHeaderUnit:Unit
    {

        [DoNotSerialize]
        [PortLabel("用户名")]
        public ValueInput _userName;

        [DoNotSerialize]
        [PortLabel("密码")]
        public ValueInput _passWord;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput _enter;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput _exit;


        [DoNotSerialize]
        [PortLabel("授权头")]
        public ValueOutput _authorizationHeader;

        protected override void Definition()
        {
            _enter = ControlInput(nameof(_enter), __enter);
            _exit = ControlOutput(nameof(_exit));
            _userName = ValueInput<string>(nameof(_userName));
            _passWord = ValueInput<string>(nameof(_passWord));
            _authorizationHeader = ValueOutput(nameof(_authorizationHeader),x=> __authorization);

            Requirement(_userName,_enter);
            Requirement(_passWord, _enter);

            Assignment(_enter, _authorizationHeader);

            Succession(_enter,_exit);
        }

        private Dictionary<string, string> __authorization;
        private ControlOutput __enter(Flow flow)
        {
            string authorization = authenticate(flow.GetValue<string>(_userName),
                flow.GetValue<string>(_passWord));

            __authorization = new Dictionary<string, string>(){
                {"AUTHORIZATION", authorization}
            };

            return _exit;
        }

        string authenticate(string username, string password)
        {
            string auth = username + ":" + password;
            auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
            auth = "Basic " + auth;
            return auth;
        }
    }
}