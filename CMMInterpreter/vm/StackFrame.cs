using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.vm
{
    /**
     * @author 谭惠日 杨翔
     * @since 12-07-2020
     * 用于运行时过程调用
     * 
     */
    class StackFrame
    {
        // 参数列表
        private List<Object> parameterList;

        // 局部变量栈
        private List<Object> localVariables;

        // 操作栈
        private Stack<Object> opStack;

        // 返回地址 对应中间的行号
        private int returnAddress;


        StackFrame(int _returnAddress)
        {
            parameterList = new List<object>();
            localVariables = new List<object>();
            opStack = new Stack<object>();
            returnAddress = _returnAddress;
        }





    }
}
