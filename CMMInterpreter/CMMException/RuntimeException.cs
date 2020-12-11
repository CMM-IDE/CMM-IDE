using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.CMMException
{
    public class RuntimeException:Exception
    {
        public int line { get; set; }
        public RuntimeException(string msg,int line) : base(msg)
        {
            this.line = line;
        }
    }
}
