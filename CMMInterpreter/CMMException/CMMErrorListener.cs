using Antlr4.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CMMInterpreter.CMMException
{
    //错误监听器
  public  class CMMErrorListener : BaseErrorListener, IAntlrErrorListener<int>
    {
        public IOutputStream outputStream = null;
        public IErrorShow errorShow = null;
        public CMMErrorListener() : base() { }
        public CMMErrorListener(IOutputStream output,IErrorShow errorShow):base()
        {
            this.outputStream = output;
            this.errorShow = errorShow;
        }

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            IList<String> stack = ((Parser)recognizer).GetRuleInvocationStack();
            stack.Reverse();
            StringBuilder errorMsg = new StringBuilder();
           // errorMsg.Append("syntax error:\n");
            errorMsg.Append("line" + line + ":" + charPositionInLine + ": " + msg+"\n");
            outputStream?.Print(errorMsg.ToString());
            errorShow.ShowErrorPositionUI(line, charPositionInLine);
           // throw new ErrorInfo(msg,line,charPositionInLine);
        }

        public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            StringBuilder errorMsg = new StringBuilder();
            // errorMsg.Append("syntax error:\n");
            errorMsg.Append("line" + line + ":" + charPositionInLine + ": " + msg + "\n");
            outputStream?.Print(errorMsg.ToString());
            errorShow.ShowErrorPositionUI(line, charPositionInLine);
        }
    }
}