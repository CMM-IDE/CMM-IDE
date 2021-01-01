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

using CMMInterpreter.debuger;
using System.Collections.Generic;
using CMMInterpreter.CMMException;

namespace IDE_UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IOutputStream, VirtualMachineListener
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

        private CMMDebuger cmmDebuger;

        private Thread debugThread;

        private bool isDebug = false;

        private Thread runnerThread = null;

        private IdleExec idleExec = new IdleExec(2);


        /// <summary>
        /// 准备运行代码
        /// </summary>
        private IParseTree prepareForRunning()
        {
            String input = textEditor.Text;
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new CMMLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CMMParser parser = new CMMParser(tokens);
            parser.RemoveErrorListeners();


            var listener = new CMMErrorListener();
            parser.AddErrorListener(listener);
            parser.BuildParseTree = true;
            IParseTree tree = parser.statements();

            if(listener.errors.Count != 0) {

                if (!State.ErrorWindowShowed) {
                    errorPanel.Errors = listener.errors;
                    extraPanelButton_Click(btnErrorWindow, null);
                }

                textEditor.Errors = listener.errors;
                return null;
            }
            return tree;
        }

        /// <summary>
        /// 运行代码
        /// </summary>
        private void run_Click(object sender, RoutedEventArgs e)
        {
            IParseTree tree = prepareForRunning();
            if (tree == null) {
                return;
            }

            if (!State.ConsoleShowed) {
                extraPanelButton_Click(btnConsoleWindow, null);
            }

            consoleTextBox.Text = "";

            var graph = new ParseTreeGrapher().CreateGraph(tree, CMMParser.ruleNames);
            drawTreePanel.Graph = graph;

            var visitor = new CompileVisitor();
            try {
                visitor.generateCodes(tree);
                for (int i = 0; i < visitor.codes.Count; i++) {
                    Print(i + ":" + visitor.codes[i].toString());
                }

            }
            catch (VariableNotFountException exp) {
                Print(exp.ToString());
            }

            VirtualMachine vm = new VirtualMachine();
            vm.register(this);
            vm.interpret(visitor.codes);

        }

        public void Print(string s)
        {
            Dispatcher.Invoke(() => {
                consoleTextBox.Text += s;
                consoleTextBox.ScrollToEnd();
            });
        }

        public void write(Object o)
        {
            Print(o.ToString() + "\n");
        }

        /// <summary>
        /// 调试代码
        /// </summary>
        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            if (!State.DebugWindowShowed)
            {
                extraPanelButton_Click(btnDebugWindow, null);
            }

            consoleTextBox.Text = "";
            String input = textEditor.Text;
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new CMMLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CMMParser parser = new CMMParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.statements();

            var graph = new ParseTreeGrapher().CreateGraph(tree, CMMParser.ruleNames);
            drawTreePanel.Graph = graph;

            var visitor = new CompileVisitor();
            try
            {
                visitor.generateCodes(tree);
            }
            catch (VariableNotFountException exp)
            {
                Print(exp.ToString());
            }

            // 断点列表
            List<int> breakpoints = textEditor.GetBreakPoints();

            // 初始化调试器
            cmmDebuger = new CMMDebuger(visitor.codes, breakpoints);
            cmmDebuger.LoadDebugInformation(visitor.GetGlobalSymbolTable(), visitor.GetFunctionSymbolTable(),visitor.GetFunctionAddressTable());
            cmmDebuger.setListener(this);
            cmmDebuger.OutputStream = this;
            cmmDebuger.NeedDebug += HandlerDebug;
            cmmDebuger.DebugFinish += CmmDebugerDebugFinish;

            debugThread = new Thread(() => {
                try
                {
                    Print("\n调试模式\n");
                    isDebug = true;
                    cmmDebuger.Run();
                    Print("\nprogram exit\n");
                    isDebug = false;
                }
                catch (RuntimeException e1)
                {
                    Print("Line:" + e1.line.ToString() + " " + e1.Message);
                }
                catch (Exception e2)
                {
                    Print(e2.Message);
                }
            });

            debugThread.Name = "Debug";
            debugThread.Start();
        }

        /// <summary>
        /// 处理调试
        /// </summary>
        private void HandlerDebug()
        {
            Dispatcher.Invoke(() => {
                // 获取行号信息
                consoleTextBox.Text += "\nCurrentLine: " + cmmDebuger.GetCurrentLine().ToString() + "\n";
                
                List<FrameInformation> informations = cmmDebuger.GetCurrentFrame();
                List<StackFrameInformation> stackFrames = cmmDebuger.GetStackFrames();

                debugPanel.InDebugMode = true;
                debugPanel.StackFrameSymbols = stackFrames;
                textEditor.CurrentDebugLine = cmmDebuger.GetCurrentLine() - 1;

                // 获取当前栈帧
                consoleTextBox.Text += "\n---Current Frame Stack---\nAddress\tName\tValue\n";
                foreach (FrameInformation information in informations)
                {
                    consoleTextBox.Text += information.Address + "\t" + information.Name + "\t" + information.Value + "\n";
                }

                // 获取调用栈
                consoleTextBox.Text += "\n---Call Frame Stack---\n";
                foreach (StackFrameInformation information in stackFrames)
                {
                    consoleTextBox.Text += information.Name + "\t" + information.Line + "\nAddress\tName\tValue\n";
                    foreach (FrameInformation frame in information.Frame)
                    {
                        consoleTextBox.Text += frame.Address + "\t" + frame.Name + "\t" + frame.Value + "\n";
                    }
                }
            });
        }

        /// <summary>
        /// 处理调试面板的调试操作
        /// </summary>
        private void handleRequireDebugAction(DebugOperation oper)
        {
            if (!isDebug) {
                return;
            }
            switch (oper) {
                case DebugOperation.Continue:
                    cmmDebuger.Continue();
                    break;
                case DebugOperation.StepOver:
                    cmmDebuger.StepOver();
                    break;
                case DebugOperation.StepInto:
                    cmmDebuger.StepInto();
                    break;
                case DebugOperation.StepOut:
                    break;
            }
            debugThread.Resume();
        }

        //private void btnStepInto_Click(object sender, RoutedEventArgs e)
        //{
        //    if (isDebug)
        //    {
        //        cmmDebuger.StepInto();
        //        debugThread.Resume();
        //    }
        //}

        //private void btnStepOver_Click(object sender, RoutedEventArgs e)
        //{
        //    if (isDebug)
        //    {
        //        cmmDebuger.StepOver();
        //        debugThread.Resume();
        //    }
        //}

        //private void btnContinue_Click(object sender, RoutedEventArgs e)
        //{
        //    if (isDebug)
        //    {
        //        cmmDebuger.Continue();
        //        debugThread.Resume();
        //    }
        //}

        /// <summary>
        /// 处理调试结束
        /// </summary>
        private void CmmDebugerDebugFinish()
        {
            Dispatcher.Invoke(() => {
                isDebug = false;
                debugPanel.InDebugMode = false;
                textEditor.ClearDebugMarker();
                cmmDebuger = null;
            });
        }


        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (isDebug) {
                cmmDebuger.Stop();
                debugThread.Resume();
                CmmDebugerDebugFinish();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            textEditor.clearHover();
            Application.Current.Shutdown();
        }
    }
}