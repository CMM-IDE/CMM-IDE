using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.CMMException
{
    public class ErrorInfo:Exception
    {
        public int line;
        public int charPositionInLine;
        public ErrorInfo(String msg,int line,int positionInLine):base(msg)
        {
            this.line = line;
            this.charPositionInLine = positionInLine;
        }
    }
}
