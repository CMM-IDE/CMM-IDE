using System;
using System.Collections.Generic;
using System.Text;
using CMMInterpreter.util;

namespace CMMInterpreter.inter
{
    abstract class BaseScope : Scope
    {
        private Scope enclosingScope;
        private Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

        public BaseScope(Scope enclosingScope)
        {
            this.enclosingScope = enclosingScope;
        }

        public void define(Symbol sym)
        {
            symbols.Add(sym.getName(), sym);
        }

        public Scope getEnclosingScope()
        {
            return enclosingScope;
        }

        public string getScopeName() {
            return "";
         }

        public bool redundant(string name)
        {
            Symbol symbol = resolve(name);
            return symbol != null && !symbol.getScope().getScopeName().Equals("global");
        }

        public void remove(string name)
        {
            symbols.Remove(name);
        }

        public Symbol resolve(string name)
        {
            Symbol s;
            if (symbols.TryGetValue(name, out s))
            {
                return s;
            }
            if (enclosingScope!= null)
            {
                return enclosingScope.resolve(name);
            }
            return null;
        }
    }
}
