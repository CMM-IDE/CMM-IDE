using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.CMMException
{
    //错误处理策略
   public class CMMErrorStrategy:DefaultErrorStrategy
    {
        /* public override void Recover(Parser recognizer, RecognitionException e)
         {
             throw e;
         }*/
        public override IToken RecoverInline(Parser recognizer)
        {
            throw new InputMismatchException(recognizer);
        }
        public override void Sync(Parser recognizer)
        {

        }
        protected override void ReportNoViableAlternative(Parser recognizer, NoViableAltException e)
        {
            ITokenStream tokens = ((ITokenStream)recognizer.InputStream);
            string input;
            if (tokens != null)
            {
                if (e.StartToken.Type == TokenConstants.EOF)
                {
                    input = "<EOF>";
                }
                else
                {
                    input = tokens.GetText(e.StartToken, e.OffendingToken);
                }
            }
            else
            {
                input = "<unknown input>";
            }
            string msg = "无法决定采用哪一条匹配路径 " + EscapeWSAndQuote(input);
            NotifyErrorListeners(recognizer, msg, e);
        }
        protected override void ReportInputMismatch(Parser recognizer, InputMismatchException e)
        {
            string msg = "字符缺少匹配 " + GetTokenErrorDisplay(e.OffendingToken) + " 期望匹配的字符： " + e.GetExpectedTokens().ToString(recognizer.Vocabulary);
            NotifyErrorListeners(recognizer, msg, e);
        }
        protected override void ReportFailedPredicate(Parser recognizer, FailedPredicateException e)
        {
            string ruleName = recognizer.RuleNames[recognizer.RuleContext.RuleIndex];
            string msg = "规则 " + ruleName + " " + e.Message;
            NotifyErrorListeners(recognizer, msg, e);
        }
        protected override void ReportUnwantedToken(Parser recognizer)
        {
            if (InErrorRecoveryMode(recognizer))
            {
                return;
            }
            BeginErrorCondition(recognizer);
            IToken t = recognizer.CurrentToken;
            string tokenName = GetTokenErrorDisplay(t);
            IntervalSet expecting = GetExpectedTokens(recognizer);
            string msg = "无效的输入 " + tokenName + " expecting " + expecting.ToString(recognizer.Vocabulary);
            recognizer.NotifyErrorListeners(t, msg, null);
        }
        protected override void ReportMissingToken(Parser recognizer)
        {
            if (InErrorRecoveryMode(recognizer))
            {
                return;
            }
            BeginErrorCondition(recognizer);
            IToken t = recognizer.CurrentToken;
            IntervalSet expecting = GetExpectedTokens(recognizer);
            string msg = "缺少 " + expecting.ToString(recognizer.Vocabulary) + " 在 " + GetTokenErrorDisplay(t);
            recognizer.NotifyErrorListeners(t, msg, null);
        }
    }
}
