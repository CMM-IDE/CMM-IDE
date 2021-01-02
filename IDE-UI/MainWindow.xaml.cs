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
using System.Diagnostics;

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
        /// 中间代码面板
        /// </summary>
        private CodeWindow codeWindow = new CodeWindow();

        private string intermediateCode;

        /// <summary>
        /// 虚拟控制台
        /// </summary>
        private TextBox consoleTextBox;

        private HintControl hintControl = new HintControl();

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
            errorPanel.Errors = null;
            textEditor.Errors = null;

            String input = textEditor.Text;
            if (String.IsNullOrEmpty(input)) {
                return null;
            }

            var listener = new CMMErrorListener();


            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new ExceptionLexer(stream, listener);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CMMParser parser = new CMMParser(tokens);

            Debug.WriteLine(tokens.Size);
            parser.RemoveErrorListeners();

            
            parser.AddErrorListener(listener);
            parser.ErrorHandler = new CMMErrorStrategy();

            parser.BuildParseTree = true;
            IParseTree tree = parser.statements();

            //如果出错，转到错误面板并返回null
            if(listener.errors.Count != 0) {
                Debug.WriteLine(listener.errors.Count);
                if (!State.ErrorWindowShowed) {
                    extraPanelButton_Click(btnErrorWindow, null);
                }
                errorPanel.Errors = listener.errors;
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

                //中间代码存为字符串。
                intermediateCode = "";
                for (int i = 0; i < visitor.codes.Count; i++) {
                    string newLine = i + ":\t" + visitor.codes[i].toString() + "\n";
                    intermediateCode += newLine;
                }
            }
            catch (VariableNotFountException exp) {
                Print(exp.ToString());
                intermediateCode = null;
            }

            VirtualMachine vm = new VirtualMachine();
            vm.register(this);
            vm.interpret(visitor.codes);

        }

        /// <summary>
        /// 准备调试
        /// </summary>
        private void prepareForDebug()
        {

            if (!State.DebugWindowShowed) {
                extraPanelButton_Click(btnDebugWindow, null);
            }
            debugPanel.consolePresenter.Content = consoleTextBox;

            consoleTextBox.Text = "";
        }

        /// <summary>
        /// 调试代码
        /// </summary>
        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {

            IParseTree tree = prepareForRunning();
            if (tree == null) {
                return;
            }

            prepareForDebug();

            var graph = new ParseTreeGrapher().CreateGraph(tree, CMMParser.ruleNames);
            drawTreePanel.Graph = graph;

            var visitor = new CompileVisitor();
            try {
                visitor.generateCodes(tree);

                //中间代码存为字符串。
                intermediateCode = "";
                for (int i = 0; i < visitor.codes.Count; i++) {
                    string newLine = i + ":\t" + visitor.codes[i].toString() + "\n";
                    intermediateCode += newLine;
                }
            }
            catch (VariableNotFountException exp) {
                Print(exp.ToString());
                intermediateCode = null;
            }

            // 断点列表
            List<int> breakpoints = textEditor.GetBreakPoints();

            // 初始化调试器
            cmmDebuger = new CMMDebuger(visitor.codes, breakpoints);
            cmmDebuger.LoadDebugInformation(visitor.GetGlobalSymbolTable(), visitor.GetFunctionInformations());
            cmmDebuger.setListener(this);
            cmmDebuger.OutputStream = this;
            cmmDebuger.NeedDebug += HandlerDebug;
            cmmDebuger.DebugFinish += CmmDebugerDebugFinish;

            debugThread = new Thread(() => {
                try {
                    //Print("\n调试模式\n");
                    isDebug = true;
                    cmmDebuger.Run();
                    Print("\nprogram exit\n");
                    isDebug = false;
                }
                catch (RuntimeException e1) {
                    Print("Line:" + e1.line.ToString() + " " + e1.Message);
                }
                catch (Exception e2) {
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
                //consoleTextBox.Text += "\nCurrentLine: " + cmmDebuger.GetCurrentLine().ToString() + "\n";
                
                List<FrameInformation> informations = cmmDebuger.GetCurrentFrame();
                List<StackFrameInformation> stackFrames = cmmDebuger.GetStackFrames();

                debugPanel.InDebugMode = true;
                debugPanel.StackFrameSymbols = stackFrames;
                textEditor.CurrentDebugLine = cmmDebuger.GetCurrentLine() - 1;

                //// 获取当前栈帧
                //consoleTextBox.Text += "\n---Current Frame Stack---\nAddress\tName\tValue\n";
                //foreach (FrameInformation information in informations)
                //{
                //    consoleTextBox.Text += information.Address + "\t" + information.Name + "\t" + information.Value + "\n";
                //}

                //// 获取调用栈
                //consoleTextBox.Text += "\n---Call Frame Stack---\n";
                //foreach (StackFrameInformation information in stackFrames)
                //{
                //    consoleTextBox.Text += information.Name + "\t" + information.Line + "\nAddress\tName\tValue\n";
                //    foreach (FrameInformation frame in information.Frame)
                //    {
                //        consoleTextBox.Text += frame.Address + "\t" + frame.Name + "\t" + frame.Value + "\n";
                //    }
                //}
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

        private void Window_Closing(object sender, EventArgs e)
        {
            NewFileItem_Click(null, null);
            debugThread?.Abort();
            textEditor?.clearHover();
            codeWindow?.Close();
            codeWindow = null;
            Application.Current.Shutdown();
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

        private void SelectAllItem_Click(object sender, RoutedEventArgs e)
        {
            textEditor.textEditor.SelectAll();
        }

        private void CutItem_Click(object sender, RoutedEventArgs e)
        {
            textEditor.textEditor.Cut();
        }

        private void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            textEditor.textEditor.Copy();
        }

        private void PasteItem_Click(object sender, RoutedEventArgs e)
        {
            textEditor.textEditor.Paste();
        }

        private void UndoItem_Click(object sender, RoutedEventArgs e)
        {
            textEditor.textEditor.Undo();
        }

        private void RedoItem_Click(object sender, RoutedEventArgs e)
        {
            textEditor.textEditor.Redo();
        }


        private void continueRuning_Click(object sender, RoutedEventArgs e)
        {
            if(isDebug) {
                cmmDebuger.Continue();
                debugThread.Resume();
            }
        }

        private void stepOver_Click(object sender, RoutedEventArgs e)
        {
            if (isDebug) {
                cmmDebuger.StepOver();
                debugThread.Resume();
            }
        }

        private void stepInto_Click(object sender, RoutedEventArgs e)
        {
            if (isDebug) {
                cmmDebuger.StepInto();
                debugThread.Resume();
            }
        }
    }
}