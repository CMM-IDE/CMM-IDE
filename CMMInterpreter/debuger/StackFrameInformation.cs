using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.debuger
{
    /// <summary>
    /// 调用栈信息
    /// </summary>
    public class StackFrameInformation
    {
        /// <summary>
        /// 函数名
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 函数体起始行号
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// 函数栈帧
        /// </summary>
        public List<FrameInformation> Frame { get; set; }
    }
}
