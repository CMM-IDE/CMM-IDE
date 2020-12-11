using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.inter
{
    class LocalScope : BaseScope
    {
        public LocalScope(Scope enclosingScope) : base(enclosingScope)
        {
        }

        public string getScopeName()
        {
            return "local";
        }
    }
}
