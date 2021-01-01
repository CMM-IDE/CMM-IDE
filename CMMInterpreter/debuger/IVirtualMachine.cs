using CMMInterpreter.vm;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.debuger
{
    /// <summary>
    /// 虚拟机接口
    /// </summary>
    public interface IVirtualMachine
    {
        /// <summary>
        /// 获取当前栈帧
        /// </summary>
        /// <returns>当前栈帧</returns>
        List<FrameInformation> GetCurrentFrame();

        /// <summary>
        /// 获取调用栈信息
        /// </summary>
        /// <returns>调用栈信息</returns>
        List<StackFrameInformation> GetStackFrames();

        /// <summary>
        /// 获取刚执行完的指令信息
        /// </summary>
        /// <returns>刚执行完的指令信息</returns>
        IntermediateCodeInformation GetLastCodeInformation();

        /// <summary>
        /// 获取源代码-中间代码信息
        /// </summary>
        /// <returns>源代码中间代码信息</returns>
        Dictionary<int, IntermediateCodeInformation> GetIntermediateCodeInformation();

        /// <summary>
        /// 执行单条指令
        /// </summary>
        /// <param name="code">指令</param>
        void InterpretSingleInstruction(IntermediateCode code);

        /// <summary>
        /// 替换中间代码为int指令
        /// </summary>
        /// <param name="address">替换地址</param>
        /// <returns>原始中间代码</returns>
        IntermediateCode ReplaceWithInt(int address);

        /// <summary>
        /// 恢复中间代码
        /// </summary>
        /// <param name="address">恢复地址</param>
        /// <param name="code">恢复代码</param>
        void Resume(int address, IntermediateCode code);

        /// <summary>
        /// 开始运行
        /// </summary>
        void Run();

        /// <summary>
        /// 停止运行
        /// </summary>
        void Stop();

        /// <summary>
        /// 装载中间代码
        /// </summary>
        void Load(List<IntermediateCode> codes);

        /// <summary>
        /// 装载调试信息
        /// </summary>
        /// <param name="globalSymbolTable">全局符号表</param>
        /// <param name="functionInformationTable">函数信息表</param>
        void LoadDebugInformation(Dictionary<string, int> globalSymbolTable, Dictionary<string, FunctionInformation> functionInformationTable);

        /// <summary>
        /// 设置调试处理器
        /// </summary>
        /// <param name="handler">调试处理器</param>
        void SetDebugHandler(Action handler);

        /// <summary>
        /// 设置读入处理器
        /// </summary>
        /// <param name="handler">读入处理器</param>
        void SetReadHandler(Action handler);

        /// <summary>
        /// 设置结束处理器
        /// </summary>
        /// <param name="handler">结束处理器</param>
        void SetFinishHandler(Action handler);

        /// <summary>
        /// 设置窗口监听器
        /// </summary>
        /// <param name="listener">监听器</param>
        void RegisterWindowListener(VirtualMachineListener listener);
    }
}
