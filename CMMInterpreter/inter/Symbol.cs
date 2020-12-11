
using CMMInterpreter.inter;

namespace CMMInterpreter
{
    class Symbol
    {
        string name;
        Type type;
        Scope scope;
        object value;

        public string getName()
        {
            return name;
        }

        public Type getType()
        {
            return type;
        }

        public Scope getScope()
        {
            return scope;
        }

        public object getValue()
        {
            return value;
        }

        public Symbol(string name)
        {
            this.name = name;
        }

        /*
          * 变量构造函数
          */
        public Symbol(string name,Type retType)
        {
            this.name = name;
            this.type = retType;
        }

        /*
         * 变量构造函数
         */
        public Symbol(string name, Type type,Scope scope)
        {
            this.name = name;
            this.scope = scope;
            this.type = type;
        }

        /*
         * 变量构造函数
         */
        public Symbol(string name,Type type,object value, Scope scope)
        {
            this.name = name;
            this.scope = scope;
            this.type = type;
            this.value = value;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public void setType(Type type)
        {
            this.type = type;
        }

        public void setScope(Scope scope)
        {
            this.scope = scope;
        }

        public void setValue(object value)
        {
            this.value = value;
        }
    }
}
