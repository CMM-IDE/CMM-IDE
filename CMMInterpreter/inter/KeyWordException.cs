using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.inter
{
    class KeyWordException : Exception
    {
        public KeyWordException(string msg) : base(msg) { }
    }
}
