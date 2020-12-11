using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.inter
{
    class VariableSymbol : Symbol
    {
        public VariableSymbol(string name,Type type,Scope scope) : base(name, type, scope) { }

        public VariableSymbol(string name, Type type,object value,Scope scope) : base(name, type,value,scope) { }
    }
}
