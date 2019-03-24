//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年08月30日-11:25
//Icarus.GameFramework

using System;
using System.IO;

namespace Icarus.GameFramework.DataTable
{
    public abstract class CSVDataRow : IDataRow,IDataRowCreate
    {
        /// <summary>
        /// 分隔号,默认为空格
        /// </summary>
        protected virtual Char SeparatedValue { get; } = ' ';

        /// <summary>
        /// 字段数量
        /// </summary>
        protected virtual int ColCount { get; } = 2;

        public abstract int Id { get; protected set; }

        public void ParseDataRow(string dataRowText)
        {
            string[] text = dataRowText.Split(SeparatedValue);

            if(text.Length != ColCount)
            {
                throw new GameFrameworkException($"CSV 字段数量和数据不一致," +
                                                 $"请重写ColCount,现在ColCount为:{ColCount}," +
                                                 $"数据解析数量为:{text.Length}");
            }

            for (var index = 0; index < text.Length; index++)
            {
                var str = text[index];
                SetData(index, str);
            }
        }

        public void ParseDataRow(ArraySegment<byte> dataRowBytes)
        {
            throw new NotImplementedException();
        }

        public void ParseDataRow(Stream stream, int dataRowOffset, int dataRowLength)
        {
            throw new NotImplementedException();
        }

        protected abstract void SetData(int index, string data);

        public abstract string Create();
    }
}