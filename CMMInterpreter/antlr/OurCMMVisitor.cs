using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.antlr
{
	public class VarValue
	{
		public int StartIndex;
		public object Value;

		public VarValue(int startIndex, object value)
		{
			StartIndex = startIndex;
			Value = value;
		}
	}
	public class OurCMMVisitor:CMMBaseVisitor<object>
    {

	}
}
