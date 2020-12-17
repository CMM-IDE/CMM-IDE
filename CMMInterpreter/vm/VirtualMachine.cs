using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.vm
{
    class VirtualMachine
    {
        // 运行时栈
        private List<StackFrame> stack;

        // 中间代码
        private List<IntermediateCode> codes;

        // 程序计数器
        private int pc;


        VirtualMachine(List<IntermediateCode> _codes)
        {
            codes = _codes;



        }

        // 解释执行代码
        public void interpret()
        {

        }


    }
}
