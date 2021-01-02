using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.debuger
{
    /// <summary>
    /// 源代码-中间代码信息
    /// </summary>
    public class IntermediateCodeInformation
    {
        /// <summary>
        /// 源代码行号
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// 中间代码地址
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// 该行源代码是否含有函数调用
        /// </summary>
        public bool IsFunctionCall { get; set; }

        /// <summary>
        /// 调用函数入口地址表
        /// </summary>
        public LinkedList<int> FuncionEntryList { get; set; }


        /// <summary>
        /// 是否为函数体语句
        /// </summary>
        public bool IsFunctionBody { get; set; }
    }
}
