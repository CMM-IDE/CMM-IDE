using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.debuger
{
    /// <summary>
    /// 栈帧信息
    /// </summary>
    public class FrameInformation
    {
        /// <summary>
        /// 地址
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// 符号名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
