
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using CMMInterpreter.CMMException;
using CMMInterpreter.util;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CMMInterpreter.inter
{
    public class RefPhase : CMMBaseVisitor<object>
    {
        Scope globalScope;
        Scope currentScope;
        string sharedTypeStr;
        bool enterFunctionScope = false;
        Stack<Scope> loopStack = new Stack<Scope>();
        public IOutputStream outputStream = null;

        public event Action NeedInput;

        public string buffer = null;

        public override object VisitStatements([NotNull] CMMParser.StatementsContext context)
        {
            //程序总入口
            if (globalScope == null)
            {
                globalScope = new GlobalScope(null);
                currentScope = globalScope;
            }
            base.VisitStatements(context);
            return null;
        }

        public override object VisitExpressionStatement([NotNull] CMMParser.ExpressionStatementContext context)
        {
            if (context.ChildCount == 2)
            {
                VisitExpression(context.expression());
            }
            return null;
        }

        /*
         * 声明语句
         */
        public override object VisitDeclaration([NotNull] CMMParser.DeclarationContext context)
        {
            //函数声明
            if (context.ChildCount == 1)
            {
                return VisitFunctionDeclaration(context.functionDeclaration());
            }
            //变量声明
            else
            {
                return VisitVariableDeclaration(context.variableDeclaration());
            }
        }

        /*
         * 变量声明
         */
        public override object VisitVariableDeclaration([NotNull] CMMParser.VariableDeclarationContext context)
        {
            sharedTypeStr = context.typeSpecifier().GetText();
            VisitInitializerList(context.initializerList());
            return null;
        }

        /*
         * 变量声明列表
         */
        public override object VisitInitializerList([NotNull] CMMParser.InitializerListContext context)
        {
            VisitInitializer(context.initializer());
            if (context.ChildCount == 3)
            {
                VisitInitializerList(context.initializerList());
            }
            return null;
        }

        /*
         * 单一变量声明
         * initializer
	           : Identifier '=' expression
	           | Identifier '[' expression ']' '=' '[' expression ( ',' expression )* ']'
	           ;
         */
        public override object VisitInitializer([NotNull] CMMParser.InitializerContext context)
        {
            string id = context.Identifier().GetText();
            if (currentScope.redundant(id))
            {
                //变量重复声明
               throw new RuntimeException( "变量"+id+"重复声明！",context.Identifier().Symbol.Line);
            }
            if (context.ChildCount == 3)
            {
                object value = VisitExpression(context.expression(0));
                checkType(sharedTypeStr, value);
                BuiltInTypeSymbol type = new BuiltInTypeSymbol(sharedTypeStr);
                Symbol symbol = new VariableSymbol(id, type, value, currentScope);
                currentScope.define(symbol);
            }
            else
            {
                //Identifier '[' expression ']' '=' '[' expression ( ',' expression )* ']'
                object rt = VisitExpression(context.expression(0));
                checkType("int", rt);
                int size = (int)rt;
                if (size <= 0)
                {
                    //数组大小应大于0
                    throw new RuntimeException("数组"+id+"的大小应该是正整数！", context.Identifier().Symbol.Line);
                }
                int childCount = 8 + (size - 1) * 2;
                if (context.ChildCount != childCount)
                {
                    // 元素不够
                    throw new RuntimeException("没有足够的元素给数组" + id + "进行初始化！", context.Identifier().Symbol.Line);
                }
                List<object> al = new List<object>(size);
                for(int i = 6; i < childCount - 1; i += 2)
                {
                    object value = Visit(context.GetChild(i));
                    checkType(sharedTypeStr, value);
                    al.Add(value);
                }
                switch (sharedTypeStr)
                {
                    case "int":
                       int[] i_res = new int[size];
                       for(int i = 0; i < size; ++i)
                        {
                            i_res[i] = (int)al[i];
                        }
                        BuiltInTypeSymbol int_arr = new BuiltInTypeSymbol(sharedTypeStr + "_arr");
                        Symbol int_arr_symbol = new VariableSymbol(id, int_arr, i_res, currentScope);
                        currentScope.define(int_arr_symbol);
                        break;
                    case "real":
                        decimal[] d_res = new decimal[size];
                        for (int i = 0; i < size; ++i)
                        {
                            if(al[i] is int)
                            {
                                d_res[i] = (int)al[i];
                            }
                            else
                            {
                                d_res[i] = (decimal)al[i];
                            }
                        }
                        BuiltInTypeSymbol dec_arr = new BuiltInTypeSymbol(sharedTypeStr + "_arr");
                        Symbol dec_arr_symbol = new VariableSymbol(id, dec_arr, d_res, currentScope);
                        currentScope.define(dec_arr_symbol);
                        break;
                    case "bool":
                        bool[] b_res = new bool[size];
                        for(int i = 0; i < size; ++i)
                        {
                            b_res[i] = (bool)al[i];
                        }
                        BuiltInTypeSymbol bool_arr = new BuiltInTypeSymbol(sharedTypeStr + "_arr");
                        Symbol bool_arr_symbol = new VariableSymbol(id, bool_arr, b_res, currentScope);
                        currentScope.define(bool_arr_symbol);
                        break;
                }
            }
            return null;
        }

        /*private void checkType(string typeStr,object value,IToken token)
        {
            bool res = false;
            switch (typeStr)
            {
                case "int":
                case "int_arr":
                    res = value is int || value is int[];
                    break;
                case "real":
                    res = value is decimal || value is int;
                    break;
                case "real_arr":
                    res = value is decimal || value is int || value is decimal[];
                    break;
                case "bool":
                case "bool_arr":
                    res = value is bool || value is bool[];
                    break;
            }
            if (!res)
            {
                throw new RuntimeException(token.Text+"类型非法！", token.Line);
            }
        }*/
        private void checkType(string typeStr, object value)
        {
            bool res = false;
            switch (typeStr)
            {
                case "int":
                case "int_arr":
                    res = value is int || value is int[];
                    break;
                case "real":
                    res = value is decimal || value is int;
                    break;
                case "real_arr":
                    res = value is decimal || value is int || value is decimal[];
                    break;
                case "bool":
                case "bool_arr":
                    res = value is bool || value is bool[];
                    break;
            }
            if (!res)
            {
                throw new Exception();
            }
        }
        /*
         * 函数声明
         */
        /*
         functionDeclaration
	        : returnType Identifier parameterClause codeBlock
	        ; 
           */
        public override object VisitFunctionDeclaration([NotNull] CMMParser.FunctionDeclarationContext context)
        {
            string retTypeStr = context.returnType().GetText();
            BuiltInTypeSymbol retType = null;
            if (!retTypeStr.Equals("void"))
            {
                retType = new BuiltInTypeSymbol(retTypeStr);
            }
            string id = context.Identifier().GetText();
            if (currentScope.redundant(id))
            {
                throw new RuntimeException("函数"+id+"重复声明！",context.Identifier().Symbol.Line);
            }
            FunctionSymbol function=null;
            object parameters = VisitParameterClause(context.parameterClause());
            if (parameters == null)
            {
                function = new FunctionSymbol(id, retType, globalScope, null, context.codeBlock());
            }
            else
            {
                function = new FunctionSymbol(id, retType, globalScope, (LinkedHashMap<string, BuiltInTypeSymbol>) parameters, context.codeBlock());
            }
            globalScope.define(function);
            return null;
        }

        public override object VisitParameterClause([NotNull] CMMParser.ParameterClauseContext context)
        {
            //没有参数
            if (context.ChildCount == 2)
            {
                return null;
            }
            //有参数
            else
            {
                return VisitParameterList(context.parameterList());
            }
        }

        public override object VisitParameterList([NotNull] CMMParser.ParameterListContext context)
        {
            LinkedHashMap<string, BuiltInTypeSymbol> al = new LinkedHashMap<string, BuiltInTypeSymbol>();
            if (context.ChildCount == 2)
            {
                al.Add(context.Identifier().GetText(), new BuiltInTypeSymbol(context.typeSpecifier().GetText()));
                return al;
            }else if (context.ChildCount == 3)
            {
                al.Add(context.Identifier().GetText(), new BuiltInTypeSymbol(context.typeSpecifier().GetText()+"_arr"));
                return al;
            }
            else if (context.ChildCount == 4)
            {
                al.Add(context.Identifier().GetText(), new BuiltInTypeSymbol(context.typeSpecifier().GetText()));
                LinkedHashMap<string, BuiltInTypeSymbol> after = (LinkedHashMap<string, BuiltInTypeSymbol>)VisitParameterList(context.parameterList());
                ICollection<string> keys= after.Keys;
                foreach(string key in keys)
                {
                    al.Add(key, after[key]);
                }
                return al;
            }
            else
            {
                al.Add(context.Identifier().GetText(), new BuiltInTypeSymbol(context.typeSpecifier().GetText()+"_arr"));
                LinkedHashMap<string, BuiltInTypeSymbol> after = (LinkedHashMap<string, BuiltInTypeSymbol>)VisitParameterList(context.parameterList());
                ICollection<string> keys = after.Keys;
                foreach (string key in keys)
                {
                    al.Add(key, after[key]);
                }
                return al;
            }
        }

        /*
         * 赋值语句
         */
        public override object VisitAssignment(CMMParser.AssignmentContext context)
        {
            if (context != null)
            {
                handleAssign(context.leftValue(), VisitExpression(context.expression()));
            }
            return null;
        }


        private void handleAssign([NotNull] CMMParser.LeftValueContext context,object rightValue)
        {
            Symbol symbol = currentScope.resolve(context.Identifier().GetText());
            if (symbol == null || !(symbol is VariableSymbol))
            {
                throw new RuntimeException("变量"+symbol+"未定义，"+"或者对函数"+symbol+"进行了非法赋值！",context.Identifier().Symbol.Line);
            }
            string typeStr = symbol.getType().getName();
            checkType(typeStr, rightValue);
            if (context.ChildCount == 1)
            {
                symbol.setValue(rightValue);
            }
            else
            {
                object indexObj = Visit(context.expression());
                checkType("int", indexObj);
                arrayAssign(symbol, (int)indexObj, rightValue);
            }
        }

        /*
         * 数组赋值
         */
        private void arrayAssign(Symbol symbol,int index,object rightValue)
        {
            switch (symbol.getType().getName())
            {
                case "int_arr":
                    int[] leftValue = (int[])symbol.getValue();
                    indexCheck(leftValue.Length, index);
                    leftValue[index] = (int)rightValue;
                    break;
                case "real_arr":
                    decimal[] d_value = (decimal[])symbol.getValue();
                    indexCheck(d_value.Length, index);
                    if (rightValue is int)
                    {
                        d_value[index] = (int)rightValue;
                    }
                    else
                    {
                        d_value[index] = (decimal)rightValue;
                    }
                    break;
                case "bool_arr":
                    bool[] b_value = (bool[])symbol.getValue();
                    indexCheck(b_value.Length, index);
                    b_value[index] = (bool)rightValue;
                    break;
            }
        }

        /*
         * 数组下标检查
         */
        private void indexCheck(int len,int index)
        {
            if (index < 0 || index >= len)
            {
                throw new Exception("数组下标溢出");
            }
        }

        /*
         * while 循环
         */
        public override object VisitWhileStatement([NotNull] CMMParser.WhileStatementContext context)
        {
            object condition = VisitExpression(context.expression());
            checkType("bool", condition);
            loopStack.Push(currentScope);
            while ((bool)VisitExpression(context.expression()))
            {
                try
                {
                    VisitCodeBlock(context.codeBlock());
                }catch(KeyWordException e)
                {
                    if (e.Message.Equals("break"))
                    {
                        currentScope = loopStack.Pop();
                        return null;
                    }
                }
            }
            loopStack.Pop();
            return null;
        }

        /*
         * do_while循环
         */
        public override object VisitDoWhileStatement([NotNull] CMMParser.DoWhileStatementContext context)
        {
            object condition = VisitExpression(context.expression());
            checkType("bool", condition);
            loopStack.Push(currentScope);
            do
            {
                try
                {
                    VisitCodeBlock(context.codeBlock());
                }catch(KeyWordException e)
                {
                    if (e.Message.Equals("break"))
                    {
                        currentScope = loopStack.Pop();
                        return null;
                    }
                }
            } while ((bool)VisitExpression(context.expression()));
            loopStack.Pop();
            return null;
        }

        /*
         * for循环
         */
        /*
        forStatement
	        : 'for' '(' ( forInitializer )? ';' ( expression )? ';' ( assignment )? ')' codeBlock
	        ;
        forInitializer
	        : variableDeclaration
	        | assignment
	        ;
         */
        private void visitForInitializer(CMMParser.ForInitializerContext context, List<string> forIds)
        {
            if (context != null)
            {
                if (context.variableDeclaration() != null)
                {
                    handleForVariableDeclaration(context.variableDeclaration(), forIds);
                }
                else
                {
                    VisitAssignment(context.assignment());
                }
            }
        }

        public override object VisitForStatement([NotNull] CMMParser.ForStatementContext context)
        {
            List<string> forIds = new List<string>();
            visitForInitializer(context.forInitializer(), forIds);
            if (context.expression() != null)
            {
                object condition = VisitExpression(context.expression());
                checkType("bool", condition);
                loopStack.Push(currentScope);
                for (; (bool)VisitExpression(context.expression()); VisitAssignment(context.assignment()))
                {
                    try
                    {
                        VisitCodeBlock(context.codeBlock());
                    }
                    catch (KeyWordException e)
                    {
                        if (e.Message.Equals("break"))
                        {
                            currentScope = loopStack.Pop();
                            forIds.ForEach(item => currentScope.remove(item));
                            return null;
                        }
                    }
                }
            }
            else
            {
                loopStack.Push(currentScope);
                for (; ; VisitAssignment(context.assignment()))
                {
                    try
                    {
                        VisitCodeBlock(context.codeBlock());
                    }
                    catch (KeyWordException e)
                    {
                        if (e.Message.Equals("break"))
                        {
                            currentScope = loopStack.Pop();
                            forIds.ForEach(item => currentScope.remove(item));
                            return null;
                        }
                    }
                }
            }
            loopStack.Pop();
            forIds.ForEach(item => currentScope.remove(item));
            return null;

            //这里可能复杂了，有可能可以直接visit,不管是不是空
            ////有两个思路，手动匹配 和 正则表达式匹配，这里手动匹配
            //bool initialize = false;//是否有初始化语句
            //bool judge = false;//是否有条件判断语句
            //bool update = false;//是否有更新语句
            //if (context.forInitializer() != null)
            //{
            //    initialize = true;
            //}
            //if (context.expression() != null)
            //{
            //    judge = true;
            //}
            //if (context.assignment() != null)
            //{
            //    update = true;
            //}
            ////string forText = context.GetText();//完整的for循环语句，去掉了全部空格
            ////int lastIndex = forText.IndexOf('{');// 第一个'{'一定是与'for()'里的')'紧邻
            ////string match = forText.Substring(4, lastIndex - 5);//得到（）里面的代码
            ////if (match[0] != ';')
            ////{
            ////    //说明含有初始化语句
            ////    initialize = true;
            ////}
            ////int secondSemicolon = match.LastIndexOf(';');
            ////if (match[secondSemicolon - 1] != ';')
            ////{
            ////    //说明含有条件判断语句
            ////    judge = true;
            ////}
            ////if (secondSemicolon != match.Length - 1)
            ////{
            ////    //说明含有更新语句
            ////    update = true;
            ////}
            //if (initialize)
            //{
            //    if (judge)
            //    {
            //        if (update)
            //        {
            //            object condition = VisitExpression(context.expression());
            //            checkType("bool", condition);
            //            for (VisitForInitializer(context.forInitializer()); (bool)VisitExpression(context.expression()); VisitExpression(context.expression(2)))
            //            {
            //                try
            //                {
            //                    VisitCodeBlock(context.codeBlock());
            //                }
            //                catch (KeyWordException e)
            //                {
            //                    if (e.Message.Equals("break"))
            //                    {
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            object condition = VisitExpression(context.expression(1));
            //            checkType("bool", condition);
            //            for (VisitExpression(context.expression(0)); (bool)VisitExpression(context.expression(1));)
            //            {
            //                try
            //                {
            //                    VisitCodeBlock(context.codeBlock());
            //                }
            //                catch (KeyWordException e)
            //                {
            //                    if (e.Message.Equals("break"))
            //                    {
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (update)
            //        {
            //            for (VisitExpression(context.expression(0)); ; VisitExpression(context.expression(1)))
            //            {
            //                try
            //                {
            //                    VisitCodeBlock(context.codeBlock());
            //                }
            //                catch (KeyWordException e)
            //                {
            //                    if (e.Message.Equals("break"))
            //                    {
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            for (VisitExpression(context.expression(0)); ;)
            //            {
            //                try
            //                {
            //                    VisitCodeBlock(context.codeBlock());
            //                }
            //                catch (KeyWordException e)
            //                {
            //                    if (e.Message.Equals("break"))
            //                    {
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    if (judge)
            //    {
            //        if (update)
            //        {
            //            object condition = VisitExpression(context.expression(0));
            //            checkType("bool", condition);
            //            for (; (bool)VisitExpression(context.expression(0)); VisitExpression(context.expression(1)))
            //            {
            //                try
            //                {
            //                    VisitCodeBlock(context.codeBlock());
            //                }
            //                catch (KeyWordException e)
            //                {
            //                    if (e.Message.Equals("break"))
            //                    {
            //                        break;
            //                    }
            //                }
            //            }

            //        }
            //        else
            //        {
            //            object condition = VisitExpression(context.expression(0));
            //            checkType("bool", condition);
            //            for (; (bool)VisitExpression(context.expression(0));)
            //            {
            //                try
            //                {
            //                    VisitCodeBlock(context.codeBlock());
            //                }
            //                catch (KeyWordException e)
            //                {
            //                    if (e.Message.Equals("break"))
            //                    {
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (update)
            //        {
            //            for (; ; VisitExpression(context.expression(0)))
            //            {
            //                try
            //                {
            //                    VisitCodeBlock(context.codeBlock());
            //                }
            //                catch (KeyWordException e)
            //                {
            //                    if (e.Message.Equals("break"))
            //                    {
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            for (; ; )
            //            {
            //                try
            //                {
            //                    VisitCodeBlock(context.codeBlock());
            //                }
            //                catch (KeyWordException e)
            //                {
            //                    if (e.Message.Equals("break"))
            //                    {
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //return null;
        }
        private void handleForVariableDeclaration(CMMParser.VariableDeclarationContext context, List<string> forIds)
        {
            string typeStr = context.typeSpecifier().GetText();
            CMMParser.InitializerListContext initializerListContext = context.initializerList();
            List<CMMParser.InitializerContext> initializerContexts = getInitializerContexts(initializerListContext);
            initializerContexts.ForEach(item => handleForDeclarationHelper(typeStr, item, forIds));
        }

        private List<CMMParser.InitializerContext> getInitializerContexts(CMMParser.InitializerListContext context)
        {
            List<CMMParser.InitializerContext> res = new List<CMMParser.InitializerContext>();
            res.Add(context.initializer());
            if (context.ChildCount != 1)
            {
                res.AddRange(getInitializerContexts(context.initializerList()));
            }
            return res;
        }
        private void handleForDeclarationHelper(string typeStr, CMMParser.InitializerContext context, List<string> forIds)
        {
            string id = context.Identifier().GetText();
            if (currentScope.redundant(id))
            {
                //变量重复声明
                throw new RuntimeException("变量" + id + "重复声明！", context.Identifier().Symbol.Line);
            }
            if (context.ChildCount == 3)
            {
                object value = VisitExpression(context.expression(0));
                checkType(typeStr, value);
                BuiltInTypeSymbol type = new BuiltInTypeSymbol(typeStr);
                Symbol symbol = new VariableSymbol(id, type, value, currentScope);
                currentScope.define(symbol);
                forIds.Add(id);
            }
            else
            {
                object rt = VisitExpression(context.expression(0));
                checkType("int", rt);
                int size = (int)rt;
                if (size <= 0)
                {
                    //数组大小应大于0
                    throw new RuntimeException("数组" + id + "的大小应该是正整数！", context.Identifier().Symbol.Line);
                }
                int childCount = 8 + (size - 1) * 2;
                if (context.ChildCount != childCount)
                {
                    // 元素不够
                    throw new RuntimeException("没有足够的元素给数组" + id + "进行初始化！", context.Identifier().Symbol.Line);
                }
                List<object> al = new List<object>(size);
                for (int i = 6; i < childCount - 1; i += 2)
                {
                    object value = Visit(context.GetChild(i));
                    checkType(typeStr, value);
                    al.Add(value);
                }
                switch (typeStr)
                {
                    case "int":
                        int[] i_res = new int[size];
                        for (int i = 0; i < size; ++i)
                        {
                            i_res[i] = (int)al[i];
                        }
                        BuiltInTypeSymbol int_arr = new BuiltInTypeSymbol(typeStr + "_arr");
                        Symbol int_arr_symbol = new VariableSymbol(id, int_arr, i_res, currentScope);
                        currentScope.define(int_arr_symbol);
                        forIds.Add(id);
                        break;
                    case "real":
                        decimal[] d_res = new decimal[size];
                        for (int i = 0; i < size; ++i)
                        {
                            if (al[i] is int)
                            {
                                d_res[i] = (int)al[i];
                            }
                            else
                            {
                                d_res[i] = (decimal)al[i];
                            }
                        }
                        BuiltInTypeSymbol dec_arr = new BuiltInTypeSymbol(typeStr + "_arr");
                        Symbol dec_arr_symbol = new VariableSymbol(id, dec_arr, d_res, currentScope);
                        currentScope.define(dec_arr_symbol);
                        forIds.Add(id);
                        break;
                    case "bool":
                        bool[] b_res = new bool[size];
                        for (int i = 0; i < size; ++i)
                        {
                            b_res[i] = (bool)al[i];
                        }
                        BuiltInTypeSymbol bool_arr = new BuiltInTypeSymbol(typeStr + "_arr");
                        Symbol bool_arr_symbol = new VariableSymbol(id, bool_arr, b_res, currentScope);
                        currentScope.define(bool_arr_symbol);
                        forIds.Add(id);
                        break;
                }
            }
        }
        /*
         * if 语句
         */
        public override object VisitIfStatement([NotNull] CMMParser.IfStatementContext context)
        {
            object condition = VisitExpression(context.expression());
            checkType("bool", condition);
            if ((bool)condition)
            {
                VisitCodeBlock(context.codeBlock());
            }
            else
            {
                if (context.ChildCount != 5)
                {
                    CMMParser.ElseClauseContext[] elseClauseContexts = context.elseClause();
                    for(int i = 0; i < elseClauseContexts.Length; ++i)
                    {
                        if ((bool)VisitElseClause(elseClauseContexts[i]))
                        {
                            break;
                        }
                    }
                }
            }
            return (bool)condition;
        }

        /*
         * else
         */
        public override object VisitElseClause([NotNull] CMMParser.ElseClauseContext context)
        {
            if (context.codeBlock() != null)
            {
                VisitCodeBlock(context.codeBlock());
                return true;
            }
            else
            {
                return VisitIfStatement(context.ifStatement());
            }
        }

        /*
         * read
         */
        /*
         readStatement
	        : 'read' '(' leftValue ( ',' leftValue )* ')' ';'
	        ;
         */
        public override object VisitReadStatement([NotNull] CMMParser.ReadStatementContext context)
        {
            CMMParser.LeftValueContext[] leftValueContexts = context.leftValue();
            for(int i = 0; i < leftValueContexts.Length; ++i)
            {
                handleRead(leftValueContexts[i]);
            }
            return null;
        }

        private void handleRead([NotNull]CMMParser.LeftValueContext context)
        {
            Symbol symbol = currentScope.resolve(context.Identifier().GetText());
            if (symbol == null || !(symbol is VariableSymbol))
            {
                throw new RuntimeException("变量" + symbol + "未定义，" + "或者对函数" + symbol + "进行了非法赋值！", context.Identifier().Symbol.Line);
            }
            string typeStr = symbol.getType().getName();
            
            if (context.ChildCount == 1)
            {
                if (typeStr.Equals("int"))
                {
                    // Console.WriteLine("请输入一个整数:");
                    outputStream?.Print("请输入一个整数:\n");
                    NeedInput?.Invoke();
                    Thread.CurrentThread.Suspend();
                    try
                    {
                        int rightValue = Int32.Parse(buffer);
                        symbol.setValue(rightValue);
                    }
                    catch (OverflowException e)
                    {
                        //需要细化处理，暂时这样
                        throw new Exception(e.Message);
                    }
                    catch (FormatException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else if (typeStr.Equals("real"))
                {
                   // Console.WriteLine("请输入一个实数:");
                    outputStream?.Print("请输入一个实数:\n");
                    NeedInput?.Invoke();
                    Thread.CurrentThread.Suspend();
                    try
                    {
                        decimal rightValue = decimal.Parse(buffer);
                        symbol.setValue(rightValue);
                    }
                    catch (OverflowException e)
                    {
                        throw new Exception(e.Message);
                    }
                    catch (FormatException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else if (typeStr.Equals("bool"))
                {
                    //Console.WriteLine("请输入一个bool值:");
                    outputStream?.Print("请输入一个bool值:\n");
                    NeedInput?.Invoke();
                    Thread.CurrentThread.Suspend();
                    try
                    {
                        bool rightValue = bool.Parse(buffer);
                        symbol.setValue(rightValue);
                    }
                    catch (FormatException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                object indexObj = Visit(context.expression());
                checkType("int", indexObj);
                int index = (int)indexObj;
                if (typeStr.Equals("int_arr")) 
                {
                    int[] arr = (int[])symbol.getValue();
                    indexCheck(arr.Length, index);
                   // Console.WriteLine("请输入一个整数：");
                    outputStream?.Print("请输入一个整数:\n");
                    NeedInput?.Invoke();
                    Thread.CurrentThread.Suspend();
                    try
                    {
                        int rightValue = Int32.Parse(buffer);
                        arr[index] = rightValue;
                    }
                    catch (OverflowException e)
                    {
                        throw new Exception(e.Message);
                    }
                    catch (FormatException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else if (typeStr.Equals("real_arr"))
                {
                    decimal[] arr = (decimal[])symbol.getValue();
                    indexCheck(arr.Length, index);
                   // Console.WriteLine("请输入一个实数:");
                    outputStream?.Print("请输入一个实数:\n");
                    NeedInput?.Invoke();
                    Thread.CurrentThread.Suspend();
                    try
                    {
                        decimal rightValue = decimal.Parse(buffer);
                        arr[index] = rightValue;
                    }
                    catch (OverflowException e)
                    {
                        throw new Exception(e.Message);
                    }
                    catch (FormatException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else if (typeStr.Equals("bool_arr")) 
                {
                    bool[] arr = (bool[])symbol.getValue();
                    indexCheck(arr.Length, index);
                   // Console.WriteLine("请输入一个bool值:");
                    outputStream?.Print("请输入一个bool值:\n");
                    NeedInput?.Invoke();
                    Thread.CurrentThread.Suspend();
                    try
                    {
                        bool rightValue = bool.Parse(buffer);
                        arr[index] = rightValue;
                    }
                    catch (FormatException e)
                    {
                        throw new Exception(e.Message);
                    }
                } 
                else { 
                    throw new Exception(); 
                }
            }
        }

        /*
         * write语句
         */
        public override object VisitWriteStatement([NotNull] CMMParser.WriteStatementContext context)
        {
            if (context.ChildCount == 4)
            {
                //Console.WriteLine();
                outputStream?.Print("\n");
            }
            else
            {
                CMMParser.ExpressionContext[] expressionContexts = context.expression();
                for(int i = 0; i < expressionContexts.Length; ++i)
                {
                    object rtValue = VisitExpression(expressionContexts[i]);
                    handleWrite(rtValue);
                }
            }
            return null;
        }

        private void handleWrite(object rtValue)
        {
            if (rtValue is int[])
            {
                // Console.Write("[");
                outputStream?.Print("[");
                Array.ForEach((int[])rtValue, (item) => outputStream?.Print(" " + item));
                outputStream?.Print(" ] ");
            }
            else if (rtValue is decimal[])
            {
                outputStream?.Print("[");
                Array.ForEach((decimal[])rtValue, (item) => outputStream?.Print(" " + item));
                outputStream?.Print(" ] ");
            }
            else if (rtValue is bool[])
            {
                outputStream?.Print("[");
                Array.ForEach((bool[])rtValue, (item) => outputStream?.Print(" " + item));
                outputStream?.Print(" ] ");
            }
            else
            {
                outputStream?.Print(rtValue+" ");
            }
        }

        public override object VisitCallStatement([NotNull] CMMParser.CallStatementContext context)
        {
            string id = context.Identifier().GetText();
            Symbol functionSymbol = globalScope.resolve(id);
            if(functionSymbol ==null || ! (functionSymbol is FunctionSymbol))
            {
                throw new RuntimeException("函数" + id + "未定义，" + "或者" + id + "不是一个函数！", context.Identifier().Symbol.Line);
            }
            LinkedHashMap<string,BuiltInTypeSymbol> orderedArgs = ((FunctionSymbol)functionSymbol).getOrderedArgs();
            List<object> parameters = null;
            if (context.expressionList() != null)
            {
                parameters = (List<object>)VisitExpressionList(context.expressionList());
            }

            Scope functionScope = new LocalScope(globalScope);
            Scope saveScope = currentScope;
            currentScope = functionScope;
            checkAndAssignParameters(orderedArgs, parameters);

            object result = null;
            try
            {
                enterFunctionScope = true;
                VisitCodeBlock(((FunctionSymbol)functionSymbol).getContext());
            }
            catch (ReturnValue r)
            {
                result = r.value;
            }
            currentScope = saveScope;
            return result;
        }

        private void checkAndAssignParameters(LinkedHashMap<string, BuiltInTypeSymbol> orderedArgs, List<object> parameters)
        {
            if (orderedArgs != null)
            {
                if (parameters == null || parameters.Count != orderedArgs.Count)
                {
                    throw new Exception();
                }
                ICollection<string> keys = orderedArgs.Keys;
                int i = 0;
                foreach(string key in keys)
                {
                    checkType(orderedArgs[key].getName(), parameters[i]);
                    Symbol symbol = new VariableSymbol(key, orderedArgs[key], parameters[i], currentScope);
                    currentScope.define(symbol);
                    ++i;
                }
            }
            else
            {
                if (parameters != null)
                {
                    throw new Exception();
                }
            }
        }

        public override object VisitExpressionList([NotNull] CMMParser.ExpressionListContext context)
        {
            List<object> values = new List<object>();
            if (context.ChildCount == 1)
            {
                values.Add(VisitExpression(context.expression()));
            }
            else
            {
                values.AddRange((List<object>)VisitExpressionList(context.expressionList()));
                values.Add(VisitExpression(context.expression()));
            }
            return values;
        }

        public override object VisitCodeBlock([NotNull] CMMParser.CodeBlockContext context)
        {
            Scope scope;
            //进入函数代码块不开启新的作用域，已经在函数调用的时候开启过
            if (enterFunctionScope)
            {
                enterFunctionScope = false;
                scope = currentScope;
            }
            else
            {
                scope = new LocalScope(currentScope);
            }
            currentScope = scope;
            VisitStatements(context.statements());
            currentScope = currentScope.getEnclosingScope();
            return null;
        }

        public override object VisitJumpStatement([NotNull] CMMParser.JumpStatementContext context)
        {
            if (context.GetChild(0).GetText().Equals("break"))
            {
                throw new KeyWordException("break");
            }
            else if (context.GetChild(0).GetText().Equals("continue")){
                throw new KeyWordException("continue");
            }
            else
            {
                if (context.ChildCount == 2)
                {
                    throw new ReturnValue();
                }
                else
                {
                    object value = VisitExpression(context.expression());
                    throw new ReturnValue(value);
                }
            }
        }

        public override object VisitExpression([NotNull] CMMParser.ExpressionContext context)
        {
            if (context.ChildCount == 1)
            {
                return VisitBoolExpression(context.boolExpression());
            }
            else if (context.ChildCount == 2)
            {
                return !(bool)VisitExpression(context.expression());
            }
            else if (context.GetChild(1).GetText().Equals("&&"))
            {
                return (bool)Visit(context.expression()) && (bool)Visit(context.boolExpression());
            }
            else
            {
                return (bool)Visit(context.expression()) || (bool)Visit(context.boolExpression());
            }
        }

        public override object VisitBoolExpression([NotNull] CMMParser.BoolExpressionContext context)
        {
            if (context.ChildCount == 3)
            {
                object left = Visit(context.additiveExpression(0));
                object right = Visit(context.additiveExpression(1));
                checkType("real", left);
                checkType("real", right);
                decimal a = 0;
                decimal b = 0;
                if(left is int) {
                    a = (int)left;
                }
                else
                {
                    a = (decimal)left;
                }
                if(right is int)
                {
                    b = (int)right;
                }
                else
                {
                    b = (decimal)right;
                }
                switch (context.relationalOperator().GetText())
                {
                    case "<=":
                        return a <= b;
                    case ">=":
                        return a >= b;
                    case "==":
                        return a == b;
                    case "<":
                        return a < b;
                    case ">":
                        return a > b;
                    case "<>":
                        return a != b;
                    default:
                        return false;
                }
            }
            else
            {
                return Visit(context.additiveExpression(0));
            }
        }

        public override object VisitAdditiveExpression([NotNull] CMMParser.AdditiveExpressionContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.term());
            }
            else
            {
                object left = Visit(context.additiveExpression());
                object right = Visit(context.term());
                checkType("real", left);
                checkType("real", right);
                bool isDecimal = left is decimal || right is decimal;
                int a = 0, b = 0;
                decimal c = 0, d = 0;
                if (!isDecimal)
                {
                    a = (int)left;
                    b = (int)right;
                    switch (context.@operator.Text)
                    {
                        case "+":
                            return a + b;
                        case "-":
                            return a - b;
                    }
                }
                else
                {
                    if(left is decimal)
                    {
                        c = (decimal)left;
                        if(right is decimal)
                        {
                            d = (decimal)right;
                        }
                        else
                        {
                            d = (int)right;
                        }
                    }
                    else
                    {
                        c = (int)left;
                        d = (decimal)right;
                    }
                    switch (context.@operator.Text)
                    {
                        case "+":
                            return c + d;
                        case "-":
                            return c - d;
                    }
                }
                return 0;
            }
        }

        public override object VisitTerm([NotNull] CMMParser.TermContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.factor());
            }
            else
            {
                object left = Visit(context.term());
                object right = Visit(context.factor());
                checkType("real", left);
                checkType("real", right);
                bool isDecimal = left is decimal || right is decimal;
                int a = 0, b = 0;
                decimal c = 0, d = 0;
                if (!isDecimal)
                {
                    a = (int)left;
                    b = (int)right;
                    switch (context.@operator.Text)
                    {
                        case "*":
                            return a * b;
                        case "/":
                            return a / b;
                    }
                }
                else
                {
                    if (left is decimal)
                    {
                        c = (decimal)left;
                        if (right is decimal)
                        {
                            d = (decimal)right;
                        }
                        else
                        {
                            d = (int)right;
                        }
                    }
                    else
                    {
                        c = (int)left;
                        d = (decimal)right;
                    }
                    switch (context.@operator.Text)
                    {
                        case "*":
                            return c * d;
                        case "/":
                            return c / d;
                    }
                }
                return 0;
            }
        }


        /*
         
         c = 3*3; // c -> mem[index0]
        // c -> 当前栈的位置sp0， sp++

        a = 0;
        // mem[index0] > 0
        // 去栈sp0的位置去找
        if(c > 0){
        }
        // 最后再把栈弹出 sp--；ｚ
         
         */

        
// mem的话，没有办法回收空间。相当于放在了堆里，但是没有gcZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ 
        public override object VisitFactor([NotNull] CMMParser.FactorContext context)
        {
            if (context.True() != null)
            {
                return true;
            }else if (context.False() != null)
            {
                return false;
            }
            else if (context.GetChild(0).GetText().Equals("("))
            {
                return Visit(context.expression());
            }
            else if (context.GetChild(0).GetText().Equals("-"))
            {
                object retValue = Visit(context.factor());
                if(retValue is int)
                {
                    return -(int)retValue;
                }
                else if(retValue is decimal)
                {
                    return -(decimal)retValue;
                }else
                {
                    throw new RuntimeException("负号只能用于int和real类型！",context.factor().Identifier().Symbol.Line);
                }
            }
            else if (context.IntegerLiteral() != null)
            {
                try
                {
                    return Int32.Parse(context.IntegerLiteral().GetText());
                 }
                catch (OverflowException e)
                {
                    throw new Exception(e.Message);
                }
                catch (FormatException e)
                {
                    throw new Exception(e.Message);
                }
            }
            else if (context.RealLiteral() != null)
            {
                try
                {
                    return decimal.Parse(context.RealLiteral().GetText());
                }
                catch (OverflowException e)
                {
                    throw new Exception(e.Message);
                }
                catch (FormatException e)
                {
                    throw new Exception(e.Message);
                }
            }
            else
            {
                Symbol symbol = currentScope.resolve(context.Identifier().GetText());
                if (symbol == null)
                {
                    //未定义？
                    throw new RuntimeException(context.Identifier().GetText() + "未定义！",context.Identifier().Symbol.Line);
                }
                if(context.ChildCount == 3)
                {
                    if (!(symbol is FunctionSymbol))
                    {
                        //不是一个函数
                        throw new RuntimeException( context.Identifier().GetText() + "不是一个函数！", context.Identifier().Symbol.Line);
                    }
                    FunctionSymbol function = (FunctionSymbol)symbol;
                    if (function.getType() == null)
                    {
                        //函数无返回值
                        throw new RuntimeException(context.Identifier().GetText() + "是无返回值函数！", context.Identifier().Symbol.Line);
                    }
                    if (function.getOrderedArgs() != null)
                    {
                        //参数不足
                        throw new RuntimeException("没有提供"+context.Identifier().GetText() + "所需要的参数！", context.Identifier().Symbol.Line);
                    }
                    Scope scope = new LocalScope(globalScope);
                    Scope saveScope = currentScope;
                    currentScope = scope;
                    object rtValue = null;
                    try
                    {
                        enterFunctionScope = true;
                        Visit(function.getContext());
                    }
                    catch (ReturnValue e)
                    {
                        rtValue = e.value;
                    }
                    currentScope = saveScope;
                    return rtValue;
                }
                else if (context.expressionList() != null)
                {
                    if (!(symbol is FunctionSymbol))
                    {
                        throw new RuntimeException(context.Identifier().GetText() + "不是一个函数！", context.Identifier().Symbol.Line);
                    }
                    FunctionSymbol function = (FunctionSymbol)symbol;
                    if (function.getType() == null)
                    {
                        throw new RuntimeException(context.Identifier().GetText() + "是无返回值函数！", context.Identifier().Symbol.Line);
                    }
                    List<object> parameters = (List<object>)Visit(context.expressionList());

                    Scope scope = new LocalScope(globalScope);
                    Scope saveScope = currentScope;
                    currentScope = scope;
                    checkAndAssignParameters(function.getOrderedArgs(), parameters);

                    object rtValue = null;
                    try
                    {
                        enterFunctionScope = true;
                        VisitCodeBlock(function.getContext());
                    }
                    catch (ReturnValue e)
                    {
                        rtValue = e.value;
                    }
                    currentScope = saveScope;
                    return rtValue;
                }
                else
                {
                    return load(context);
                }
            }
        }

        public override object VisitLeftValue([NotNull] CMMParser.LeftValueContext context)
        {
            return load(context);
        }

        private object load([NotNull] ParserRuleContext context)
        {
            CMMParser.LeftValueContext leftValueContext = null;
            CMMParser.FactorContext factorContext = null;
            if (context is CMMParser.LeftValueContext)
            {
                leftValueContext = (CMMParser.LeftValueContext)context;
            }
            else
            {
                factorContext = (CMMParser.FactorContext)context;
            }
            if (leftValueContext != null)
            {
                Symbol symbol = currentScope.resolve(leftValueContext.Identifier().GetText());
                if (symbol == null || !(symbol is VariableSymbol))
                {
                    throw new RuntimeException(symbol.getName() + "不存在或者不是symbol！", leftValueContext.Identifier().Symbol.Line);
                }
                if (leftValueContext.ChildCount == 1)
                {
                    return symbol.getValue();
                }
                else
                {
                    if (!(symbol.getValue() is Array))
                    {
                        throw new RuntimeException(symbol.getName()+"不是一个数组！", leftValueContext.Identifier().Symbol.Line);
                    }
                    object indexObj = Visit(leftValueContext.expression());
                    checkType("int", indexObj);
                    int index = (int)indexObj;
                    if (symbol.getType().getName().Equals("int_arr"))
                    {
                        int[] arr = (int[])symbol.getValue();
                        indexCheck(arr.Length, index);
                        return arr[index];
                    }
                    else if (symbol.getType().getName().Equals("real_arr"))
                    {
                        decimal[] arr = (decimal[])symbol.getValue();
                        indexCheck(arr.Length, index);
                        return arr[index];
                    }
                    else
                    {
                        bool[] arr = (bool[])symbol.getValue();
                        indexCheck(arr.Length, index);
                        return arr[index];
                    }
                }
            }
            else
            {
                Symbol symbol = currentScope.resolve(factorContext.Identifier().GetText());
                if (symbol == null || !(symbol is VariableSymbol))
                {
                    throw new RuntimeException(symbol.getName() + "不存在或者是不是symbol！", leftValueContext.Identifier().Symbol.Line);
                }
                if (factorContext.ChildCount == 1)
                {
                   return symbol.getValue();
                }
                else
                {
                    if (!(symbol.getValue() is Array))
                    {
                        throw new RuntimeException(symbol.getName() + "不是一个数组！", leftValueContext.Identifier().Symbol.Line);
                    }
                    object indexObj = Visit(factorContext.expression());
                    checkType("int", indexObj);
                    int index = (int)indexObj;
                    if (symbol.getType().getName().Equals("int_arr"))
                    {
                        int[] arr = (int[])symbol.getValue();
                        indexCheck(arr.Length, index);
                        return arr[index];
                    }
                    else if (symbol.getType().getName().Equals("real_arr"))
                    {
                        decimal[] arr = (decimal[])symbol.getValue();
                        indexCheck(arr.Length, index);
                        return arr[index];
                    }
                    else
                    {
                        bool[] arr = (bool[])symbol.getValue();
                        indexCheck(arr.Length, index);
                        return arr[index];
                    }
                }
            }
        }
    }
}
