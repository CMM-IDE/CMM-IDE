using CMMInterpreter.CMMException;
using System;
using System.Runtime.Serialization;

namespace CMMInterpreter.vm
{
    public class VariableRedefinedException : Exception
    {
        private string v;
        private CMMParser.InitializerContext context;
        private ErrorInfo errorInfo;

        public ErrorInfo Error => errorInfo;

        public VariableRedefinedException()
        {
        }


        public VariableRedefinedException(string v, CMMParser.InitializerContext context)
        {
            this.v = v;
            this.context = context;
            errorInfo = new ErrorInfo(context.Start.Line, context.Start.Column, "列变量" + v + "多次定义");
        }

        override public string ToString()
        {
            return context.Start.Line + "行" + context.Start.Column + "列变量" + v + "多次定义";
        }

    }
}