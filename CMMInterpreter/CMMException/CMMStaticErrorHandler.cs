using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CMMInterpreter.CMMException
{
    //静态错误检查类，只能检查词法语法错误
   public class CMMStaticErrorHandler
    {
        public IOutputStream outputStream;
        public IErrorShow error;
        public CMMStaticErrorHandler(IOutputStream outputStream,IErrorShow errorShow)
        {
            this.outputStream = outputStream;
            this.error = errorShow;
        }
        public void StaticError(String input)
        {
            try {
                ICharStream stream = CharStreams.fromstring(input);
                ITokenSource lexer = new ExceptionLexer(stream, outputStream, error);
                ITokenStream tokens = new CommonTokenStream(lexer);
                CMMParser parser = new CMMParser(tokens);
                parser.BuildParseTree = true;
                parser.RemoveErrorListeners();
                CMMErrorListener errorListener = new CMMErrorListener();
                errorListener.outputStream = outputStream;
                errorListener.errorShow = error;
                parser.AddErrorListener(errorListener);
                CMMErrorStrategy errorStrategy = new CMMErrorStrategy();
                parser.ErrorHandler = errorStrategy;
                parser.statements();
            }
            catch (Exception e1) {
                outputStream?.Print(e1.Message);
            }

        }
    }
}
