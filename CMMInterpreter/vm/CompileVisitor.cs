using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using CMMInterpreter.vm.exception;
using System.Text;
using Antlr4.Runtime.Tree;

namespace CMMInterpreter.vm
{
    /**
     * @author 谭惠日 杨翔
     * @since 12-07-2020
     * Visitor用于生成中间代码
     * 
     */
    public class CompileVisitor : CMMBaseVisitor<object>
    {
        // 函数地址表
        Dictionary<string, int> functionAddressTable = new Dictionary<string, int>();

        // 当前局部变量表
        Dictionary<string, int> curLocalVariablesTable = new Dictionary<string, int>();

        // 存储函数的信息 key是函数名 value是存储函数信息的类
        Dictionary<string, FunctionInformation> functionInformations = new Dictionary<string, FunctionInformation>();

        // 局部变量表大小
        int curLocalVariablesTableLength = 0;

        // 用于编译时给表达式求值
        Stack<Object> expStack = new Stack<Object>();


        // 指令集合
        public List<IntermediateCode> codes = new List<IntermediateCode>();

        public Dictionary<string, int> GetGlobalSymbolTable()
        {
            return curLocalVariablesTable;
        }

        public Dictionary<string, FunctionInformation> GetFunctionInformations()
        {
            return functionInformations;
        }

        public Dictionary<string, int> GetFunctionAddressTable()
        {
            return functionAddressTable;
        }

        public List<IntermediateCode> generateCodes(IParseTree tree)
        {
            base.Visit(tree);
            fillFuncAddress();
            return codes;
        }

        public void fillFuncAddress()
        {
            for(int i = 0; i < codes.Count; i++)
            {
                if (codes[i].getType().Equals(InstructionType.call))
                {
                    // 先从code中取出函数名，到functionAddressTable中查找函数地址
                    functionAddressTable.TryGetValue(Convert.ToString(codes[i].operant), out int funcAddr);
                    // 函数地址回填
                    codes[i].operant = funcAddr;
                }
            }
        }


        /**
         * 读取identifier的text，然后从函数地址表中读取该函数的中间代码起始地址
         * 调用call指令
         * 将expression List压入被调用函数的局部变量表中
         */
        public override object VisitCallStatement([NotNull] CMMParser.CallStatementContext context)
        {
            

            // 看一下有多少参数
            VisitExpressionList(context.expressionList());
            int count = 0;
            if(context.expressionList() != null)
                count = getLen(context.expressionList());

            //
            
            IntermediateCode code1 = new IntermediateCode(count, InstructionType.push, context.Start.Line);
            codes.Add(code1);

            IntermediateCode code0 = new IntermediateCode(context.Identifier().GetText(), InstructionType.call, context.Start.Line);
            codes.Add(code0);

            return null;
        }

        public override object VisitExpressionStatement([NotNull] CMMParser.ExpressionStatementContext context)
        {
            return base.VisitExpressionStatement(context);
        }


        public override object VisitExpressionList([NotNull] CMMParser.ExpressionListContext context)
        {
            return base.VisitExpressionList(context);
        }

        /*
         获取expressionList的长度
         */
        private int getLen(CMMParser.ExpressionListContext expressionListContext)
        {
            if (expressionListContext.ChildCount == 1) return 1;
            else
                return getLen(expressionListContext.expressionList()) + 1;
        }



        /**
         * 将expression的值求出来并压到栈中
         * 具体的值要看children的情况
         * 
         */
        public override object VisitExpression([NotNull] CMMParser.ExpressionContext context)
        {
            // 访问子树
            Object result = VisitChildren(context);
            Boolean op1, op2;
            if (context.ChildCount.Equals(3))
            {
                
                switch(context.GetChild(1).GetText())
                {
                    case "&&":
                        codes.Add(new IntermediateCode(InstructionType.and, context.Start.Line));
                        // op1 = (Boolean)expStack.Pop();
                        // op2 = (Boolean)expStack.Pop();
                        // expStack.Push(op2 && op1 ? 1 : 0);
                        break;
                    case "||":
                        codes.Add(new IntermediateCode(InstructionType.or, context.Start.Line));
                        // op1 = (Boolean)expStack.Pop();
                        // op2 = (Boolean)expStack.Pop();
                        // expStack.Push(op2 || op1 ? 1 : 0);
                        
                        break;
                    default:
                        
                        break;
                }

            }
            else if (context.ChildCount.Equals(2)) {
                Double op = (Double)expStack.Pop();
                expStack.Push(op == 1 ? 0 : 1);
                codes.Add(new IntermediateCode(InstructionType.not, context.Start.Line));
            }

            return result;
        }


