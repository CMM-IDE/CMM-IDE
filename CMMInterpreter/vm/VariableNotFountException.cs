using CMMInterpreter.CMMException;
using System;
using System.Runtime.Serialization;

namespace CMMInterpreter.vm
{
    public class VariableNotFountException : Exception
    {
        private Antlr4.Runtime.ParserRuleContext _context;
        private string _variable;
        private ErrorInfo _errorInfo;

        public ErrorInfo Error => _errorInfo;

        public VariableNotFountException(string variable, Antlr4.Runtime.ParserRuleContext context)
        {
            this._context = context;
            this._variable = variable;
            this._errorInfo = new ErrorInfo(context.Start.Line, context.Start.Column, "列变量" + _variable + " 没有定义");

        }

        override public string ToString()
        {
            return _context.Start.Line + "行" + _context.Start.Column + "列变量"+_variable + " 没有定义";
        }
    }
}