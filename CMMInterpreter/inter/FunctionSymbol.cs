using CMMInterpreter.util;
using System.Collections.Generic;

namespace CMMInterpreter.inter
{
    class FunctionSymbol : Symbol
    {
        LinkedHashMap<string,BuiltInTypeSymbol> orderedArgs = new LinkedHashMap<string, BuiltInTypeSymbol>();
        CMMParser.CodeBlockContext context;
        public FunctionSymbol(string name, Type retType, Scope scope, LinkedHashMap<string, BuiltInTypeSymbol> orderedArgs, CMMParser.CodeBlockContext context) : base(name, retType, scope)
        {
            this.orderedArgs = orderedArgs;
            this.context = context;
        }

        public LinkedHashMap<string, BuiltInTypeSymbol> getOrderedArgs()
        {
            return orderedArgs;
        }

        public CMMParser.CodeBlockContext getContext()
        {
            return context;
        }
    }

    //    LinkedHashMap<string, Symbol> orderedArgs = new LinkedHashMap<string, Symbol>();
    //    Scope enclosingScope;
    //    CMMParser.CodeBlockContext context;

    //    public FunctionSymbol(string name, Type retType, Scope enclosingScope):base(name,retType)
    //    {
    //        this.enclosingScope = enclosingScope;
    //    }

    //    public void define(Symbol sym)
    //    {
    //        orderedArgs.Add(sym.getName(), sym);
    //    }

    //    public Scope getEnclosingScope()
    //    {
    //        return enclosingScope;
    //    }

    //    public bool redundant(string name)
    //    {
    //        Symbol symbol = resolve(name);
    //        return symbol != null && !symbol.getScope().getScopeName().Equals("global");
    //    }

    //    public Symbol resolve(string name)
    //    {
    //        Symbol s;
    //        if (orderedArgs.TryGetValue(name,out s))
    //        {
    //            return s;
    //        }

    //        if (getEnclosingScope() != null)
    //        {
    //            return enclosingScope.resolve(name);
    //        }
    //        return null;
    //    }

    //    public string getScopeName() { return getName(); }
    //}
}
