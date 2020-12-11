using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.inter
{
    class ReturnValue : Exception
    {
        public object value;

        public ReturnValue() { }

        public ReturnValue(object value)
        {
            this.value = value;
        }
    }
}
