using CMMInterpreter.debuger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CMMInterpreter.vm
{
    /// <summary>
    /// 虚拟机
    /// </summary>
    public class VirtualMachine : IVirtualMachine
    {
        /// <summary>
        /// 运行时栈 每个线程对应一个运行时栈
        /// </summary>
        List<Stack<StackFrame>> stacks;

        /// <summary>
        /// 代码区
        /// </summary>
        List<IntermediateCode> codesArray;

        /// <summary>
        /// 当前栈帧
        /// </summary>
        StackFrame currentStackFrame;

        /// <summary>
        /// 全局栈帧
        /// </summary>
        StackFrame globalStackFrame;

        /// <summary>
        /// 当前符号表
        /// </summary>
        Dictionary<string, int> currentSymbolTable;

        /// <summary>
        /// 全局符号表
        /// </summary>
        Dictionary<string, int> globalSymbolTable;

        /// <summary>
        /// 函数符号表
        /// </summary>
        Dictionary<int, Dictionary<string,int>> functionSymbolTable;

        /// <summary>
        /// 函数信息表
        /// </summary>
        Dictionary<string, FunctionInformation> functionInformationTable;

        /// <summary>
        /// 调用栈
        /// </summary>
        Stack<StackFrame> stack;

        /// <summary>
        /// 调用栈中函数的入口地址
        /// </summary>
        Stack<int> entryStacks;

        /// <summary>
        /// 是否停止运行
        /// </summary>
        bool isStop;

        /// <summary>
        /// 调试器事件
        /// </summary>
        public event Action NeedDebug;

        /// <summary>
        /// 运行结束事件
        /// </summary>
        public event Action RunFinish;

        // 需要输入事件
        public event Action needInput;

        // 输入缓冲区
        public string buffer = null;

        /// <summary>
        /// 程序计数器
        /// </summary>
        int pc = 0;

        /// <summary>
        /// 窗口监听器
        /// </summary>
        VirtualMachineListener mainWindowListener;

        public VirtualMachine()
        {
            stacks = new List<Stack<StackFrame>>();
        }

        /// <summary>
        /// 注册窗口监听器
        /// </summary>
        /// <param name="listener">窗口监听器</param>
        public void register(VirtualMachineListener listener) {
            mainWindowListener = listener;
        }

        

        /// <summary>
        /// 解释执行代码
        /// </summary>
        /// <param name="codes">中间代码</param>
        /// <returns>是否成功运行</returns>
        public Boolean interpret(List<IntermediateCode> codes)
        {
            
            // 初始化栈
            Stack<StackFrame> stack = new Stack<StackFrame>();
            stacks.Add(stack);
            // 当前栈帧
            StackFrame currentStackFrame = new StackFrame(0);
            stack.Push(currentStackFrame);

            codesArray = codes;
            printCodes();
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
                        write(currentStackFrame, code.operant);
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

        /// <summary>
        /// 加法指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        /// <param name="pc">程序计数器</param>
        void add(StackFrame frame, int pc)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            frame.pushToOperantStack(op2 + op1);
        }

        /// <summary>
        /// 减法指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        /// <param name="pc">程序计数器</param>
        void sub(StackFrame frame, int pc)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            frame.pushToOperantStack(op2 - op1);
        }

        /// <summary>
        /// 乘法指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void mul(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            frame.pushToOperantStack(op2 * op1);
        }

        /// <summary>
        /// 除法指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void div(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            frame.pushToOperantStack(op2 / op1);
        }

        /// <summary>
        /// 取反指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void neg(StackFrame frame)
        {
            double op = Convert.ToDouble(frame.popFromOperantStack());
            frame.pushToOperantStack(-op);
        }

        /// <summary>
        /// 与指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void and(StackFrame frame)
        {
            Boolean op1 = Convert.ToBoolean(frame.popFromOperantStack());
            Boolean op2 = Convert.ToBoolean(frame.popFromOperantStack());
            frame.pushToOperantStack(op2 && op1);
        }

        /// <summary>
        /// 或指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void or(StackFrame frame)
        {
            Boolean op1 = Convert.ToBoolean(frame.popFromOperantStack());
            Boolean op2 = Convert.ToBoolean(frame.popFromOperantStack());
            frame.pushToOperantStack(op2 || op1);
        }

        /// <summary>
        /// 非指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void not(StackFrame frame)
        {
            Boolean op = Convert.ToBoolean(frame.popFromOperantStack());
            frame.pushToOperantStack(!op);
        }

        /// <summary>
        /// 压栈指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        /// <param name="operant">操作数</param>
        void push(StackFrame frame, Object operant)
        {
            frame.pushToOperantStack(operant);
        }

        /// <summary>
        /// 弹栈指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        /// <param name="operant">操作数</param>
        void pop(StackFrame frame, Object operant)
        {
            if (operant == null)
            {
                // pop不带操作数
                frame.popFromOperantStack(-1, false);
            }
            else if (operant.ToString().Contains("("))
            {
                // 指针寻址模式
                frame.popFromOperantStack(Convert.ToInt32(operant.ToString().Substring(1, operant.ToString().Length - 2)), true);
            }
            else
            {
                frame.popFromOperantStack(Convert.ToInt32(operant), false);
            }
        }

        /// <summary>
        /// 大于指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void g(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 > op1)
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }

        /// <summary>
        /// 小于指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void l(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 < op1)
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }

        /// <summary>
        /// 大于等于指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void ge(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 >= op1)
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }

        /// <summary>
        /// 小于等于指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void le(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 <= op1)
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }

        /// <summary>
        /// 等于指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void eq(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2.Equals(op1))
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }

        /// <summary>
        /// 不等于指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        void ne(StackFrame frame)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (!op2.Equals(op1))
                frame.pushToOperantStack(1.0);
            else
                frame.pushToOperantStack(0.0);
        }

        /// <summary>
        /// 无条件转指令
        /// </summary>
        /// <param name="operant">操作数</param>
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

        /// <summary>
        /// 大于则跳转指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        /// <param name="operant">操作数</param>
        void jg(StackFrame frame, Object operant)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 > op1)
            {
                pc = (int)operant-1;
            }
        }

        /// <summary>
        /// 小于则跳转指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        /// <param name="operant">操作数</param>
        void jl(StackFrame frame, Object operant)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (op2 < op1)
            {
                pc = (int)operant-1;
            }
        }

        /// <summary>
        /// 不等于则跳转指令
        /// </summary>
        /// <param name="frame">当前栈帧</param>
        /// <param name="operant">操作数</param>
        void jne(StackFrame frame, Object operant)
        {
            double op1 = Convert.ToDouble(frame.popFromOperantStack());
            double op2 = Convert.ToDouble(frame.popFromOperantStack());
            if (!op2.Equals(op1))
            {
                pc = (int)operant-1;
            }
        }

        /// <summary>
        /// 调用指令
        /// </summary>
        void call()
        {

        }

        /// <summary>
        /// 读指令
        /// </summary>
        /// <param name="frame"></param>
        void read(StackFrame frame)
        {
            // 读取一个输入压到栈顶
            mainWindowListener.write("请输入一个整数:");
            needInput?.Invoke();
            Thread.CurrentThread.Suspend();
            frame.pushToOperantStack((Object)buffer);
        }

        /// <summary>
        /// 写指令
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        void write(StackFrame frame, Object operant)
        {
            if (Convert.ToBoolean(operant))
            {
                // 操作数为1，栈顶元素出栈并打印之
                Object peek = frame.popFromOperantStack();
                mainWindowListener.write(peek);
                return;
            }
            // 否则打印换行符号
            mainWindowListener.write("\n");
            return;
            
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

        /// <summary>
        /// 中断指令
        /// </summary>
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
                information.Name = functionInformationTable.FirstOrDefault(q => q.Value.enrtyAddress == entry).Key;
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

        /// <summary>
        /// 获取源代码-中间代码信息
        /// </summary>
        /// <returns>源代码-中间代码信息</returns>
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

        /// <summary>
        /// 执行单条指令
        /// </summary>
        /// <param name="code">单条指令</param>
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
                    write(currentStackFrame, code.operant);
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
            RunFinish?.Invoke();
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
        /// 设置结束处理器
        /// </summary>
        /// <param name="handler">结束处理器</param>
        public void SetFinishHandler(Action handler)
        {
            this.RunFinish += handler;
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
        /// <param name="functionInformationTable">函数信息表</param>
        public void LoadDebugInformation(Dictionary<string, int> globalSymbolTable, Dictionary<string, FunctionInformation> functionInformationTable)
        {
            this.globalSymbolTable = globalSymbolTable;
            this.functionInformationTable = functionInformationTable;
            this.functionSymbolTable = new Dictionary<int, Dictionary<string, int>>();
            foreach(FunctionInformation information in functionInformationTable.Values)
            {
                functionSymbolTable.Add(information.enrtyAddress, information.localVariableTable);
            }
        }

        /// <summary>
        /// 获取栈帧信息
        /// </summary>
        /// <param name="stackFrame">栈帧</param>
        /// <param name="symbolTable">符号表</param>
        /// <returns>栈帧信息</returns>
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

        // 在控制台打印codes，用来debug，勿删
        public void printCodes()
        {
            int line = 0;
            foreach (IntermediateCode code in codesArray)
            {
                Console.WriteLine(line.ToString() + ":" + code.type.ToString() + " " + (code.operant == null ? "" : code.operant.ToString()));
                line++;
            }
        }
    }
}
