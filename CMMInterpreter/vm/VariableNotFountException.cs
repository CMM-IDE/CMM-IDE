using System;
using System.Runtime.Serialization;

namespace CMMInterpreter.vm
{
    public class VariableNotFountException : Exception
    {
        private Antlr4.Runtime.ParserRuleContext _context;
        private string _variable;

        public VariableNotFountException()
        {
        }

        public VariableNotFountException(string message) : base(message)
        {
        }

        public VariableNotFountException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public VariableNotFountException(string variable, Antlr4.Runtime.ParserRuleContext context)
        {
            this._context = context;
            this._variable = variable;
        }

        protected VariableNotFountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        
        public string ToString()
        {
            return _variable + " 没有定义。起始位置为 " + _context.Start + " 结束位置为：" + _context.Stop;
        }
    }
}