        /**
         * 判断子表达式的值并压入栈中
         * 
         * 
         */
        public override object VisitBoolExpression([NotNull] CMMParser.BoolExpressionContext context)
        {
            Object res = VisitChildren(context);
            Double op1, op2;

            if (context.ChildCount.Equals(3))
            {
                string relationalOperator = context.GetChild(1).GetText();
                
                IntermediateCode code = new IntermediateCode(context.Start.Line);
                switch (relationalOperator)
                {
                    case "<=":
                        code.type = InstructionType.le;
                        
                        // op1 = (Double)expStack.Pop();
                        // op2 = (Double)expStack.Pop();
                        // expStack.Push(op2 <= op1 ? 1 : 0);
                        break;
                    case ">=":
                        code.type = InstructionType.ge;
                        // op1 = (Double)expStack.Pop();
                        // op2 = (Double)expStack.Pop();
                        // expStack.Push(op2 >= op1 ? 1 : 0);
                        break;
                    case "==":
                        code.type = InstructionType.eq;
                        // op1 = (Double)expStack.Pop();
                        // op2 = (Double)expStack.Pop();
                        // expStack.Push(op2 == op1 ? 1 : 0);
                        break;
                    case "<":
                        code.type = InstructionType.l;
                        // op1 = (Double)expStack.Pop();
                        // op2 = (Double)expStack.Pop();
                        // expStack.Push(op2 < op1 ? 1 : 0);
                        break;
                    case ">":
                        code.type = InstructionType.g;
                        // op1 = (Double)expStack.Pop();
                        // op2 = (Double)expStack.Pop();
                        // expStack.Push(op2 > op1 ? 1 : 0);
                        break;
                    case "<>":
                        code.type = InstructionType.ne;
                        // op1 = (Double)expStack.Pop();
                        // op2 = (Double)expStack.Pop();
                        // expStack.Push(op2 != op1 ? 1 : 0);
                        break;
                    default:
                        break;
                }
                codes.Add(code);


            }
            else {
                // additiveExpression的情况不用处理 直接把值压栈
            }

            return res;
        }


        public override object VisitRelationalOperator([NotNull] CMMParser.RelationalOperatorContext context)
        {
            return base.VisitRelationalOperator(context);
        }


        /**
         * factor 情况比较多
         * 
         * 
         */

