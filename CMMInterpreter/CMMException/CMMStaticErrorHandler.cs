using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CMMInterpreter.CMMException
{
   public class CMMStaticErrorHandler
    {
        /*
         * 调用实例：
         * CMMStaticErrorHandler handler=new CMMStaticErrorHandler(IOutputstream对象);
         * handler.StaticError(输入字符);
         */
        public IOutputStream outputStream;
        public CMMStaticErrorHandler(IOutputStream outputStream)
        {
            this.outputStream = outputStream;
        }
        public void StaticError(String input)
        {
            try{
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new CMMLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CMMParser parser = new CMMParser(tokens);
            parser.BuildParseTree = true;
            parser.RemoveErrorListeners();//移除默认监听器
            CMMErrorListener errorListener = new CMMErrorListener();
            errorListener.outputStream = this.outputStream;
            parser.AddErrorListener(errorListener);//添加自定义错误监听器
            CMMErrorStrategy errorStrategy = new CMMErrorStrategy();
            parser.ErrorHandler = errorStrategy;//添加自定义错误策略
                parser.statements();
                }
                catch (RuntimeException e1)
                {
                    outputStream?.Print("Line:" + e1.line.ToString() + " " + e1.Message);
                }
                catch (Exception)
                {
                }
            
        }
    }
}
