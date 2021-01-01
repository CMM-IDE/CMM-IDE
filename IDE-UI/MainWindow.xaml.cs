using Antlr4.Runtime;
using IDE_UI.DrawTreeGraph;
using CMMInterpreter.inter;
using System;
using System.Windows;
using System.Windows.Controls;
using IDE_UI.Helper;
using System.Threading;
using CMMInterpreter.vm;
using IDE_UI.Controls;
using Antlr4.Runtime.Tree;

namespace IDE_UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IOutputStream
    {
        public MainWindow()
        {
            InitializeComponent();
            AllowsTransparency = false;
            this.DataContext = this;
            init();
        }

        /// <summary>
        /// 虚拟控制台
        /// </summary>
        private TextBox consoleTextBox;

        /// <summary>
        /// 调试面板
        /// </summary>
        private DebugPanel debugPanel;

        /// <summary>
        /// 调试面板
        /// </summary>
        private ErrorPanel errorPanel;

        /// <summary>
        /// 画树面板
        /// </summary>
        private DrawTreePanel drawTreePanel;

        /// <summary>
        /// 虚拟终端是否在输入
        /// </summary>
        private bool isInputMode = false;

        /// <summary>
        /// 虚拟终端输入的长度
        /// </summary>
        private int inputLength = 0;

        /// <summary>
        /// IDE状态信息
        /// </summary>
        public IDEState State { get; set; } = new IDEState();

        private Thread runnerThread = null;

        private RefPhase visitor = null;



        private void run_Click(object sender, RoutedEventArgs e)
        {

            if(!State.ConsoleShowed) {
                extraPanelButton_Click(btnConsoleWindow, null);
            }
            
            consoleTextBox.Text = "";
            String input = textEditor.Text;
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new CMMLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CMMParser parser = new CMMParser(tokens);
            parser.BuildParseTree = true;

            

            IParseTree tree = parser.statements();

            IParseTreeGrapher parseTreeGrapher = new ParseTreeGrapher();
            var graph = parseTreeGrapher.CreateGraph(tree, CMMParser.ruleNames);

            drawTreePanel.Graph = graph;

            var visitor = new CompileVisitor();
            try
            {
                visitor.Visit(tree);
                foreach (IntermediateCode code in visitor.codes)
                {
                    Print(code.toString());
                }
            }
            catch(VariableNotFountException exp)
            {
                Print(exp.ToString());
            }

            VirtualMachine vm = new VirtualMachine();

            Print("最终结果为：" + vm.interpret(visitor.codes).ToString());
            /*
            var visitor = new RefPhase();
            this.visitor = visitor;
            visitor.outputStream = this;
            visitor.NeedInput += handleNeedInput;

            runnerThread = new Thread(() => {

                try {
                    visitor.Visit(tree);
                    Print("\nprogram exit\n");
                }
                catch (RuntimeException e1) {
                    Print("Line:" + e1.line.ToString() + " " + e1.Message);
                }
                catch (Exception e2) {
                    Print(e2.Message);
                }
            });

            runnerThread.Start();
            */
        }

        public void Print(string s)
        {
            Dispatcher.Invoke(() => {
                consoleTextBox.Text += s;
            });
        }

        private void init()
        {
            consoleTextBox = new TextBox();
            consoleTextBox.FontSize = 14;
            consoleTextBox.AcceptsReturn = true;
            consoleTextBox.TextWrapping = TextWrapping.Wrap;
            consoleTextBox.TextChanged += TextChangedEventHandler;
            consoleTextBox.KeyUp += consoleTextBox_KeyUp;
            consoleTextBox.KeyDown += ConsoleTextBox_KeyDown;
            consoleTextBox.PreviewKeyDown += ConsoleTextBox_PreviewKeyDown;
            consoleTextBox.IsReadOnlyCaretVisible = false;
            consoleTextBox.IsReadOnly = true;

            debugPanel = new DebugPanel();

            drawTreePanel = new DrawTreePanel();

            errorPanel = new ErrorPanel();
        }


    }
}
