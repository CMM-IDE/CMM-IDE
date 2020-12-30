using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.vm
{
    public class VirtualMachine
    {
        // 运行时栈 每个线程对应一个运行时栈
        List<Stack<StackFrame>> stacks;

        int pc = 0;

        VirtualMachineListener mainWindowListener;
        public VirtualMachine()
        {
            stacks = new List<Stack<StackFrame>>();
        }

        public void register(VirtualMachineListener listener) {
            mainWindowListener = listener;
        }

        // 解释执行代码
        public Boolean interpret(List<IntermediateCode> codes)
        {
            
            // 初始化栈
            Stack<StackFrame> stack = new Stack<StackFrame>();
            stacks.Add(stack);
            // 当前栈帧
            StackFrame currentStackFrame = new StackFrame(0);
            stack.Push(currentStackFrame);

            IntermediateCode[] codesArray = codes.ToArray();
            for (; pc < codes.Count; pc++)
            {
                IntermediateCode code = codesArray[pc];
                switch (code.type)
                {
                    case InstructionType.add:
                        add(currentStackFrame, pc);
                        break;
                    case InstructionType.sub:
                        sub(currentStackFrame, pc);
                        break;
                    case InstructionType.mul:
                        mul(currentStackFrame);
                        break;
                    case InstructionType.div:
                        div(currentStackFrame);
                        break;
                    case InstructionType.neg:
                        neg(currentStackFrame);
                        break;
                    case InstructionType.and:
                        and(currentStackFrame);
                        break;
                    case InstructionType.or:
                        or(currentStackFrame);
                        break;
                    case InstructionType.not:
                        not(currentStackFrame);
                        break;
                    case InstructionType.push:
                        push(currentStackFrame, code.operant);
                        break;
                    case InstructionType.pop:
                        pop(currentStackFrame, code.operant);
                        break;
                    case InstructionType.g:
                        g(currentStackFrame);
                        break;
                    case InstructionType.l:
                        l(currentStackFrame);
                        break;
                    case InstructionType.ge:
                        ge(currentStackFrame);
                        break;
                    case InstructionType.le:
                        le(currentStackFrame);
                        break;
                    case InstructionType.eq:
                        eq(currentStackFrame);
                        break;
                    case InstructionType.ne:
                        ne(currentStackFrame);
                        break;
                    case InstructionType.j:
                        j(code.operant);
                        break;
                    case InstructionType.je:
                        je(currentStackFrame, code.operant);
                        break;
                    case InstructionType.jg:
                        jg(currentStackFrame, code.operant);
                        break;
                    case InstructionType.jl:
                        jl(currentStackFrame, code.operant);
                        break;
                    case InstructionType.jne:
                        jne(currentStackFrame, code.operant);
                        break;
                    case InstructionType.call:
                        // 创建新的函数栈帧 传入pc
                        StackFrame newStackFrame = new StackFrame(pc);
                        // 首先获取参数个数
                        int paraNum = (int)currentStackFrame.popFromOperantStack();
                        // 向新的栈帧中压入参数
                        Stack<Object> tmp = new Stack<Object>();
                        for (int i = 0; i < paraNum; i++) {
                            tmp.Push(currentStackFrame.popFromOperantStack());
                        }
                        for (int i = 0; i < paraNum; i++) {
                            newStackFrame.pushToVariableStack(tmp.Pop());
                        }
                        // 压入新栈
                        stack.Push(newStackFrame);
                        currentStackFrame = newStackFrame;
                        pc = (int)code.operant - 1;
                        break;
                    case InstructionType.read:
                        read(currentStackFrame);
                        break;
                    case InstructionType.write:
                        write(currentStackFrame);
                        break;
                    case InstructionType.delv:
                        delv(currentStackFrame);
                        break;
                    case InstructionType.b:
                        b(currentStackFrame);
                        break;
                    case InstructionType.cnt:
                        cnt(currentStackFrame);
                        break;
                    case InstructionType.pushv:
                        pushv(currentStackFrame, code.operant);
                        break;
                    case InstructionType.i:
                        break;
                    case InstructionType.ret:
                        pc = currentStackFrame.getReturnAddress();
                        int flag = (int)currentStackFrame.popFromOperantStack();
                        
                        Object returnValue = currentStackFrame.peek();
                        // 函数返回 移除当前栈帧
                        stack.Pop();
                        if (flag == 1) {
                            // 将函数返回值压入到调用者的栈帧中
                            stack.Peek().pushToOperantStack(returnValue);
                        }
                        currentStackFrame = stack.Peek();

                        break;
                    default:

                        break;

                }


            }

            
            // 运行结束销毁
            stacks.Remove(stack);
            return true;
        }

        void add(StackFrame frame, int pc)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op2 + op1);
        }


        void sub(StackFrame frame, int pc)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op2 - op1);
        }

        void mul(StackFrame frame)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op2 * op1);
        }

        void div(StackFrame frame)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op2 / op1);
        }
        void neg(StackFrame frame)
        {
            double op = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(-op);
        }
        void and(StackFrame frame)
        {
            Boolean op1 = (Boolean)frame.popFromOperantStack();
            Boolean op2 = (Boolean)frame.popFromOperantStack();
            frame.pushToOperantStack(op2 && op1);
        }
        void or(StackFrame frame)
        {
            Boolean op1 = (Boolean)frame.popFromOperantStack();
            Boolean op2 = (Boolean)frame.popFromOperantStack();
            frame.pushToOperantStack(op2 || op1);
        }
        void not(StackFrame frame)
        {
            Boolean op = (Boolean)frame.popFromOperantStack();
            frame.pushToOperantStack(!op);
        }
        void push(StackFrame frame, Object operant)
        {
            frame.pushToOperantStack(operant);
        }
        void pop(StackFrame frame, Object operant)
        {
            
            if (operant.ToString().Contains("("))
            {
                // 指针寻址模式
                frame.popFromOperantStack(Convert.ToInt32(operant.ToString().Substring(1, operant.ToString().Length - 2)), true);
            }
            else
            {
                frame.popFromOperantStack((int)operant, false);
            }
        }
        void g(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 > op1)
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }
        void l(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 < op1)
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }
        void ge(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 < op1)
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }
        void le(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 <= op1)
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }
        void eq(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2.Equals(op1))
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }
        void ne(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (!op2.Equals(op1))
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }


        void j(Object operant)
        {
            pc = (int)operant-1;

        }

        /**
         * TODO：
         * 
         * 目前几个跳转指令都是将栈顶元素弹出来然后比较 不用放缓
         * 之后要改掉
         * 
         */
        void je(StackFrame frame, Object operant)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op1.Equals(op2))
            {
                pc = (int)operant-1;
            }
        }

        void jg(StackFrame frame, Object operant)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 > op1)
            {
                pc = (int)operant-1;
            }
        }

        void jl(StackFrame frame, Object operant)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 < op1)
            {
                pc = (int)operant-1;
            }
        }
        void jne(StackFrame frame, Object operant)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (!op2.Equals(op1))
            {
                pc = (int)operant-1;
            }
        }
        void call()
        {

        }
        void read(StackFrame frame)
        {
            // 读取一个输入压到栈顶
            // 
        }

        Object write(StackFrame frame)
        {
            // 打印栈顶元素
            Console.WriteLine(frame.peek());
            mainWindowListener.write(frame.peek());
            return frame.peek();
        }

        void delv(StackFrame frame)
        {

        }

        void b(StackFrame frame)
        {

        }

        void cnt(StackFrame frame)
        {

        }

        void pushv(StackFrame frame, Object o)
        {
            // pushv <index>，将index处的变量压入栈中，有两种形式
            // 1、形如 pushv 1 表示将局部变量表中地址为1的元素push到操作战中
            // 2、形如 pushv 无操作数，则将栈顶元素出栈作为操作数。
            if (o != null) {
                frame.pushToOperantStackFromVariable((int)o);
            }
            else {
                int addr = (int)frame.popFromOperantStack();
                frame.pushToOperantStackFromVariable(addr);
            }
            
        }

        void ret(StackFrame frame)
        {
        }






    }
}
