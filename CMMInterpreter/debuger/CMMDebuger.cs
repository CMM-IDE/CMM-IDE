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
        /// 运行模式, 0为step into, 1为step over
        /// </summary>
        private int mode;
        private List<int> breakpoints;
        private Dictionary<int, IntermediateCodeInformation> intermediateCodeInformations;
        private Dictionary<int, IntermediateCode> savedInstructions;

        private IVirtualMachine vm;
        private Thread vmThread;

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
            //vm = new VirtualMachine();

            // 载入中间代码
            vm.Load(codes);

            // 获取源代码-中间代码信息
            intermediateCodeInformations = vm.GetIntermediateCodeInformation();

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
        /// 开始调试
        /// </summary>
        public void Run()
        {
            vmThread = new Thread(()=>
            {
                try
                {
                    vm.Run();
                }
                catch (RuntimeException e1)
                {
                    OutputStream?.Print("Line:" + e1.line.ToString() + " " + e1.Message);
                }
                catch (Exception e2)
                {
                    OutputStream?.Print(e2.Message);
                }
            });

            vmThread.Start();
        }

        /// <summary>
        /// 停止调试
        /// </summary>
        public void Stop()
        {
            vm.Stop();
            vmThread.Abort();
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
            // 挂起调试器进程等待用户操作
            Thread.CurrentThread.Suspend();
            NeedDebug?.Invoke();

            // 根据用户输入决定调试模式
            switch (mode)
            {
                case 0:
                    // Step into模式
                    // TODO
                    vmThread.Resume();
                    break;
                case 1:
                    // Step over模式
                    // TODO
                    vmThread.Resume();
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
