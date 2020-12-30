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
    public enum InstructionType
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

        // 所有比较指令都是将栈顶下一个元素和栈顶元素出栈进行比较，并将结果0/1压入栈顶
        g,      // 大于
        l,      // 小于
        ge,     // 大于等于
        le,     // 小于等于
        eq,     // 等于
        ne,     // 不等于

        j,      // 强制跳转
        je,     // 等于时跳转
        jg,     // 大于时跳转
        jl,     // 小于时跳转
        jne,    // 不相等时跳转
        call,   // 函数调用 格式为 call <address> 其中address表示调用的函数的中间代码的起始地址
        read,   // 读取一个输入压到栈顶
        write,  // 打印操作栈栈顶元素
        delv,   // delVar <addr> 删除局部变量表中从addr开始的所有变量
        b,      // break语句，回填之后会被更换为jmp语句 
        cnt,    // continue语句，回填之后会被更换为jmp语句
        pushv,  // pushv <index>，将index处的变量压入栈中
        inc,    // 自增指令，若没有操作数栈顶元素+1 若有操作数栈顶元素+操作数
        i,      // 调试用的中断指令
        ret,    // 函数返回 返回后将函数返回值压到上一个函数的stack frame的操作栈中
        nop     // 空指令 什么都不做

    }
}
