using CMMInterpreter.CMMException;
using CMMInterpreter.vm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CMMInterpreter.debuger
{
    /// <summary>
    /// 调试器
    /// </summary>
    public class CMMDebuger
    {
        /// <summary>
        /// 输出流
        /// </summary>
        public IOutputStream OutputStream { get; set; }

        /// <summary>
        /// 输入缓冲
        /// </summary>
        public string Buffer { get; set; }

        /// <summary>
        /// 用户调试操作事件
        /// </summary>
        public event Action NeedDebug;

        /// <summary>
        /// 调试结束事件
        /// </summary>
        public event Action DebugFinish;

        /// <summary>
        /// 运行模式, -1为停止调试, 0为step into, 1为step over, 2为continue
        /// </summary>
        private int mode;

        /// <summary>
        /// 断点列表
        /// </summary>
        private List<int> breakpoints;

        /// <summary>
        /// 中间代码
        /// </summary>
        private List<IntermediateCode> codesArray;

        /// <summary>
        /// 函数信息表
        /// </summary>
        private Dictionary<string, FunctionInformation> functionInformationTable;

        /// <summary>
        /// 下一行源代码缓存
        /// </summary>
        private Dictionary<int, List<int>> nextLines;

        /// <summary>
        /// 中间代码信息表
        /// </summary>
        private Dictionary<int, IntermediateCodeInformation> intermediateCodeInformations;

        /// <summary>
        /// 中间代码保存表
        /// </summary>
        private Dictionary<int, IntermediateCode> savedInstructions;

        /// <summary>
        /// 中间代码长度
        /// </summary>
        private int maxLine;

        /// <summary>
        /// 虚拟机
        /// </summary>
        private IVirtualMachine vm;

        /// <summary>
        /// 初始化调试器
        /// </summary>
        /// <param name="codes">中间代码</param>
        /// <param name="breakpointList">断点列表</param>
        public CMMDebuger(List<IntermediateCode> codes, List<int> breakpointList)
        {
            // 保存断点信息
            breakpoints = breakpointList;

            // VM初始化
            vm = new VirtualMachine();

            // 设置中断处理器
            vm.SetDebugHandler(HandleInterrupt);

            // 设置结束处理器
            vm.SetFinishHandler(HandlerFinish);

            // 载入中间代码
            vm.Load(codes);
            codesArray = codes;
        }

        /// <summary>
        /// 设置窗口监听器
        /// </summary>
        /// <param name="listener">监听器</param>
        public void setListener(VirtualMachineListener listener)
        {
            vm.RegisterWindowListener(listener);
        }

        /// <summary>
        /// 开始调试
        /// </summary>
        public void Run()
        {
            // 获取源代码-中间代码信息
            intermediateCodeInformations = GetIntermediateCodeInformation();

            nextLines = new Dictionary<int, List<int>>();

            savedInstructions = new Dictionary<int, IntermediateCode>();

            // 获取最大行信息
            maxLine = int.MinValue;

            foreach (int key in intermediateCodeInformations.Keys)
            {
                if (key > maxLine)
                {
                    maxLine = key;
                }
            }

            // 遍历断点信息
            foreach (int breakpoint in breakpoints)
            {
                // 保存断点处指令并替换为int指令
                if (intermediateCodeInformations.ContainsKey(breakpoint))
                {
                    IntermediateCodeInformation information = intermediateCodeInformations[breakpoint];
                    int address = information.Address;
                    IntermediateCode saved = vm.ReplaceWithInt(address);
                    savedInstructions.Add(address, saved);
                }
            }
            vm.Run();
        }

        /// <summary>
        /// 装载调试信息
        /// </summary>
        /// <param name="globalSymbolTable">全局符号表</param>
        /// <param name="functionInformationTable">函数信息表</param>
        public void LoadDebugInformation(Dictionary<string, int> globalSymbolTable, Dictionary<string, FunctionInformation> functionInformationTable)
        {
            this.functionInformationTable=functionInformationTable;
            vm.LoadDebugInformation(globalSymbolTable, functionInformationTable);
        }

        /// <summary>
        /// 停止调试
        /// </summary>
        public void Stop()
        {
            mode = -1;
        }

        /// <summary>
        /// 增加断点
        /// </summary>
        /// <param name="breakpoint">断点行号</param>
        public void AddBreakpoint(int breakpoint)
        {
            // 寻找插入位置
            int index = 0,length=breakpoints.Count;
            while (index<length && breakpoints[index] < breakpoint)
            {
                index++;
            }
            breakpoints.Insert(index, breakpoint);

            // 保存并替换中间指令为int
            IntermediateCodeInformation information = intermediateCodeInformations[breakpoint];
            int address = information.Address;
            IntermediateCode saved = vm.ReplaceWithInt(address);
            savedInstructions.Add(address, saved);
        }

        /// <summary>
        /// 移除断点
        /// </summary>
        /// <param name="breakpoint">断点行号</param>
        public void RemoveBreakpoint(int breakpoint)
        {
            // 移除断点
            breakpoints.Remove(breakpoint);

            // 恢复原始中间指令
            IntermediateCodeInformation information = intermediateCodeInformations[breakpoint];
            int address = information.Address;
            IntermediateCode saved = savedInstructions[address];
            savedInstructions.Remove(address);
            vm.Resume(address, saved);
        }

        /// <summary>
        /// 获取断点处行号
        /// </summary>
        /// <returns>行号</returns>
        public int GetCurrentLine()
        {
            return vm.GetLastCodeInformation().Line;
        }

        /// <summary>
        /// 获取当前栈帧
        /// </summary>
        /// <returns>栈帧</returns>
        public List<FrameInformation> GetCurrentFrame()
        {
            return vm.GetCurrentFrame();
        }

        /// <summary>
        /// 获取调用栈信息
        /// </summary>
        /// <returns>调用栈</returns>
        public List<StackFrameInformation> GetStackFrames()
        {
            return vm.GetStackFrames();
        }

        /// <summary>
        /// 逐行运行
        /// </summary>
        public void StepInto()
        {
            mode = 0;
        }

        /// <summary>
        /// 逐过程运行
        /// </summary>
        public void StepOver()
        {
            mode = 1;
        }

        /// <summary>
        /// 继续运行直到下一个断点
        /// </summary>
        public void Continue()
        {
            mode = 2;
        }

        /// <summary>
        /// 处理中断
        /// </summary>
        private void HandleInterrupt()
        {
            NeedDebug?.Invoke();
            // 挂起调试器进程等待用户操作
            Thread.CurrentThread.Suspend();

            // 根据用户输入决定调试模式
            switch (mode)
            {
                case -1:
                    vm.Stop();
                    break;
                case 0:
                    // Step into模式
                    // 根据当前行号获取保存的中间代码
                    int line0 = vm.GetLastCodeInformation().Line;
                    IntermediateCodeInformation information0 = intermediateCodeInformations[line0];
                    int address0 = information0.Address;
                    IntermediateCode saved = savedInstructions[address0];
                    

                    // 查询当前行是否为函数调用
                    if (information0.IsFunctionCall)
                    {
                        // 对函数入口表中的每一个函数入口指令进行保存并替换int指令
                        foreach (int functionEntry in information0.FuncionEntryList)
                        {
                            if (!savedInstructions.ContainsKey(functionEntry))
                            {
                                IntermediateCode function = vm.ReplaceWithInt(functionEntry);
                                savedInstructions.Add(functionEntry, function);
                            }
                        }
                    }

                    // 单条执行中间代码
                    vm.InterpretSingleInstruction(saved);

                    // 对下一行源代码对应的中间代码进行保存并替换int
                    ReplaceNextLine(line0);

                    break;
                case 1:
                    // Step over模式
                    // 根据当前行号获取保存的中间代码
                    int line1 = vm.GetLastCodeInformation().Line;
                    IntermediateCodeInformation information1 = intermediateCodeInformations[line1];
                    int address1 = information1.Address;
                    IntermediateCode saved1 = savedInstructions[address1];

                    // 单条执行中间代码
                    vm.InterpretSingleInstruction(saved1);

                    // 对下一行源代码对应的中间代码进行保存并替换int
                    ReplaceNextLine(line1);

                    break;
                case 2:
                    // Continue模式
                    // 根据当前行号获取保存的中间代码
                    int line2 = vm.GetLastCodeInformation().Line;
                    IntermediateCodeInformation information2 = intermediateCodeInformations[line2];
                    int address2 = information2.Address;
                    IntermediateCode saved2 = savedInstructions[address2];

                    // 单条执行中间代码
                    vm.InterpretSingleInstruction(saved2);

                    break;
                default:
                    // 其他模式
                    break;
            }
        }

        /// <summary>
        /// 处理输入
        /// </summary>
        private void HandleRead()
        {

        }

        private void HandlerFinish()
        {
            DebugFinish?.Invoke();
        }

        private void ReplaceNextLine(int line)
        {
            // 如果当前行不是最后一行
            if (line != maxLine)
            {
                // 判断是否已缓存该行的下一行
                if (!nextLines.ContainsKey(line))
                {
                    // 判断是否为函数体的行
                    int functionEntry = -1;
                    foreach (FunctionInformation information in functionInformationTable.Values)
                    {
                        if (information.enrtyAddress <= intermediateCodeInformations[line].Address&&information.outAddress >= intermediateCodeInformations[line].Address)
                        {
                            functionEntry = information.enrtyAddress;
                            break;
                        }
                    }

                    if (functionEntry != -1)
                    {
                        // 若是函数体中的断点则判断是否被调用
                        foreach (KeyValuePair<int, IntermediateCodeInformation> information in intermediateCodeInformations)
                        {
                            IntermediateCodeInformation currentInforamtion = information.Value;
                            if (currentInforamtion.IsFunctionCall && currentInforamtion.FuncionEntryList.Contains(functionEntry))
                            {
                                // 若被调用则获取调用语句的下一行
                                // 同时保存该行的下一行
                                if (information.Key != maxLine)
                                {
                                    int nextLine = GetNextLine(information.Key);
                                    while (intermediateCodeInformations[nextLine].IsFunctionBody&&nextLine<maxLine)
                                    {
                                        nextLine = GetNextLine(nextLine);
                                    }
                                    nextLines.Add(line, new List<int> { GetNextLine(line), nextLine });
                                    break;
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 非函数体中的直接获取下一行
                        int nextLine = GetNextLine(line);
                        while (intermediateCodeInformations[nextLine].IsFunctionBody && nextLine < maxLine)
                        {
                            nextLine = GetNextLine(nextLine);
                        }
                        nextLines.Add(line, new List<int> { nextLine });
                    }
                }
                
                
                foreach(int nextLine in nextLines[line])
                {
                    // 对下一行源代码对应的中间代码进行保存并替换int
                    IntermediateCodeInformation nextLineInformation = intermediateCodeInformations[nextLine];
                    int nextLineAddress = nextLineInformation.Address;
                    if (!savedInstructions.ContainsKey(nextLineAddress))
                    {
                        IntermediateCode nextLineSaved = vm.ReplaceWithInt(nextLineAddress);
                        savedInstructions.Add(nextLineAddress, nextLineSaved);
                    }
                }
            }
        }

        private int GetNextLine(int line)
        {
            // 寻找最邻近的下一非空行
            int nextLine = line + 1;
            while (nextLine < maxLine && !intermediateCodeInformations.ContainsKey(nextLine))
            {
                nextLine++;
            }
            return nextLine;
        }

        /// <summary>
        /// 获取源代码-中间代码信息
        /// </summary>
        /// <returns>源代码中间代码信息</returns>
        private Dictionary<int, IntermediateCodeInformation> GetIntermediateCodeInformation()
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
                if (informations.TryGetValue(currentLine, out currentInformation))
                {
                    // 该行含函数调用则更新函数入口地址列表
                    if (current.type == InstructionType.call)
                    {
                        currentInformation.IsFunctionCall = true;
                        currentInformation.FuncionEntryList.AddLast((int)current.operant);
                        currentInformation.IsFunctionBody = false;
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
                    currentInformation.IsFunctionBody = false;
                    informations.Add(currentLine, currentInformation);
                }


            }
            foreach(FunctionInformation information in functionInformationTable.Values)
            {
                int line = codesArray[information.enrtyAddress].lineNum;
                informations[line].IsFunctionBody = true;
            }
            return informations;
        }
    }
}
