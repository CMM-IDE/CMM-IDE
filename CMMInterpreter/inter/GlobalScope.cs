using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.inter
{
    class GlobalScope : BaseScope
    {
        public GlobalScope(Scope enclosingScope) : base(enclosingScope)
        {
        }

        public string getScopeName()
        {
            return "global";
        }
    }
}
