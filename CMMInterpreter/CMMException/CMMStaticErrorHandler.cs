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
         * ErroInfo erroInfo=handler.StaticError(输入字符);存在错误返回一个ErrorInfo对象，否则返回null；
         */
        public IOutputStream outputStream;
        public CMMStaticErrorHandler(IOutputStream outputStream)
        {
            this.outputStream = outputStream;
        }
        public ErrorInfo StaticError(String input)
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
                return null;
            }
            catch (ErrorInfo e1)
            {
                return e1;
            }
            catch (Exception)
            {
                return null;
            }
            
        }
    }
}
