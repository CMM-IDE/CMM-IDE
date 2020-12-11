using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.inter
{
    class BuiltInTypeSymbol : Symbol, Type
    {
        public BuiltInTypeSymbol(string name) : base(name) { }
    }
}
