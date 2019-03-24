//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System;
using System.IO;

namespace Icarus.GameFramework.DataTable
{
    /// <summary>
    /// 数据表基类。
    /// </summary>
    public abstract class DataTableBase
    {
        private readonly string m_Name;

        /// <summary>
        /// 初始化数据表基类的新实例。
        /// </summary>
        public DataTableBase()
            : this(null)
        {

        }

        /// <summary>
        /// 初始化数据表基类的新实例。
        /// </summary>
        /// <param name="name">数据表名称。</param>
        public DataTableBase(string name)
        {
            m_Name = name ?? string.Empty;
        }

        /// <summary>
        ///  随机获取行
        /// </summary>
        /// <returns></returns>
        public abstract IDataRow GetDataRowRandom();
        
        /// <summary>
        /// 获取id最小的行
        /// </summary>
        /// <returns></returns>
        public abstract IDataRow GetDataRowMinId();
        
        /// <summary>
        /// 获取id最大的行
        /// </summary>
        /// <returns></returns>
        public abstract IDataRow GetDataRowMxnId();
        
        /// <summary>
        /// 获取行
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IDataRow GetDataRowType(int id);
        
        /// <summary>
        /// 获取所有数据表行。
        /// </summary>
        /// <returns>所有数据表行。</returns>
        public abstract IDataRow[] GetAllDataRowTypes();
        
        /// <summary>
        /// 获取数据表名称。
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// 获取数据表行的类型。
        /// </summary>
        public abstract Type Type
        {
            get;
        }

        /// <summary>
        /// 获取数据表行数。
        /// </summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// 增加数据表行。
        /// </summary>
        /// <param name="dataRowSegment">要解析的数据表行片段。</param>
        internal abstract bool AddDataRow(GameFrameworkSegment<string> dataRowSegment);

        /// <summary>
        /// 增加数据表行。
        /// </summary>
        /// <param name="dataRowSegment">要解析的数据表行片段。</param>
        internal abstract bool AddDataRow(GameFrameworkSegment<byte[]> dataRowSegment);

        /// <summary>
        /// 增加数据表行。
        /// </summary>
        /// <param name="dataRowSegment">要解析的数据表行片段。</param>
        internal abstract bool AddDataRow(GameFrameworkSegment<Stream> dataRowSegment);

        
        /// <summary>
        /// 关闭并清理数据表。
        /// </summary>
        internal abstract void Shutdown();
    }
}
