using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.vm
{
    public class VirtualMachine
    {
        // 运行时栈 每个线程对应一个运行时栈
        List<List<StackFrame>> stacks;

        public VirtualMachine()
        {
            stacks = new List<List<StackFrame>>();
        }

        // 解释执行代码
        public Object interpret(List<IntermediateCode> codes)
        {
            int pc = 0;
            // 初始化栈
            List<StackFrame> stack = new List<StackFrame>();
            stacks.Add(stack);
            // 当前栈帧
            StackFrame currentStackFrame = new StackFrame(0);
            stack.Add(currentStackFrame);

            foreach (IntermediateCode code in codes)
            {
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
                        g(stack);
                        break;
                    case InstructionType.l:
                        l(stack);
                        break;
                    case InstructionType.ge:
                        ge(stack);
                        break;
                    case InstructionType.le:
                        le(stack);
                        break;
                    case InstructionType.eq:
                        eq(stack);
                        break;
                    case InstructionType.ne:
                        ne(stack);
                        break;
                    case InstructionType.j:
                        j(stack);
                        break;
                    case InstructionType.je:
                        je(stack);
                        break;
                    case InstructionType.jg:
                        jg(stack);
                        break;
                    case InstructionType.jl:
                        jl(stack);
                        break;
                    case InstructionType.jne:
                        jne(stack);
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
                pc++;


            }

            
            // 运行结束销毁
            stacks.Remove(stack);
            return currentStackFrame.getVariable(0);
        }

        void add(StackFrame frame, int pc)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op1 + op2);
        }


        void sub(StackFrame frame, int pc)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op1 - op2);
        }

        void mul(StackFrame frame)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op1 * op2);
        }

        void div(StackFrame frame)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op1 / op2);
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
        void g(List<StackFrame> frame)
        {

        }
        void l(List<StackFrame> frame)
        {

        }
        void ge(List<StackFrame> frame)
        {

        }
        void le(List<StackFrame> frame)
        {

        }
        void eq(List<StackFrame> frame)
        {

        }
        void ne(List<StackFrame> frame)
        {

        }


        void j(List<StackFrame> frame)
        {

        }

        void je(List<StackFrame> frame)
        {

        }

        void jg(List<StackFrame> frame)
        {

        }

        void jl(List<StackFrame> frame)
        {

        }
        void jne(List<StackFrame> frame)
        {

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
