using AduSkin.Controls.Metro;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CMMInterpreter.CMMException;
using CMMInterpreter.inter;
using System;
using System.Windows;


namespace IDE_UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow, IOutputStream
    {
        public MainWindow()
        {
            InitializeComponent();
            textBox.FontSize = 12;
        }


        private void loadSample_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Text = "write(\"CMMM语言while循环示例:\");\nnumber a = 10;\nwhile (a <> 0) {\n\ta = a - 1;\n\twrite(a + \" \");\n}";
        }

        private void run_Click(object sender, RoutedEventArgs e)
        {
            /*textBox.Text = "";
            String input = textEditor.Text;
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new CMMMLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CMMMParser parser = new CMMMParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.program();
            var visitor = new TestVisitor();
            visitor.outputStream = this;
            visitor.Visit(tree);*/
            try
            {
                textBox.Text = "";
                String input = textEditor.Text;
                ICharStream stream = CharStreams.fromstring(input);
                ITokenSource lexer = new CMMLexer(stream);
                ITokenStream tokens = new CommonTokenStream(lexer);
                CMMParser parser = new CMMParser(tokens);
                parser.BuildParseTree = true;
                IParseTree tree = parser.statements();
                var visitor = new RefPhase();
                visitor.outputStream = this;
                visitor.Visit(tree);
            }catch(RuntimeException e1)
            {
                Print("Line:"+e1.line.ToString()+" "+e1.Message);
                //Print(e1.Message);
            }
            catch(Exception e2)
            {
                Print(e2.Message);
            }
        }

        public void Print(string s)
        {
            textBox.Text += s;
        }
    }
}
