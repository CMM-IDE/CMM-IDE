using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.vm
{
    public class VirtualMachine
    {
        // 运行时栈 每个线程对应一个运行时栈
        List<List<StackFrame>> stacks;

        int pc = 0;

        public VirtualMachine()
        {
            stacks = new List<List<StackFrame>>();
        }


        // 解释执行代码
        public Object interpret(List<IntermediateCode> codes)
        {
            
            // 初始化栈
            List<StackFrame> stack = new List<StackFrame>();
            stacks.Add(stack);
            // 当前栈帧
            StackFrame currentStackFrame = new StackFrame(0);
            stack.Add(currentStackFrame);

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
                        and(stack);
                        break;
                    case InstructionType.or:
                        or(stack);
                        break;
                    case InstructionType.not:
                        not(stack);
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
                        call(stack);
                        break;
                    case InstructionType.read:
                        read(stack);
                        break;
                    case InstructionType.write:
                        write(stack);
                        break;
                    case InstructionType.delv:
                        delv(stack);
                        break;
                    case InstructionType.b:
                        b(stack);
                        break;
                    case InstructionType.cnt:
                        cnt(stack);
                        break;
                    case InstructionType.pushv:
                        pushv(currentStackFrame, code.operant);
                        break;
                    case InstructionType.ret:
                        ret(stack);
                        break;
                    default:

                        break;

                }


            }

            
            // 运行结束销毁
            stacks.Remove(stack);
            return currentStackFrame.getVariable(0);
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
        void and(List<StackFrame> frame)
        {

        }
        void or(List<StackFrame> frame)
        {

        }
        void not(List<StackFrame> frame)
        {

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
            pc = (int)operant;

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
                pc = (int)operant;
            }
        }

        void jg(StackFrame frame, Object operant)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 > op1)
            {
                pc = (int)operant;
            }
        }

        void jl(StackFrame frame, Object operant)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 < op1)
            {
                pc = (int)operant;
            }
        }
        void jne(StackFrame frame, Object operant)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (!op2.Equals(op1))
            {
                pc = (int)operant;
            }
        }
        void call(List<StackFrame> frame)
        {

        }
        void read(List<StackFrame> frame)
        {

        }

        void write(List<StackFrame> frame)
        {

        }

        void delv(List<StackFrame> frame)
        {

        }

        void b(List<StackFrame> frame)
        {

        }

        void cnt(List<StackFrame> frame)
        {

        }

        void pushv(StackFrame frame, Object o)
        {
            frame.pushToOperantStackFromVariable((int)o);
        }

        void ret(List<StackFrame> frame)
        {

        }






    }
}
