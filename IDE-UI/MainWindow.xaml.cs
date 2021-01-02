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

            init();
        }

        #region 属性
        VirtualMachine vm;

        /// <summary>
        /// 中间代码面板
        /// </summary>
        private CodeWindow codeWindow = new CodeWindow();

        /// <summary>
        /// 暂时存放中间代码
        /// </summary>
        private string intermediateCode;

        /// <summary>
        /// 虚拟控制台
        /// </summary>
        private TextBox consoleTextBox;

        /// <summary>
        /// 仅仅用于提示信息的一个面板。其实完全没必要，但比较赶，所以用了丑陋的做法
        /// </summary>
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

        /// <summary>
        /// 调试器
        /// </summary>
        private CMMDebuger cmmDebuger;

        /// <summary>
        /// 调试代码的线程
        /// </summary>
        private Thread debugThread;

        /// <summary>
        /// 是否处于调试模式
        /// </summary>
        private bool isDebug = false;

        /// <summary>
        /// 运行代码的线程
        /// </summary>
        private Thread runnerThread = null;

        /// <summary>
        /// 用于在空闲一段时间后，执行某些操作的定时器类
        /// </summary>
        private IdleExecutor idleExec = new IdleExecutor(2);

        #endregion



        /// <summary>
        /// 运行代码
        /// </summary>
        private void run_Click(object sender, RoutedEventArgs e)
        {
            IParseTree tree = prepareForCodeGen();
            CompileVisitor visitor = prepareForRunning(tree);

            if(visitor == null) {
                return;
            }
            
            VirtualMachine vm = new VirtualMachine();
            this.vm = vm;
            vm.register(this);
            vm.needInput += handleNeedInput;

            runnerThread = new Thread(() =>
            {
                vm.interpret(visitor.codes);
            });
            runnerThread.Start();
        }


        /// <summary>
        /// 调试代码
        /// </summary>
        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {

            IParseTree tree = prepareForCodeGen();
            CompileVisitor visitor = prepareForRunning(tree);

            if (visitor == null) {
                return;
            }

            prepareForDebug();

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
        /// 准备生成代码
        /// </summary>
        private IParseTree prepareForCodeGen()
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

            parser.RemoveErrorListeners();

            parser.AddErrorListener(listener);
            parser.ErrorHandler = new CMMErrorStrategy();

            parser.BuildParseTree = true;
            IParseTree tree = parser.statements();

            //如果出错，转到错误面板并返回null
            if (listener.errors.Count != 0) {

                handleCompileTimeError(listener.errors);
                return null;
            }
            var graph = new ParseTreeGrapher().CreateGraph(tree, CMMParser.ruleNames);
            drawTreePanel.Graph = graph;

            return tree;
        }

        /// <summary>
        /// 准备运行代码
        /// </summary>
        private CompileVisitor prepareForRunning(IParseTree tree)
        {
            if (tree == null) {
                return null;
            }

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
                handleCompileTimeError(new List<ErrorInfo> { exp.Error });
                intermediateCode = null;
                return null;
            }
            catch (VariableRedefinedException exp) {
                handleCompileTimeError(new List<ErrorInfo> { exp.Error });
                intermediateCode = null;
                return null;
            }

            if (!State.ConsoleShowed) {
                extraPanelButton_Click(btnConsoleWindow, null);
            }

            consoleTextBox.Text = "";

            return visitor;
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
        /// 处理运行前错误
        /// </summary>
        private void handleCompileTimeError(List<ErrorInfo> errors)
        {
            if (!State.ErrorWindowShowed) {
                extraPanelButton_Click(btnErrorWindow, null);
            }
            errorPanel.Errors = errors;
            textEditor.Errors = errors;
        }

        /// <summary>
        /// 处理调试
        /// </summary>
        private void HandlerDebug()
        {
            Dispatcher.Invoke(() => {
                
                List<FrameInformation> informations = cmmDebuger.GetCurrentFrame();
                List<StackFrameInformation> stackFrames = cmmDebuger.GetStackFrames();

                debugPanel.InDebugMode = true;
                debugPanel.StackFrameSymbols = stackFrames;
                textEditor.CurrentDebugLine = cmmDebuger.GetCurrentLine() - 1;
                //这里本来被注释掉的大段东西放在文件末尾
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
            Print(o.ToString().Equals("\n") ? "\n" : (o.ToString() + " "));
        }
    }
}


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