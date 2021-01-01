using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.vm
{
    public class FunctionInformation
    {
        // 函数名
        public string name { get; set; }
        // 入口地址
        public int enrtyAddress { get; set; }
        // 出口地址
        public int outAddress { get; set; }
        // 局部变量表
        public Dictionary<string, int> localVariableTable { get; set; }

        public FunctionInformation(string functionName)
        {
            this.name = functionName;
        }
    }
}
