using CMMInterpreter.debuger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMMInterpreter.vm
{
    public class VirtualMachine : IVirtualMachine
    {
        // 运行时栈 每个线程对应一个运行时栈
        List<Stack<StackFrame>> stacks;

        // 代码区
        List<IntermediateCode> codesArray;

        // 当前栈帧
        StackFrame currentStackFrame;

        // 全局栈帧
        StackFrame globalStackFrame;

        // 当前符号表
        Dictionary<string, int> currentSymbolTable;

        /// <summary>
        /// 全局符号表
        /// </summary>
        Dictionary<string, int> globalSymbolTable;

        /// <summary>
        /// 函数符号表
        /// </summary>
        Dictionary<int, Dictionary<string, int>> functionSymbolTable;

        /// <summary>
        /// 函数入口表
        /// </summary>
        Dictionary<string, int> functionAddressTable;

        Stack<StackFrame> stack;

        Stack<int> entryStacks;

        bool isStop;
        
        // 调试器动作
        public event Action NeedDebug;

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

            codesArray = codes;
            
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
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
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
            Boolean op1 = Convert.ToBoolean(frame.popFromOperantStack());
            Boolean op2 = Convert.ToBoolean(frame.popFromOperantStack());
            frame.pushToOperantStack(op2 && op1);
        }
        void or(StackFrame frame)
        {
            Boolean op1 = Convert.ToBoolean(frame.popFromOperantStack());
            Boolean op2 = Convert.ToBoolean(frame.popFromOperantStack());
            frame.pushToOperantStack(op2 || op1);
        }
        void not(StackFrame frame)
        {
            Boolean op = Convert.ToBoolean(frame.popFromOperantStack());
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
                frame.popFromOperantStack(Convert.ToInt32(operant), false);
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
                frame.pushToOperantStackFromVariable(Convert.ToInt32(o));
            }
            else {
                int addr = Convert.ToInt32(frame.popFromOperantStack());
                frame.pushToOperantStackFromVariable(addr);
            }
            
        }

        void i()
        {
            // 调用调试器处理
            NeedDebug?.Invoke();
        }

        void ret(StackFrame frame)
        {

        }

        /// <summary>
        /// 获取当前栈帧
        /// </summary>
        /// <returns>当前栈帧</returns>
        public List<FrameInformation> GetCurrentFrame()
        {
            return GetFrame(currentStackFrame,currentSymbolTable);
        }

        /// <summary>
        /// 获取调用栈信息
        /// </summary>
        /// <returns>调用栈信息</returns>
        public List<StackFrameInformation> GetStackFrames()
        {
            List<StackFrameInformation> informations = new List<StackFrameInformation>();
            // 临时变量保存调用栈
            Stack<StackFrame> frameClone= new Stack<StackFrame>();

            // 遍历调用栈
            foreach (int entry in entryStacks)
            {
                StackFrame currentStackFrame = stack.Pop();
                frameClone.Push(currentStackFrame);
                StackFrameInformation information = new StackFrameInformation();
                information.Name = functionAddressTable.FirstOrDefault(q => q.Value == entry).Key;
                information.Line = codesArray[entry].lineNum;
                information.Frame = GetFrame(currentStackFrame, functionSymbolTable[entry]);
                informations.Add(information);
            }

            // 全局栈帧信息
            StackFrameInformation informationGlobal = new StackFrameInformation();
            informationGlobal.Name = "global";
            informationGlobal.Line = 0;
            informationGlobal.Frame = GetFrame(globalStackFrame, globalSymbolTable);
            informations.Add(informationGlobal);

            // 恢复调用栈
            foreach (StackFrame frame in frameClone)
            {
                stack.Push(frame);
            }
            return informations;
        }

        /// <summary>
        /// 获取刚执行完的指令信息
        /// </summary>
        /// <returns>指令信息</returns>
        public IntermediateCodeInformation GetLastCodeInformation()
        {
            IntermediateCodeInformation lastInformation= new IntermediateCodeInformation();
            lastInformation.Address = pc;
            lastInformation.Line = codesArray[pc].lineNum;
            return lastInformation;
        }

        public Dictionary<int, IntermediateCodeInformation> GetIntermediateCodeInformation()
        {
            int length = codesArray.Count;
            Dictionary<int, IntermediateCodeInformation> informations = new Dictionary<int, IntermediateCodeInformation>();
            // 遍历中间代码
            for (int i = 0; i < length; i++)
            {
                IntermediateCode current = codesArray[i];
                int currentLine = current.lineNum;
                IntermediateCodeInformation currentInformation;

                // 判断该中间指令对应源代码的行是否已经加入信息表中
                if(informations.TryGetValue(currentLine, out currentInformation))
                {
                    // 该行含函数调用则更新函数入口地址列表
                    if (current.type == InstructionType.call)
                    {
                        currentInformation.IsFunctionCall = true;
                        currentInformation.FuncionEntryList.AddLast((int)current.operant);
                        informations[currentLine] = currentInformation;
                    }
                }
                else
                {
                    // 该行源代码首条中间指令
                    currentInformation = new IntermediateCodeInformation();
                    currentInformation.Address = i;
                    currentInformation.Line = currentLine;
                    currentInformation.IsFunctionCall = false;
                    currentInformation.FuncionEntryList = new LinkedList<int>();
                    informations.Add(currentLine, currentInformation);
                }

                
            }
            return informations;
        }

        public void InterpretSingleInstruction(IntermediateCode code)
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
                    for (int i = 0; i < paraNum; i++)
                    {
                        tmp.Push(currentStackFrame.popFromOperantStack());
                    }
                    for (int i = 0; i < paraNum; i++)
                    {
                        newStackFrame.pushToVariableStack(tmp.Pop());
                    }
                    // 压入新栈
                    stack.Push(newStackFrame);
                    entryStacks.Push((int)code.operant);
                    currentStackFrame = newStackFrame;
                    currentSymbolTable = functionSymbolTable[(int)code.operant];
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
                    i();
                    break;
                case InstructionType.ret:
                    pc = currentStackFrame.getReturnAddress();
                    int flag = (int)currentStackFrame.popFromOperantStack();

                    Object returnValue = currentStackFrame.peek();
                    // 函数返回 移除当前栈帧
                    stack.Pop();
                    entryStacks.Pop();
                    if (flag == 1)
                    {
                        // 将函数返回值压入到调用者的栈帧中
                        stack.Peek().pushToOperantStack(returnValue);
                    }
                    currentStackFrame = stack.Peek();
                    currentSymbolTable = globalSymbolTable;
                    break;
                default:

                    break;

            }
        }

        /// <summary>
        /// 替换中间代码为int
        /// </summary>
        /// <param name="address">中间代码地址</param>
        /// <returns>被替换掉的中间代码</returns>
        public IntermediateCode ReplaceWithInt(int address)
        {
            IntermediateCode saved = codesArray[address];
            codesArray[address] = new IntermediateCode(InstructionType.i,saved.lineNum);
            return saved;
        }

        /// <summary>
        /// 恢复中间代码
        /// </summary>
        /// <param name="address">中间代码地址</param>
        /// <param name="code">中间代码</param>
        public void Resume(int address, IntermediateCode code)
        {
            codesArray[address] = code;
        }

        /// <summary>
        /// 开始调试
        /// </summary>
        public void Run()
        {
            // 初始化栈
            stack = new Stack<StackFrame>();
            entryStacks = new Stack<int>();
            stacks.Add(stack);

            // 当前栈帧
            currentStackFrame = new StackFrame(0);
            globalStackFrame = currentStackFrame;
            stack.Push(currentStackFrame);

            // 代码区长度
            int length = codesArray.Count;

            // 初始化程序计数器
            pc = 0;

            isStop = false;

            // 设置当前符号表
            currentSymbolTable = globalSymbolTable;

            // 执行中间代码
            while (pc < length && !isStop)
            {
                IntermediateCode code = codesArray[pc];
                InterpretSingleInstruction(code);
                pc++;
            }

            // 停止运行
            // 运行结束销毁
            stacks.Remove(stack);
        }

        /// <summary>
        /// 停止运行
        /// </summary>
        public void Stop()
        {
            isStop = true;
        }

        /// <summary>
        /// 装载中间代码
        /// </summary>
        /// <param name="codes">中间代码</param>
        public void Load(List<IntermediateCode> codes)
        {
            this.codesArray = codes;
        }

        /// <summary>
        /// 设置调试处理器
        /// </summary>
        /// <param name="handler">调试处理器</param>
        void IVirtualMachine.SetDebugHandler(Action handler)
        {
            NeedDebug += handler;
        }

        /// <summary>
        /// 设置读入处理器
        /// </summary>
        /// <param name="handler">读入处理器</param>
        void IVirtualMachine.SetReadHandler(Action handler)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 注册窗口监听器
        /// </summary>
        /// <param name="listener">监听器</param>
        public void RegisterWindowListener(VirtualMachineListener listener)
        {
            register(listener);
        }

        /// <summary>
        /// 装载调试信息
        /// </summary>
        /// <param name="globalSymbolTable">全局符号表</param>
        /// <param name="functionSymbolTable">函数符号表</param>
        /// <param name="functionAddressTable">函数入口表</param>
        public void LoadDebugInformation(Dictionary<string, int> globalSymbolTable, Dictionary<int, Dictionary<string, int>> functionSymbolTable, Dictionary<string,int> functionAddressTable)
        {
            this.globalSymbolTable = globalSymbolTable;
            this.functionSymbolTable = functionSymbolTable;
            this.functionAddressTable = functionAddressTable;
        }

        private List<FrameInformation> GetFrame(StackFrame stackFrame, Dictionary<string, int> symbolTable)
        {
            List<FrameInformation> informations = new List<FrameInformation>();
            // 遍历符号表填充栈帧信息
            foreach (KeyValuePair<string, int> item in symbolTable)
            {
                string name = item.Key;
                int address = item.Value;
                Object value = stackFrame.getVariable(address);
                if (value != null)
                {
                    FrameInformation information = new FrameInformation();
                    information.Name = name;
                    information.Address = address;
                    information.Value = value.ToString();
                    informations.Add(information);
                }
            }
            return informations;
        }
    }
}
