using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.inter
{
    interface Scope
    {
        string getScopeName();

         Scope getEnclosingScope();

         void define(Symbol sym);

         Symbol resolve(string name);

        bool redundant(string name);

        void remove(string name);
    }
}
