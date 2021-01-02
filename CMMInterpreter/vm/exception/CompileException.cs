using System;
using System.Runtime.Serialization;

namespace CMMInterpreter.vm.exception
{
    public class CompileException : Exception
    {
        public CompileException(string message) : base(message) {

        }

    }
}