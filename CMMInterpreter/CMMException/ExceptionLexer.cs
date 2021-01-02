using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.CMMException
{
    //修改了错误处理的词法分析器
    public class ExceptionLexer : CMMLexer
    {
        public ExceptionLexer(ICharStream input, IAntlrErrorListener<int> listener) : base(input)
        {
            IAntlrErrorListener<int> errorListener = listener;
            this.RemoveErrorListeners();
            this.AddErrorListener(errorListener);
        }
        public ExceptionLexer(ICharStream input,IOutputStream outputStream,IErrorShow errorShow) : base(input)
        {
            IAntlrErrorListener<int> errorListener = new CMMErrorListener(outputStream,errorShow);
            this.RemoveErrorListeners();
            this.AddErrorListener(errorListener);
        }
        public override string GetErrorDisplay(string s)
        {
            Console.WriteLine("Test:" + s);
            Console.WriteLine(TokenStartLine);
            Console.WriteLine(TokenStartColumn);
            return base.GetErrorDisplay(s);

        }
        public override void Recover(LexerNoViableAltException e)
        {
            Console.WriteLine("调用我了!");
            base.Recover(e);
        }
        public override void NotifyListeners(LexerNoViableAltException e)
        {
            Console.WriteLine("测试位置");
            //string text = _input.GetText(Interval.Of(_tokenStartCharIndex, _input.Index));
            ICharStream _input = (ICharStream)InputStream;
            string text = _input.GetText(Interval.Of(TokenStartCharIndex, _input.Index));
            Console.WriteLine(text);
            string msg = "无法识别的字符: '" + GetErrorDisplay(text) + "'";
            IAntlrErrorListener<int> listener = ErrorListenerDispatch;
            listener.SyntaxError(ErrorOutput, this, 0, Line, Column, msg, e);
        }
        public override string GetErrorDisplay(int c)
        {
            Console.WriteLine("intDisplay");
            return base.GetErrorDisplay(c);
        }
        public override string GetCharErrorDisplay(int c)
        {
            Console.WriteLine("CharDisplay");
            return base.GetCharErrorDisplay(c);
        }
    }
}
