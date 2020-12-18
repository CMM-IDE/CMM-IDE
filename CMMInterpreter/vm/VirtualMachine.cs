using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.vm
{
    class VirtualMachine
    {
        // 运行时栈 每个线程对应一个运行时栈
        private List<List<StackFrame>> stacks;

        VirtualMachine()
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
                        push(stack);
                        break;
                    case InstructionType.pop:
                        pop(stack);
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
                        pushv(stack);
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
            return currentStackFrame.popFromOperantStack();
        }

        public void add(StackFrame frame, int pc)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op1 + op2);
        }


        public void sub(StackFrame frame, int pc)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op1 - op2);
        }

        public void mul(StackFrame frame)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op1 * op2);
        }

        public void div(StackFrame frame)
        {
            double op1 = (double)frame.popFromOperantStack();
            double op2 = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(op1 / op2);
        }
        public void neg(StackFrame frame)
        {
            double op = (double)frame.popFromOperantStack();
            frame.pushToOperantStack(-op);
        }
        public void and(List<StackFrame> frame)
        {

        }
        public void or(List<StackFrame> frame)
        {

        }
        public void not(List<StackFrame> frame)
        {

        }
        public void push(List<StackFrame> frame)
        {
            
        }
        public void pop(List<StackFrame> frame)
        {

        }
        public void g(List<StackFrame> frame)
        {

        }
        public void l(List<StackFrame> frame)
        {

        }
        public void ge(List<StackFrame> frame)
        {

        }
        public void le(List<StackFrame> frame)
        {

        }
        public void eq(List<StackFrame> frame)
        {

        }
        public void ne(List<StackFrame> frame)
        {

        }


        public void j(List<StackFrame> frame)
        {

        }

        public void je(List<StackFrame> frame)
        {

        }

        public void jg(List<StackFrame> frame)
        {

        }

        public void jl(List<StackFrame> frame)
        {

        }
        public void jne(List<StackFrame> frame)
        {

        }
        public void call(List<StackFrame> frame)
        {

        }
        public void read(List<StackFrame> frame)
        {

        }

        public void write(List<StackFrame> frame)
        {

        }

        public void delv(List<StackFrame> frame)
        {

        }

        public void b(List<StackFrame> frame)
        {

        }

        public void cnt(List<StackFrame> frame)
        {

        }

        public void pushv(List<StackFrame> frame)
        {

        }

        public void ret(List<StackFrame> frame)
        {

        }






    }
}
