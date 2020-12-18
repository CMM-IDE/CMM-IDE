using System;
using System.Runtime.Serialization;

namespace CMMInterpreter.vm
{
    internal class VariableNotFountException : Exception
    {
        private Antlr4.Runtime.ParserRuleContext context;

        public VariableNotFountException()
        {
        }

        public VariableNotFountException(string message) : base(message)
        {
        }

        public VariableNotFountException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public VariableNotFountException(string message, Antlr4.Runtime.ParserRuleContext context) : this(message)
        {
            this.context = context;
        }

        protected VariableNotFountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}