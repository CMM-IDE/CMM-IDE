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

        // 局部变量栈
        private List<Object> variableStack;

        // 操作栈
        private Stack<Object> operantStack;

        // 返回地址 对应中间的行号
        private int returnAddress;


        private int variableStackSp;

        private int operantStackSp;


        StackFrame(int _returnAddress)
        {
            variableStack = new List<object>();
            operantStack = new Stack<object>();
            returnAddress = _returnAddress;
            variableStackSp = 0;
            operantStackSp = 0;
        }


        StackFrame(int _returnAddress, Object[] parameters)
        {
            foreach (Object parameter in parameters)
            {
                pushToVariableStack(parameter);
            }
        }


        public void pushToVariableStack(Object o)
        {
            variableStack.Add(o);
            variableStackSp++;
        }

        public Object popFromVariableStack()
        {
            try
            {

                Object o = variableStack.ToArray()[variableStackSp];
                variableStackSp--;
                return o;
            }
            catch (Exception e)
            {
                Console.WriteLine("弹栈异常：" + e.Message);
                return null;
            }
            
        }


        public void removeFromVariableStack(int index)
        {
            try
            {
                variableStack.RemoveAt(index);
                variableStackSp--;
            }
            catch (Exception e)
            {
                Console.WriteLine("弹栈异常：" + e.Message);
            }

        }


        public void pushToOperantStack(Object o)
        {
            operantStack.Push(o);
            operantStackSp++;
        }

        public Object popFromOperantStack()
        {
            try
            {
                operantStackSp--;
                return operantStack.Pop();
            }
            catch ( Exception e)
            {
                Console.WriteLine("弹栈异常：" + e.Message);
                return null;
            }
        }


    }
}
