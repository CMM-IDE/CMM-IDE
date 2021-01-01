using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.CMMException
{
    //暂时没用
    public class ErrorInfo
    {
        public int Line { get; set; }

        public int CharPositionInLine { get; set; }

        public string Message { get; set; }

        public ErrorInfo(int line,int positionInLine, string message)
        {
            this.Line = line;
            this.CharPositionInLine = positionInLine;
            this.Message = message;
        }

        public override string ToString()
        {
            return "line" + Line + ":" + CharPositionInLine + ": " + Message;
        }
    }
}
