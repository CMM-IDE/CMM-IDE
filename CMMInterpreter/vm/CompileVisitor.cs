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

        // 当前局部变量表
        Dictionary<string, int> curLocalVariablesTable;


        // 指令集合
        List<IntermediateCode> codes = new List<IntermediateCode>();

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


        /*
         
         用于initializer的编译，分成两种情况。
        一种情况是直接对已经定义的普通变量进行赋值，另外一种情况是对数组的操作，没看出来。a[1]=[1],[1]
        前提：Expression Visit的结果已经被push到操作栈中
         */
        public override object VisitInitializer([NotNull] CMMParser.InitializerContext context)
        {
            
            if (context.ChildCount == 3)
            {

                // 直接对已经定义的普通变量进行赋值
                if (curLocalVariablesTable.ContainsKey(context.GetChild(0).GetText()))
                {
                    throw new VariableNotFountException();
                }
                // 待赋值的变量地址
                curLocalVariablesTable.TryGetValue(context.GetChild(0).GetText(), out int addr);
                Visit(context.GetChild(2));
                // 这里相当于把栈顶的元素pop到局部变量区中addr的位置。
                IntermediateCode code = new IntermediateCode(addr, InstructionType.pop);
                codes.Add(code);

            }
            return null;
        }

        /*
         访问参数列表，为了方便对参数进行管理，直接将参数列表保存在局部变量区中。而且由于参数列表是函数中最先被处理的变量，因此可以摆放在局部变量区的最开始位置
         */
        public override object VisitParameterList([NotNull] CMMParser.ParameterListContext context)
        {
            // 把参数加入到局部变量表中。运行时在遇到call指令的时候将要将参数按照顺序加入局部变更量表中。
            curLocalVariablesTable.Add(context.GetChild(1).GetText(), curLocalVariablesTable.Count);


            return base.VisitParameterList(context);
        }

        /*
         处理赋值操作，
        前提：leftValue将待赋值的变量在局部变更量表中的索引已经压入栈中，expression处理的结果已经压入操作栈中
         */
        public override object VisitAssignment([NotNull] CMMParser.AssignmentContext context)
        {
            Visit(context.leftValue());
            // 把leftVal的值放到局部变更量表Count位置上
            IntermediateCode code0 = new IntermediateCode(curLocalVariablesTable.Count, InstructionType.pop);

            // 把Expression的值压入栈中
            Visit(context.expression());
            //将Experession的值放入Count中
            IntermediateCode code1 = new IntermediateCode("(" + curLocalVariablesTable.Count+")", InstructionType.pop);
            // 这个过程中，局部变量表并没有增加新的元素，所以curLocalVariable并没有Add操作。
            codes.Add(code0);
            codes.Add(code1);


            return null;
        }

        /*
         处理leftValue,需要将变量对应在局部变量区的位置压入栈中
        add的结果是保存在栈上
         */
        public override object VisitLeftValue([NotNull] CMMParser.LeftValueContext context)
        {
            if(context.ChildCount == 1)
            {
                //只定义成了identifier,将identifier对应的索引地址压栈
                if (!curLocalVariablesTable.ContainsKey(context.GetChild(0).GetText())) {
                    throw new VariableNotFountException(context.GetChild(0).GetText(), context);
                }
                curLocalVariablesTable.TryGetValue(context.GetChild(0).GetText(), out int addr);
                IntermediateCode code = new IntermediateCode(addr, InstructionType.push);
                codes.Add(code);
            }
            else
            {
                // 定义成数组的那种形式a[i]
                if (!curLocalVariablesTable.ContainsKey(context.GetChild(0).GetText()))
                {
                    throw new VariableNotFountException(context.GetChild(0).GetText(), context);
                }
                curLocalVariablesTable.TryGetValue(context.GetChild(0).GetText(), out int addr);
                // 把基地址也push到操作栈上
                IntermediateCode code0 = new IntermediateCode(addr, InstructionType.push);
                Visit(context.expression());
                // 使用add指令，获得addr+expression，add指令使得这个值已经压入栈中
                IntermediateCode code1 = new IntermediateCode(InstructionType.add);
                codes.Add(code0);
                codes.Add(code1);
            }
            return null;
        }

        /*
         
         访问while表达式
         */
        public override object VisitWhileStatement([NotNull] CMMParser.WhileStatementContext context)
        {
            /*
             注意：这里的0是constant，需要补充！！！！！！！！！
            如果expression的操作结果是false，就在栈中压入0，否则压入1.
             
             */
            IntermediateCode code0 = new IntermediateCode(1, InstructionType.push);
            // 查看expression的结果
            Visit(context.expression());

            // 比较expression的结果和1的关系
            // code1的目的地址等待后续回填。因为如果不相等，说明是false，则需要跳转到codeBlock后面的位置。
            IntermediateCode code1 = new IntermediateCode(InstructionType.jne);
            // addr0是codeBlock的开始地址。
            int addr0 = codes.Count;
            
            Visit(context.codeBlock());
            IntermediateCode code2 = new IntermediateCode(1, InstructionType.push);

            // 再次查看expression的结果
            Visit(context.expression());


            // 比较expression的结果和1的关系,是1的话重新跳转回去addr0
            IntermediateCode code3 = new IntermediateCode(addr0, InstructionType.je);
            int addr1 = codes.Count;
            // 这个时候回填地址，如果刚才的条件判断不满足，那么目的地址是codeBlock结束的地址addr1
            code1.setOperant(addr1);
            codes.Add(code0);
            codes.Add(code1);
            codes.Add(code2);
            codes.Add(code3);
            
            return null;
        }

        /*
         访问do while表达式
         */
        public override object VisitDoWhileStatement([NotNull] CMMParser.DoWhileStatementContext context)
        {
            // addr0是codeBlock开始地址
            int addr0 = codes.Count;
            
            Visit(context.codeBlock());

            IntermediateCode code0 = new IntermediateCode(0, InstructionType.push);
            // 查看expression的结果
            Visit(context.expression());

            // 比较expression的结果和0的关系，如果不相等的话，就要跳转回去执行codeBlock
            IntermediateCode code1 = new IntermediateCode(addr0, InstructionType.jne);
            codes.Add(code0);
            codes.Add(code1);
            return null;
        }

        /*
         处理for语句,还没做
         */
        public override object VisitForStatement([NotNull] CMMParser.ForStatementContext context)
        {
            return base.VisitForStatement(context);
        }

        /*
         处理if语句
         */
        public override object VisitIfStatement([NotNull] CMMParser.IfStatementContext context)
        {
            IntermediateCode code0 = new IntermediateCode(1, InstructionType.push);
            // 查看expression的结果
            Visit(context.expression());

            // 比较expression的结果和1的关系,不是1的话就跳转走,code1的目的地址待回填
            IntermediateCode code1 = new IntermediateCode(InstructionType.jne);
            
            Visit(context.codeBlock());
            // 条件不符合的情况应该执行的代码行号为addr0,回填一下
            int addr0 = codes.Count;
            code1.setOperant(addr0);

            codes.Add(code0);
            codes.Add(code1);
            // 每个else语句都执行一下
            if(context.elseClause() != null)
            {
                foreach(CMMParser.ElseClauseContext ctx in context.elseClause())
                {
                    Visit(ctx);
                }
                
            }
            
            return null;
        }

        /*
         读指令
         */
        public override object VisitReadStatement([NotNull] CMMParser.ReadStatementContext context)
        {
            IntermediateCode code = new IntermediateCode(InstructionType.read);
            codes.Add(code);
            return null;
        }

        /*
         写指令
         */
        public override object VisitWriteStatement([NotNull] CMMParser.WriteStatementContext context)
        {
            IntermediateCode code = new IntermediateCode(InstructionType.write);
            codes.Add(code);
            return null;
        }

        /*
         调用函数，需要将
         */
    }
}
