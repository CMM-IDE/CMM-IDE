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
        private Object[] variableStack;

        // 操作栈
        private Stack<Object> operantStack;

        // 返回地址 对应中间的行号
        private int returnAddress;


        private int variableStackSp;

        private int operantStackSp;


        public StackFrame(int _returnAddress)
        {
            // ＴＯＤＯ:变量站暂时设置成100大小 实际应该运行时推断大小
            variableStack = new Object[100];
            operantStack = new Stack<object>();
            returnAddress = _returnAddress;
            variableStackSp = -1;
            operantStackSp = 0;
        }


        public StackFrame(int _returnAddress, Object[] parameters)
        {
            foreach (Object parameter in parameters)
            {
                pushToVariableStack(parameter);
            }
        }

        public Object getVariable(int address)
        {
            return variableStack[address];
        }

        public void pushToVariableStack(Object o)
        {
            variableStack[++variableStackSp] = o;
        }

        public Object popFromVariableStack()
        {
            try
            {

                Object o = variableStack[variableStackSp--];
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
                for (int i = index; i < index - 1; i++)
                {
                    variableStack[i] = variableStack[i + 1];
                }
                variableStackSp--;
            }
            catch (Exception e)
            {
                Console.WriteLine("移除元素异常：" + e.Message);
            }

        }

        public void pushToOperantStackFromVariable(int variableAddress)
        {
            operantStack.Push(variableStack[variableAddress]);
            operantStackSp++;
        }

        public void pushToOperantStack(Object o)
        {
            operantStack.Push(o);
            operantStackSp++;
        }

        public Object popFromOperantStack(int index, bool isPointer)
        {
            
            try
            {
                if(index == -1)
                {
                    return operantStack.Pop();
                }
                else if (isPointer)
                {
                    // pop (1)表示把局部变量地址为1元素的值作为地址，将栈顶元素出栈并赋值给这个地址的局部变量
                    int address = Convert.ToInt32(variableStack[index]);
                    operantStackSp--;
                    Object op = operantStack.Pop();
                    // 修改局部变量的值
                    variableStack[address] = op;
                    return op;


                }
                else
                {
                    operantStackSp--;
                    Object op = operantStack.Pop();
                    // 修改局部变量的值
                    variableStack[index] = op;
                    return op;
                }
                
            }
            catch ( Exception e)
            {
                Console.WriteLine("弹栈异常：" + e.Message);
                return null;
            }
        }


        public Object popFromOperantStack()
        {
            try
            {
                operantStackSp--;
                return operantStack.Pop();

            }
            catch (Exception e)
            {
                Console.WriteLine("弹栈异常：" + e.Message);
                return null;
            }
        }

        // 获取操作栈顶元素
        public Object peek() {
            return operantStack.Peek();
        }

        public int getReturnAddress()
        {
            return returnAddress;
        }

    }
}
