//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月31日-01:10
//Icarus.UnityGameFramework.Bolt

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Bolt;
using Icarus.GameFramework;
using Icarus.GameFramework.DataTable;
using Icarus.GameFramework.Event;
using Icarus.UnityGameFramework.Bolt.Util;
using Icarus.UnityGameFramework.Runtime;
using Ludiq;
using LoadDataTableFailureEventArgs = Icarus.UnityGameFramework.Runtime.LoadDataTableFailureEventArgs;
using LoadDataTableSuccessEventArgs = Icarus.UnityGameFramework.Runtime.LoadDataTableSuccessEventArgs;
using LoadDataTableUpdateEventArgs = Icarus.UnityGameFramework.Runtime.LoadDataTableUpdateEventArgs;

namespace Icarus.UnityGameFramework.Bolt.Units
{
    
    public enum CallDataTableType
    {
        加载数据表,
        获取_ID最小的行,
        获取_ID最大的行,
        获取_随机一行,
        获取_遍历数据,
        获取_根据ID,
    }

    /// <summary>
    /// 创建和获取数据表使用 _dataTableNameInType 来当数据表名
    /// </summary>
    [UnitCategory("Icarus/IUGF")]
    [UnitTitle("DataTable Manager")]
    [UnitSubtitle("资源表管理")]
    public class DataTableManagerUnit:GameFrameWorkBaseUnit
    {
        [Serialize]
        [Inspectable, UnitHeaderInspectable("数据表类型:")]
        public Type _type { get; set; }

        [Serialize]
        [Inspectable, UnitHeaderInspectable("反射输出:")]
        public bool _isType { get; set; }
        
        [Serialize]
        [Inspectable, UnitHeaderInspectable("获取类型:")]
        public CallDataTableType _callDataTableTableType { get; set; } = CallDataTableType.加载数据表;

        [DoNotSerialize]
        [PortLabel("DataTable AssetName")]
        public ValueInput _assetName;

        [DoNotSerialize]
        [PortLabel("DataTable Name")]
        public ValueInput _dataTableName;

        [DoNotSerialize]
        [PortLabel("dataTable Name In Type")]
        public ValueInput _dataTableNameInType;

        [DoNotSerialize]
        [PortLabel("priority")]
        public ValueInput _priority;

        [DoNotSerialize]
        [PortLabel("User Data")]
        public ValueInput _userDataIn;


        [DoNotSerialize]
        [PortLabel("DataTabble Content")]
        public ValueInput _content;
        
        [DoNotSerialize]
        [PortLabel("ID")]
        public ValueInput _id;

        [DoNotSerialize]
        public ValueOutput[] _colsOut;
        
        [DoNotSerialize]
        [PortLabel("查询结果")]
        public ValueOutput _resultOut;

        [DoNotSerialize]
        [PortLabel("加载成功")]
        public ControlOutput _loadSuccess;

        [DoNotSerialize]
        [PortLabel("加载中")]
        public ControlOutput _loadUpdate;

        [DoNotSerialize]
        [PortLabel("加载进度")]
        public ValueOutput _progressOut;

        [DoNotSerialize]
        [PortLabel("加载失败")]
        public ControlOutput _loadFailure;
        
        [DoNotSerialize]
        [PortLabel("Body")]
        public ControlOutput _body;
        
        [DoNotSerialize]
        [PortLabel("User Data")]
        public ValueOutput _userDataOut;

        [DoNotSerialize]
        [PortLabel("失败信息")]
        public ValueOutput _errorMessageOut;

        private float _progress;
        private string _errorMesage;
        private object[] _cols;
        private object _userData;
        private object _result;
        protected override void Definition()
        {
            base.Definition();

            if (!_typeCheck())
            {
                return;
            }

            if (_callDataTableTableType == CallDataTableType.加载数据表)
            {
                _assetName = ValueInput<string>(nameof(_assetName));
                Requirement(_assetName,_enter);
            }

            _dataTableNameInType = ValueInput<string>(nameof(_dataTableNameInType));
            
            Requirement(_dataTableNameInType,_enter);
            
            if (_callDataTableTableType == CallDataTableType.加载数据表)
            {
                _dataTableName = ValueInput(nameof(_dataTableName),string.Empty);
                _userDataIn = ValueInput<object>(nameof(_userDataIn));
                _priority = ValueInput(nameof(_priority),0);
                _userDataOut = ValueOutput(nameof(_userDataOut), x => _userData);
                _loadSuccess = ControlOutput(nameof(_loadSuccess));
                _loadUpdate = ControlOutput(nameof(_loadUpdate));
                _loadFailure = ControlOutput(nameof(_loadFailure));
                _progressOut = ValueOutput(nameof(_progressOut), x => _progress);
                _errorMessageOut = ValueOutput(nameof(_errorMessageOut), x => _errorMesage);
            }

            if (_callDataTableTableType == CallDataTableType.获取_遍历数据)
            {
                _body = ControlOutput(nameof(_body));
            }
            
            if (_callDataTableTableType == CallDataTableType.获取_根据ID)
            {
                _id = ValueInput(nameof(_id), 0);
            }


            switch (_callDataTableTableType)
            {
                case CallDataTableType.获取_ID最小的行:
                case CallDataTableType.获取_ID最大的行:
                case CallDataTableType.获取_随机一行:
                case CallDataTableType.获取_遍历数据:
                case CallDataTableType.获取_根据ID:
                    var properties = _type.GetProperties();
                    if (_isType)
                    {
                        _initCol(properties);
                    }
                    else
                    {
                        _initResult();
                    }
                    break;
            }

        }

        private void _initResult()
        {
            _resultOut = ValueOutput(_type, nameof(_resultOut), x => _result);
        }

