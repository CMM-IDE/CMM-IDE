using System;
using System.Runtime.Serialization;

namespace CMMInterpreter.vm
{
    internal class ArgumentNotDefinedException : Exception
    {
        private CMMParser.LeftValueContext context;

        public ArgumentNotDefinedException()
        {
        }

        public ArgumentNotDefinedException(string message) : base(message)
        {
        }

        public ArgumentNotDefinedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ArgumentNotDefinedException(string message, CMMParser.LeftValueContext context) : this(message)
        {
            this.context = context;
        }

        protected ArgumentNotDefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}