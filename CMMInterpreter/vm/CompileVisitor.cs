using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.vm
{
    /**
     * @author 谭惠日 杨翔
     * @since 12-07-2020
     * Visitor用于生成中间代码
     * 
     */
    class CompileVisitor : CMMBaseVisitor<object>
    {
        // 函数地址表
        Dictionary<string, int> functionAddressTable;


        /**
         * 读取identifier的text，然后从函数地址表中读取该函数的中间代码起始地址
         * 将expression list压入调用函数的stack frame的参数列表栈中（如果有的话）
         * 并且插入一条call指令
         */
        public override object VisitCallStatement([NotNull] CMMParser.CallStatementContext context)
        {
            
            return base.VisitCallStatement(context);

        }

        /**
         * 处理生成函数的中间代码时
         * 在函数的第一行地址前加入j指令跳转到函数的最后一行地址之后
         * 
         */
        public override object VisitFunctionDeclaration([NotNull] CMMParser.FunctionDeclarationContext context)
        {
            return base.VisitFunctionDeclaration(context);
        }


        /**
         * 将expression的值求出来并压到栈中
         * 具体的值要看children的情况
         * 
         */
        public override object VisitExpression([NotNull] CMMParser.ExpressionContext context)
        {
            return base.VisitExpression(context);
        }


        /**
         * 判断子表达式的值并压入栈中
         * 
         * 
         */
        public override object VisitBoolExpression([NotNull] CMMParser.BoolExpressionContext context)
        {
            return base.VisitBoolExpression(context);
        }

        /**
         * 
         */
        public override object VisitAdditiveExpression([NotNull] CMMParser.AdditiveExpressionContext context)
        {
            return base.VisitAdditiveExpression(context);
        }


    }
}
