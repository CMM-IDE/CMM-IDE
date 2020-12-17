using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CMMInterpreter.CMMException
{
   public class CMMStaticErrorHandler
    {
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
            parser.RemoveErrorListeners();
            CMMErrorListener errorListener = new CMMErrorListener();
            errorListener.outputStream = this.outputStream;
            parser.AddErrorListener(errorListener);
            CMMErrorStrategy errorStrategy = new CMMErrorStrategy();
            parser.ErrorHandler = errorStrategy;
                parser.statements();
                }
                catch (RuntimeException e1)
                {
                    outputStream?.Print("Line:" + e1.line.ToString() + " " + e1.Message);
                    //Print(e1.Message);
                }
                catch (Exception e2)
                {
                //outputStream?.Print(e2.Message);
                }
            
        }
    }
}
