using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.CMMException
{
   public class CMMErrorStrategy:DefaultErrorStrategy
    {
        public override void Recover(Parser recognizer, RecognitionException e)
        {
            throw e;
        }
        public override IToken RecoverInline(Parser recognizer)
        {
            throw new InputMismatchException(recognizer);
        }
        public override void Sync(Parser recognizer)
        {
        }
    }
}
