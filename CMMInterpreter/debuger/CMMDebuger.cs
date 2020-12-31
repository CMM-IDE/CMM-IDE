using CMMInterpreter.CMMException;
using CMMInterpreter.vm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CMMInterpreter.debuger
{
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
        /// 用户调试操作动作
        /// </summary>
        public event Action NeedDebug;

        /// <summary>
        /// 运行模式, -1为停止调试, 0为step into, 1为step over
        /// </summary>
        private int mode;
        private List<int> breakpoints;
        private Dictionary<int, IntermediateCodeInformation> intermediateCodeInformations;
        private Dictionary<int, IntermediateCode> savedInstructions;
        private int maxLine;

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

            // 载入中间代码
            vm.Load(codes);

            // 获取源代码-中间代码信息
            intermediateCodeInformations = vm.GetIntermediateCodeInformation();

            savedInstructions = new Dictionary<int, IntermediateCode>();

            // 获取最大行信息
            maxLine = int.MinValue;

            foreach(int key in intermediateCodeInformations.Keys)
            {
                if (key > maxLine) 
                { 
                    maxLine = key; 
                }
            }

            // 遍历断点信息
            foreach(int breakpoint in breakpoints)
            {
                // 保存断点处指令并替换为int指令
                if (intermediateCodeInformations.ContainsKey(breakpoint))
                {
                    IntermediateCodeInformation information = intermediateCodeInformations[breakpoint];
                    int address = information.Address;
                    IntermediateCode saved =  vm.ReplaceWithInt(address);
                    savedInstructions.Add(address, saved);
                }
            }
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
            vm.Run();
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
            List<FrameInformation> informations = new List<FrameInformation>();
            for(int i = 0; i < 5; i++)
            {
                FrameInformation information = new FrameInformation();
                information.Address = i;
                information.Name = "aaaaaaa";
                information.Value = "addddddd";
                informations.Add(information);
            }
            return informations;
            //return vm.GetCurrentFrame();
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

                    // 如果当前行不是最后一行
                    if (line0 != maxLine)
                    {
                        // 寻找最邻近的下一非空行
                        int nextLine = line0 + 1;
                        while (nextLine < maxLine && !intermediateCodeInformations.ContainsKey(nextLine))
                        {
                            nextLine++;
                        }

                        // 对下一行源代码对应的中间代码进行保存并替换int
                        IntermediateCodeInformation nextLineInformation = intermediateCodeInformations[nextLine];
                        int nextLineAddress = nextLineInformation.Address;
                        if (!savedInstructions.ContainsKey(nextLineAddress))
                        {
                            IntermediateCode nextLineSaved = vm.ReplaceWithInt(nextLineAddress);
                            savedInstructions.Add(nextLineAddress, nextLineSaved);
                        }
                    }

                    break;
                case 1:
                    // Step over模式
                    // TODO
                    // 根据当前行号获取保存的中间代码
                    int line1 = vm.GetLastCodeInformation().Line;
                    IntermediateCodeInformation information1 = intermediateCodeInformations[line1];
                    int address1 = information1.Address;
                    IntermediateCode saved1 = savedInstructions[address1];

                    // 单条执行中间代码
                    vm.InterpretSingleInstruction(saved1);

                    // 如果当前行不是最后一行
                    if (line1 != maxLine)
                    {
                        // 寻找最邻近的下一非空行
                        int nextLine = line1 + 1;
                        while (nextLine < maxLine && !intermediateCodeInformations.ContainsKey(nextLine))
                        {
                            nextLine++;
                        }

                        // 对下一行源代码对应的中间代码进行保存并替换int
                        IntermediateCodeInformation nextLineInformation = intermediateCodeInformations[nextLine];
                        int nextLineAddress = nextLineInformation.Address;
                        if (!savedInstructions.ContainsKey(nextLineAddress))
                        {
                            IntermediateCode nextLineSaved = vm.ReplaceWithInt(nextLineAddress);
                            savedInstructions.Add(nextLineAddress, nextLineSaved);
                        }
                    }

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
    }
}
