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
    }
}