        private void _initCol(PropertyInfo[] properties)
        {
            var count = properties.Length;
            _colsOut = new ValueOutput[count];
            _cols = new object[count];
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                int index = i;
                _colsOut[i] = ValueOutput(property.PropertyType, property.Name, x => _cols[index]);
            }
        }

        private bool _typeCheck()
        {
            if (_type == null)
            {
                return false;
            }
            
            
            if (!typeof(IDataRow).IsAssignableFrom(_type))
            {
                throw new Exception($"类型错误，需要 {typeof(IDataRow).AssemblyQualifiedName} 类型");
            }

            return true;
        }

        private static DataTableComponent _dataTableComponent;
        private static EventComponent _event;

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);

            if (_dataTableComponent == null)
            {
                _dataTableComponent = GameEntry.GetComponent<DataTableComponent>();
            }

            if (_event == null)
            {
                _event = GameEntry.GetComponent<EventComponent>();
            }
        }

        private Flow _flow;
        protected override ControlOutput Enter(Flow flow)
        {
            if (!_typeCheck())
            {
                throw new GameFrameworkException("Type is null");
            }
            
            var dataTableNameInType = flow.GetValue<string>(_dataTableNameInType);
            
            switch (_callDataTableTableType)
            {
                case CallDataTableType.加载数据表:
                case CallDataTableType.获取_遍历数据:
                    _flow = flow.GetNewFlow();
                    break;
            }
            
            switch (_callDataTableTableType)
            {
                case CallDataTableType.加载数据表:
                    var dataTableName = flow.GetValue<string>(_dataTableName);
                    var assetName = flow.GetValue<string>(_assetName);
                    var userData = flow.GetValue<object>(_userDataIn);
                    var priority = flow.GetValue<int>(_priority);
                    _subscribeLoadDataTableEvent();
                    _dataTableComponent.LoadDataTable(_type, dataTableName,dataTableNameInType,assetName, priority, userData);
                    break;
                case CallDataTableType.获取_ID最小的行:
                case CallDataTableType.获取_ID最大的行:
                case CallDataTableType.获取_随机一行:
                case CallDataTableType.获取_遍历数据:
                case CallDataTableType.获取_根据ID: 
                    var table = _dataTableComponent.GetDataTable(_type, dataTableNameInType);
                    switch (_callDataTableTableType)
                    {
                        case CallDataTableType.获取_ID最小的行:
                            var row = table.GetDataRowMinId();
                            _setResult(row);
                            break;
                        case CallDataTableType.获取_ID最大的行:
                             row = table.GetDataRowMxnId();
                            _setResult(row);
                            break;
                        case CallDataTableType.获取_随机一行:
                            row = table.GetDataRowRandom();
                            _setResult(row);
                            break;
                        case CallDataTableType.获取_根据ID:
                            row = table.GetDataRowType(flow.GetValue<int>(_id));
                            _setResult(row);
                            break;
                        case CallDataTableType.获取_遍历数据:
                            var rows = table.GetAllDataRowTypes();
                            foreach (var dataRow in rows)
                            {
                                _setResult(dataRow);
                                _flow.EnterTryControl(_body);
                            }
                            _flow.Dispose();
                            _flow = null;
                            break;
                    }
                    

                    break;
                default:
                    throw new GameFrameworkException($"缺失{_callDataTableTableType} 逻辑");
            }

            return _exit;
        }

        
        private void _setResult(IDataRow row)
        {
            if (_isType)
            {
                _setCols(row);
            }
            else
            {
                _result = row;
            }
        }

        private void _setCols(IDataRow row)
        {
            for (var index = 0; index < _colsOut.Length; index++)
            {
                var colOut = _colsOut[index];
                var propertie = row.GetType().GetProperty(colOut.key, colOut.type);
                _cols[index] = propertie.GetValue(row);
            }
        }

        private void _subscribeLoadDataTableEvent()
        {
            var id = GetEventID<LoadDataTableSuccessEventArgs>();
            _event.Subscribe(id, _success);
            id = GetEventID<LoadDataTableUpdateEventArgs>();
            _event.Subscribe(id, _update);
            id = GetEventID<LoadDataTableFailureEventArgs>();
            _event.Subscribe(id, _failure);
        }

        private void _failure(object sender, GameEventArgs e)
        {
            var arg = (LoadDataTableFailureEventArgs) e;
                
            _errorMesage = arg.ErrorMessage;
            _userData = arg.UserData;
            
            if (_flow.EnterTryControlAndDispose(_loadFailure))
            {
                _flow = null;
            }
            
            _event.Unsubscribe(e.Id,_failure);
            
            _unsubscribeEvent(false);
        }

        private void _unsubscribeEvent(bool isSuccess)
        {
            var id = GetEventID<LoadDataTableUpdateEventArgs>();
            _event.Unsubscribe(id,_update);

            if (isSuccess)
            {
                id = GetEventID<LoadDataTableFailureEventArgs>();
                _event.Unsubscribe(id, _failure);
            }
            else
            {
                id = GetEventID<LoadDataTableSuccessEventArgs>();
                _event.Unsubscribe(id, _success);
            }
            
        }

        private void _update(object sender, GameEventArgs e)
        {
            var arg = (LoadDataTableUpdateEventArgs) e;
                
            _progress = arg.Progress;
            _userData = arg.UserData;
            
            _flow.EnterTryControl(_loadUpdate);
        }

        private void _success(object sender, GameEventArgs e)
        {
            
            
            _userData = ((LoadDataTableSuccessEventArgs) e).UserData;
            
            if (_flow.EnterTryControlAndDispose(_loadSuccess))
            {
                _flow = null;
            }
            
            _event.Unsubscribe(e.Id,_success);
            
            _unsubscribeEvent(true);
            
        }
    }
}