        /*
         * int a = 1;
         * a = a+2;
         * out(a);
         */
        public override object VisitFactor([NotNull] CMMParser.FactorContext context)
        {
            int funcAddr;
            if(context.Identifier() != null)
            {

                switch (context.ChildCount)
                {
                    case 1:
                        // 访问局部变量
                        if (!curLocalVariablesTable.TryGetValue(context.Identifier().GetText(), out int addr))
                        {
                            throw new VariableNotFountException(context.Identifier().GetText(), context);
                        }
                        codes.Add(new IntermediateCode(addr, InstructionType.pushv, context.Start.Line));
                        
                        break;
                    case 3:
                        // 这是不带参数函数调用的情况
                        functionAddressTable.TryGetValue(context.GetChild(0).GetText(), out funcAddr);
                        codes.Add(new IntermediateCode(funcAddr, InstructionType.call, context.Start.Line));
                        break;
                    case 4:

                        if (context.GetChild(1).GetText().Equals("["))
                        {
                            // 这是数组的情况
                            curLocalVariablesTable.TryGetValue(context.GetChild(0).GetText(), out int arrayAddr);
                            // 先计算表达式的值
                            Visit(context.GetChild(2));
                            // 此时栈顶是数组的下标 再压入数组的地址 然后相加得到该元素的地址
                            codes.Add(new IntermediateCode(arrayAddr,InstructionType.push, context.Start.Line));
                            codes.Add(new IntermediateCode(InstructionType.add, context.Start.Line));
                            // pushv指令不带操作数 将当前栈顶的元素作为地址 到局部变量表中把对应地址的元素压栈
                            codes.Add(new IntermediateCode(InstructionType.pushv, context.Start.Line));
                        }
                        else
                        {
                            // 这是带参数函数调用的情况
                            // 首先把参数压栈
                            Visit(context.expressionList());
                            // 然后把参数个数压栈
                            int count = getLen(context.expressionList());
                            codes.Add(new IntermediateCode(count, InstructionType.push, context.Start.Line));
                            functionAddressTable.TryGetValue(context.Identifier().GetText(), out funcAddr);
                            // 添加call指令 不能直接压入函数地址，因为此时函数可能还没定义。先压入函数名，所有代码生成结束后再回填函数地址。
                            codes.Add(new IntermediateCode(context.Identifier().GetText(), InstructionType.call, context.Start.Line));
                        }
                        break;
                }
                return null;
                
            }
            if (context.GetChild(0).GetText() == "(")
            {
                // 表达式的情况 直接遍历表达式求值
                Visit(context.GetChild(1));
            }
            if (context.Sub() != null)
            {
                // 取相反数的情况 先visit factor，它的值压栈后加入neg指令
                Visit(context.GetChild(1));
                // 直接把栈顶元素取反
                // Double num = (Double)expStack.Pop();
                // expStack.Push(-num);
                codes.Add(new IntermediateCode(InstructionType.neg, context.Start.Line));
            }
            if (context.IntegerLiteral() != null)
            {
                // 整数直接压栈
                // expStack.Push(Convert.ToDouble(context.IntegerLiteral().GetText()));
                codes.Add(new IntermediateCode(Convert.ToDouble(context.IntegerLiteral().GetText()), InstructionType.push, context.Start.Line));
            }
            if (context.RealLiteral() != null)
            {
                // 实数直接压栈
                // expStack.Push(Convert.ToDouble(context.IntegerLiteral().GetText()));
                codes.Add(new IntermediateCode(Convert.ToDouble(context.IntegerLiteral().GetText()), InstructionType.push, context.Start.Line));
            }
            if (context.True() != null)
            {
                // true压入1
                // expStack.Push(1);
                codes.Add(new IntermediateCode(1, InstructionType.push, context.True().Symbol.Line));
            }
            if (context.False() != null)
            {
                // false压入0
                // expStack.Push(0);
                codes.Add(new IntermediateCode(0, InstructionType.push, context.False().Symbol.Line));
            }
            

            return null;
        }


        public override object VisitDeclaration([NotNull] CMMParser.DeclarationContext context)
        {
            return base.VisitDeclaration(context);
        }

        public override object VisitVariableDeclaration([NotNull] CMMParser.VariableDeclarationContext context)
        {
            


            return base.VisitVariableDeclaration(context);


        }

        public override object VisitInitializerList([NotNull] CMMParser.InitializerListContext context)
        {
            // 直接遍历所有初始化变量即可
            return base.VisitInitializerList(context);
        }


        /**
         * 
         */
        public override object VisitAdditiveExpression([NotNull] CMMParser.AdditiveExpressionContext context)
        {
            Double op1, op2;
            IntermediateCode code = new IntermediateCode(context.Start.Line);
            // 先遍历左右子树
            VisitChildren(context);
            if (context.ChildCount.Equals(3))
            {

                // 左右
                switch (context.GetChild(1).GetText())
                {
                    case "+":
                        code.type = InstructionType.add;
                        // 计算当前表达式的值 以便万一要给数组分配大小
                        // op1 = (Double)expStack.Pop();
                        // op2 = (Double)expStack.Pop();
                        // expStack.Push(op2 + op1);
                        break;
                    case "-":
                        code.type = InstructionType.sub;
                        // 计算当前表达式的值 以便万一要给数组分配大小
                        // op1 = (Double)expStack.Pop();
                        // op2 = (Double)expStack.Pop();
                        // expStack.Push(op2 - op1);
                        break;
                }
                codes.Add(code);
            }
            else
            {
                // 子树是term的情况 不用处理
            }
            

            return null;
        }

