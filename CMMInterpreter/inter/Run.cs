using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace CMMInterpreter.inter
{
   public class Run:IOutputStream
    {
        public Run() { }
        public void RunParser(String input)
        {
           // String example="write(\"CMMM语言while循环示例:\");int a = 10;while (a <> 0) {a = a - 1;write(a);}";
           // input = example;
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new CMMLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CMMParser parser = new CMMParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.statements();
            var visitor = new RefPhase();
            visitor.outputStream = this;
            visitor.Visit(tree);
        }

        public void Print(string s)
        {
            Console.WriteLine(s);
        }
    }
}
