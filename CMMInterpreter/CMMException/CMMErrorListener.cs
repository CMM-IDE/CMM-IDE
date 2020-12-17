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
  public  class CMMErrorListener : BaseErrorListener
    {
        public IOutputStream outputStream = null;
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            IList<String> stack = ((Parser)recognizer).GetRuleInvocationStack();
            stack.Reverse();
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.Append("syntax error:\n");
            errorMsg.Append("line" + line + ":" + charPositionInLine + ": " + msg+"\n");
            outputStream?.Print(errorMsg.ToString());
        }
    }
}