        public override object VisitTerm([NotNull] CMMParser.TermContext context)
        {
            Object res = VisitChildren(context);
            Double op1, op2;
            if (context.ChildCount.Equals(3))
            {
                
                // 添加计算代码
                switch(context.GetChild(1).GetText())
                {
                    case "*":
                        codes.Add(new IntermediateCode(InstructionType.mul, context.Start.Line));
                        // 计算当前表达式的值 以便万一要给数组分配大小
                        // op1 = (Double)expStack.Pop();
                        // op2 = (Double)expStack.Pop();
                        // expStack.Push(op2 * op1);
                        break;
                    case "/":
                        codes.Add(new IntermediateCode(InstructionType.div, context.Start.Line));
                        // op1 = (Double)expStack.Pop();
                        // op2 = (Double)expStack.Pop();
                        // 计算当前表达式的值 以便万一要给数组分配大小
                        // expStack.Push(op2 / op1);
                        
                        break;
                    default:
                        
                        break;
                }

            }
            else
            {
                // 不用处理
            }

            return res;
        }

        /**
         * 处理生成函数的中间代码时
         * 在函数的第一行地址前加入j指令跳转到函数的最后一行地址之后
         * 
         */
        public override object VisitFunctionDeclaration([NotNull] CMMParser.FunctionDeclarationContext context)
        {
            string funcName = context.GetChild(1).GetText();
            // jump到函数ret的下一行代码 最后再回填
            IntermediateCode jumpCode = new IntermediateCode(InstructionType.j, context.Start.Line);
            codes.Add(jumpCode);
            // 保存局部变量表和大小
            Dictionary<string, int> storedVariableTable = curLocalVariablesTable;
            int storedVariableTableSize = curLocalVariablesTableLength;
            
            curLocalVariablesTable = new Dictionary<string, int>();
            curLocalVariablesTableLength = 0;

            // 用于记录该函数的各种信息
            FunctionInformation funcInfo = new FunctionInformation(funcName);

            // 记录函数的起始地址
            int funcAddress = codes.Count;
            functionAddressTable.Add(funcName, funcAddress);
            funcInfo.enrtyAddress = funcAddress;

            // 访问参数列表
            Visit(context.parameterClause());
            // visit code block生成当前函数的中间代码
            Visit(context.codeBlock());

            // 填入函数的出口地址
            funcInfo.outAddress = codes.Count - 1;
            // 回填jump指令
            jumpCode.setOperant(codes.Count);
            funcInfo.localVariableTable = curLocalVariablesTable;
            // 将函数信息保存到函数信息表中
            functionInformations.Add(funcName, funcInfo);

            // 恢复局部变量表和大小
            curLocalVariablesTable = storedVariableTable;
            curLocalVariablesTableLength = storedVariableTableSize;
            return null;

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
                    throw new VariableRedefinedException(context.GetChild(0).GetText(), context);
                }
                curLocalVariablesTable.Add(context.GetChild(0).GetText(), curLocalVariablesTableLength);
                curLocalVariablesTableLength++;
                Visit(context.GetChild(2));
                curLocalVariablesTable.TryGetValue(context.GetChild(0).GetText(), out int index);
                IntermediateCode code = new IntermediateCode(index, InstructionType.pop, context.Start.Line);
                codes.Add(code);

            }
            else {
                // 定义数组
                // 首先visit a[exp] exp的值被压到当前stack上
                // expStack.Clear();
                // Visit(context.GetChild(2));
                // Double expValue = (Double)expStack.Pop();
                // if (Math.Ceiling(expValue) != expValue) {
                //     throw new CompileException("数组元素个数必须为整数！");
                // }

                // 支支持常量定义
                int size = Convert.ToInt32(context.GetChild(2).GetText());
                curLocalVariablesTable.Add(context.GetChild(0).GetText(), curLocalVariablesTableLength);
                
                int initialExpCount = context.expression().Length - 1;
                
                
                for(int i = 1; i < context.expression().Length; i++) {
                    // 求出所有子表达式的值 放在栈上
                    Visit(context.expression()[i]);
                    
                }
                for (int i = 0; i < size - initialExpCount; i++) {
                    // 剩下的元素全部赋值为0
                    codes.Add(new IntermediateCode(0, InstructionType.push, context.Start.Line));
                }
                for(int i = size - 1; i >= 0; i--) {
                    // 从后往前给数组赋值
                    codes.Add(new IntermediateCode(curLocalVariablesTableLength + i, InstructionType.pop, context.Start.Line));
                }
                // 更新局部变量表大小
                curLocalVariablesTableLength += size;
            }
            return null;
        }

        /*
         访问参数列表，为了方便对参数进行管理，直接将参数列表保存在局部变量区中。而且由于参数列表是函数中最先被处理的变量，因此可以摆放在局部变量区的最开始位置
         */
        public override object VisitParameterList([NotNull] CMMParser.ParameterListContext context)
        {
            // 把参数加入到局部变量表中。运行时在遇到call指令的时候将要将参数按照顺序加入局部变更量表。
            curLocalVariablesTable.Add(context.GetChild(1).GetText(), curLocalVariablesTableLength);
            curLocalVariablesTableLength++;
            if(context.parameterList() != null)
            {
                Visit(context.parameterList());
            }
          
            return null;
        }

        /*
         处理赋值操作，
        前提：leftValue将待赋值的变量在局部变更量表中的索引已经压入栈中，expression处理的结果已经压入操作栈中
         */
        public override object VisitAssignment([NotNull] CMMParser.AssignmentContext context)
        {

            Visit(context.leftValue());

            // tmp是一个虚拟的变量，用于 存储leftVal的索引 在局部变量表的位置
            int tmp = curLocalVariablesTableLength;

            // 把leftVal的值放到局部变更量表Count位置上
            IntermediateCode code0 = new IntermediateCode(tmp, InstructionType.pop, context.leftValue().Start.Line);
            codes.Add(code0);

            // 把Expression的值压入栈中
            Visit(context.expression());
            
            //将Experession的值放入Count中
            IntermediateCode code1 = new IntermediateCode("("+tmp+")", InstructionType.pop, context.expression().Start.Line);
            // 这个过程中，局部变量表并没有增加新的元素，所以curLocalVariable并没有Add操作
            codes.Add(code1);
            
            // 局部变量表里面，1号位置的值是0
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
                IntermediateCode code = new IntermediateCode(addr, InstructionType.push, context.Start.Line);
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
                IntermediateCode code0 = new IntermediateCode(addr, InstructionType.push, context.Start.Line);
                Visit(context.expression());
                // 使用add指令，获得addr+expression，add指令使得这个值已经压入栈中
                IntermediateCode code1 = new IntermediateCode(InstructionType.add, context.expression().Start.Line);
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
            如果expression的操作结果是false，就在栈中压入0，否则压入1.
             
             */
            int curSize = curLocalVariablesTableLength;
            int idx = codes.Count;
            IntermediateCode code0 = new IntermediateCode(0, InstructionType.push, context.Start.Line);
            codes.Add(code0);
            // 查看expression的结果
            Visit(context.expression());

            // 比较expression的结果和1的关系
            // code1的目的地址等待后续回填。因为如果与0相等，说明是false，则需要跳转到codeBlock后面的位置。
            IntermediateCode code1 = new IntermediateCode(InstructionType.je, context.expression().Start.Line);
            codes.Add(code1);
            // addr0是codeBlock的开始地址。
            int addr0 = codes.Count;
            
            Visit(context.codeBlock());
            IntermediateCode code2 = new IntermediateCode(0, InstructionType.j, context.codeBlock().Stop.Line);
            codes.Add(code2);
            int addr1 = codes.Count;
            // 这个时候回填地址，如果刚才的条件判断不满足，那么目的地址是codeBlock结束的地址addr1
            code1.setOperant(addr1);
            code2.setOperant(idx);
            codes.Add(new IntermediateCode(curSize, InstructionType.delv, context.codeBlock().Stop.Line));
            curLocalVariablesTableLength = curSize;
            removeVariableBiggerThanK(curSize);



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

            IntermediateCode code0 = new IntermediateCode(0, InstructionType.push, context.expression().Start.Line);
            // 查看expression的结果
            Visit(context.expression());

            // 比较expression的结果和0的关系，如果不相等的话，就要跳转回去执行codeBlock
            IntermediateCode code1 = new IntermediateCode(addr0, InstructionType.jne, context.expression().Start.Line);
            codes.Add(code0);
            codes.Add(code1);
            return null;
        }

        /*
         处理for语句
         */
        public override object VisitForStatement([NotNull] CMMParser.ForStatementContext context)
        {
            // 当前局部变量表的大小，访问完ForStatement之后要恢复局部变量表
            int curSize = curLocalVariablesTableLength;
            // 这里面可能会定义新的变量，不过没关系，直接插入局部变量表中就可以了，最后我们恢复的。
            if (context.forInitializer() != null)
            {
                Visit(context.forInitializer());
            }
            // 访问expression，将执行的结果压入栈中
            // addr_expression是expression（比较）的地址
            int addr_expression = codes.Count;
            IntermediateCode code0, code1=null;
            if (context.expression() != null)
            {
                Visit(context.expression());


                // 如果expression的结果是真，才会访问
                // 这里的0是数字，不是索引，要加上范围！！
                code0 = new IntermediateCode(0, InstructionType.push, context.Start.Line);
                // 如果是0的话，就直接跳转到codeBlock之后，并且释放局部变量。目的地址待回填
                code1 = new IntermediateCode(InstructionType.je, context.Start.Line);

                // addr0是codeBlock的起始地址
                codes.Add(code0);
                codes.Add(code1);
            }
            int addr0 = codes.Count;
            // 访问codeBlock
            if (context.codeBlock() != null)
                Visit(context.codeBlock());

            // 访问更新操作,更新操作的起始地址是addr1
            int addr1 = codes.Count;
            if (context.assignment() != null)
                Visit(context.assignment());
            // 跳转回去expression去判断
            codes.Add(new IntermediateCode(addr_expression, InstructionType.j, context.Start.Line));

            // addr2是执行完codeBlock，而且判断确定不跳转的代码
            int addr2 = codes.Count;
            if(code1 != null)
                code1.setOperant(addr2);
            // 替换所有出现的continue break，代码的范围是addr0-add2， 更新操作的代码在addr1
            replaceBreakAndConti(codes, addr0, addr2 - 1, addr1);
            //局部变量表大于curSize的部分全部删掉！
            codes.Add(new IntermediateCode(curSize, InstructionType.delv, context.Stop.Line));
            curLocalVariablesTableLength = curSize;
            removeVariableBiggerThanK(curSize);
            return null;
        }

        // 替换codes[addr0, addr2]中所有的break和continue语句，其中break -> jump addr2, continue -> jump addr1
        private void replaceBreakAndConti(List<IntermediateCode> codes, int addr0, int addr2, int addr1) 
        {
            IntermediateCode[] codeArray = codes.ToArray();
            for(int i = addr0; i <= addr2; i++)
            {
                IntermediateCode code;
                switch (codeArray[i].getType())
                {
                    case InstructionType.b:
                        code = new IntermediateCode(addr2+1, InstructionType.j, codeArray[i].lineNum);
                        codes.RemoveAt(i);
                        codes.Insert(i, code);
                        break;
                    case InstructionType.cnt:
                        code = new IntermediateCode(addr1, InstructionType.j, codeArray[i].lineNum);
                        codes.RemoveAt(i);
                        codes.Insert(i, code);
                        break;
                    default:
                        break;
                }
            }
            
        }

        /*
         处理if语句
         */
        public override object VisitIfStatement([NotNull] CMMParser.IfStatementContext context)
        {

            // 查看expression的结果
            Visit(context.expression());
            IntermediateCode code0 = new IntermediateCode(0, InstructionType.push, context.expression().Start.Line);
            codes.Add(code0);
            // 比较expression的结果和0的关系,等于0的话就跳转走,code1的目的地址待回填
            IntermediateCode code1 = new IntermediateCode(InstructionType.je, context.expression().Stop.Line);

            codes.Add(code1);
            if(context.codeBlock() != null)
                Visit(context.codeBlock());
            IntermediateCode code3 = new IntermediateCode(InstructionType.j, context.codeBlock().Stop.Line);
            codes.Add(code3);
            // 条件不符合的情况应该执行的代码行号为addr0,回填一下
            int addr0 = codes.Count;
            code1.setOperant(addr0);
            List<IntermediateCode> needToAddFinishAddrCodes = new List<IntermediateCode>();
            needToAddFinishAddrCodes.Add(code3);

            // 每个else语句都执行一下
            if (context.elseClause() != null)
            {
                foreach (CMMParser.ElseClauseContext ctx in context.elseClause())
                {
                    if (ctx.ifStatement() == null)
                    {
                        Visit(ctx.codeBlock());
                        IntermediateCode code = new IntermediateCode(InstructionType.j, ctx.codeBlock().Stop.Line);
                        needToAddFinishAddrCodes.Add(code);
                        codes.Add(code);
                    }
                    else
                    {
                        Visit(ctx.ifStatement());
                    }

                }


            }
            foreach (IntermediateCode c in needToAddFinishAddrCodes)
            {
                c.operant = codes.Count;
            }

            return null;
        }

        /*
         读指令
         */
        public override object VisitReadStatement([NotNull] CMMParser.ReadStatementContext context)
        {
            CMMParser.LeftValueContext[] leftValueContexts = context.leftValue();
            foreach(CMMParser.LeftValueContext leftValueContext in leftValueContexts)
            {
                string variableName = leftValueContext.GetChild(0).GetText();
                Boolean result = curLocalVariablesTable.TryGetValue(variableName, out int address);
                if (!result)
                {
                    throw new VariableNotFountException(variableName, context);
                }
                if (leftValueContext.ChildCount == 1)
                {
                    
                    codes.Add(new IntermediateCode(InstructionType.read, context.Start.Line));
                    codes.Add(new IntermediateCode(address, InstructionType.pop, context.Start.Line));
                }
                else
                {
                    codes.Add(new IntermediateCode(InstructionType.read, context.Start.Line));

                    // 计算数组地址
                    codes.Add(new IntermediateCode(address, InstructionType.push, context.Start.Line));
                    Visit(context.GetChild(2));
                    codes.Add(new IntermediateCode(InstructionType.add, context.Start.Line));
                    // 保存地址的值
                    codes.Add(new IntermediateCode(curLocalVariablesTableLength, InstructionType.pop, context.Start.Line));
                    // 指针寻址赋值
                    codes.Add(new IntermediateCode("(" + curLocalVariablesTableLength.ToString() + ")", InstructionType.pop, context.Start.Line));

                    
                }
            }
            
            return null;
        }

        /*
         写指令
         */
        public override object VisitWriteStatement([NotNull] CMMParser.WriteStatementContext context)
        {
            IntermediateCode code;
            if (context.ChildCount == 4) {
                // write不带参数
                code = new IntermediateCode(0, InstructionType.write, context.Start.Line);
                

            }
            else
            {
                // write带参数 先访问参数 将其压栈
                VisitChildren(context);
                // 打印栈顶元素
                code = new IntermediateCode(1, InstructionType.write, context.Start.Line);
            }

            codes.Add(code);
            return null;
        }

        /*
         jumpStatement
        return 语句在第一遍扫描结束之后回填
        break continue 可以在for while语句的时候，直接转换成为jump语句
         */
        public override object VisitJumpStatement([NotNull] CMMParser.JumpStatementContext context)
        {
            IntermediateCode code = null;
            switch (context.GetChild(0).GetText())
            {
                case "break":
                    code = new IntermediateCode(InstructionType.b, context.Start.Line);
                    break;
                case "continue":
                    code = new IntermediateCode(InstructionType.cnt, context.Start.Line);
                    break;
                case "return":
                    if(context.ChildCount == 2) {
                        // 向操作栈压入0表示没有返回值
                        codes.Add(new IntermediateCode(0, InstructionType.push, context.Start.Line));
                    }
                    else
                    {
                        Visit(context.expression());
                        // 向操作栈压入1表示有返回值
                        codes.Add(new IntermediateCode(1, InstructionType.push, context.Start.Line));
                    }
                    code = new IntermediateCode(InstructionType.ret, context.Start.Line);
                    break;
            }
            codes.Add(code);
            
            return null;
        }

        /*
         在codeBlock中定义的变量在出了block之后应该全部失效
         */
        public override object VisitCodeBlock([NotNull] CMMParser.CodeBlockContext context)
        {
            int tmp = curLocalVariablesTableLength;
            if(context.statements() != null)
            {
                Visit(context.statements());
            }
            // codeBlock中新定义的变量全部失效
            codes.Add(new IntermediateCode(tmp, InstructionType.delv,  context.Stop.Line));
            curLocalVariablesTableLength = tmp;
            removeVariableBiggerThanK(tmp);
            return null;
        }

        public void removeVariableBiggerThanK(int k)
        {
            List<String> removedVariablesName = new List<String>();
            foreach (String str in curLocalVariablesTable.Keys)
            {
                curLocalVariablesTable.TryGetValue(str, out int index);
                if(index >= k)
                {
                    removedVariablesName.Add(str);
                }
            }
            foreach(String str in removedVariablesName)
            {
                curLocalVariablesTable.Remove(str);
            }
        }


    }

}
