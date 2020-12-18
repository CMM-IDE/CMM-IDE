using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.vm
{
    /**
     * @author 谭惠日 杨翔
     * @since 12-07-2020
     * 指令类型
     * 
     */
    enum InstructionType
    {

        add,    // 加
        sub,    // 减
        mul,    // 乘
        div,    // 除
        neg,    // 取相反数
        and,    // 与
        or,     // 或
        not,    // 非
        push,   // 入栈
        pop,    // 出栈
        j,      // 强制跳转
        je,     // 等于时跳转
        jg,     // 大于时跳转
        jl,     // 小于时跳转
        jne,    // 不相等时跳转
        call,   // 函数调用 格式为 call <address> 其中address表示调用的函数的中间代码的起始地址
        read,   // 读取一个输入压到栈顶
        write,  // 打印操作栈栈顶元素
        delVar, // delVar <addr> 删除局部变量表中从addr开始的所有变量
        brea,   // break语句，回填之后会被更换为jmp语句 
        conti,  //continue语句，回填之后会被更换为jmp语句
        ret     // 函数返回 返回后将函数返回值压到上一个函数的stack frame的操作栈中
        

    }
}
