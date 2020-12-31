using AduSkin.Controls.Metro;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CMMInterpreter.CMMException;
using CMMInterpreter.inter;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Input;
using IDE_UI.Helper;
using System.Threading;
using CMMInterpreter.vm;
using CMMInterpreter.debuger;
using System.Collections.Generic;

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

        private TextBox consoleTextBox;

        private bool isInputMode = false;

        private int inputLength = 0;

        private CMMDebuger cmmDebuger;

        private Thread debugThread;

        private bool isDebug = false;

        public IDEState State {
            get {
                return state;
            }
            set {
                state = value;
            }
        }

        private IDEState state = new IDEState();

        private Thread runnerThread = null;

        private RefPhase visitor = null;



        private void run_Click(object sender, RoutedEventArgs e)
        {

            if(!state.ConsoleShowed) {
                extraWindowButton_Click(btnConsoleWindow, null);
            }
            
            consoleTextBox.Text = "";
            String input = textEditor.Text;
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new CMMLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CMMParser parser = new CMMParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.statements();
            var visitor = new CompileVisitor();
            try
            {
                visitor.Visit(tree);
                for(int i = 0; i < visitor.codes.Count; i++)
                {
                    Print(i + ":" + visitor.codes[i].toString());
                }

            }
            catch(VariableNotFountException exp)
            {
                Print(exp.ToString());
            }


            VirtualMachine vm = new VirtualMachine();
            vm.register(this);
            vm.interpret(visitor.codes);
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
        }

        public void write(Object o) {
            Print(o.ToString());
        }

        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            if (!state.ConsoleShowed)
            {
                extraWindowButton_Click(btnConsoleWindow, null);
            }

            consoleTextBox.Text = "";
            String input = textEditor.Text;
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new CMMLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CMMParser parser = new CMMParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.statements();
            var visitor = new CompileVisitor();
            try
            {
                visitor.Visit(tree);
                //for (int i = 0; i < visitor.codes.Count; i++)
                //{
                //    Print(i + ":" + visitor.codes[i].toString());
                //}

            }
            catch (VariableNotFountException exp)
            {
                Print(exp.ToString());
            }

            // 断点列表
            List<int> breakpoints = new List<int>();
            breakpoints.Add(11);

            // 初始化调试器
            cmmDebuger = new CMMDebuger(visitor.codes, breakpoints);
            cmmDebuger.setListener(this);
            cmmDebuger.OutputStream = this;
            cmmDebuger.NeedDebug += HandlerDebug;

            debugThread= new Thread(() => {
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
                consoleTextBox.Text += "\nCurrentLine: "+cmmDebuger.GetCurrentLine().ToString() + "\n";
                List<FrameInformation> informations = cmmDebuger.GetCurrentFrame();

                // 获取当前栈帧
                consoleTextBox.Text += "\n---Current Frame Stack---\nAddress\tName\tValue\n";
                foreach (FrameInformation information in informations)
                {
                    consoleTextBox.Text += information.Address+"\t"+information.Name+"\t"+information.Value+"\n";
                }
            });
        }

        private void btnStepInto_Click(object sender, RoutedEventArgs e)
        {
            if (isDebug)
            {
                cmmDebuger.StepInto();
                debugThread.Resume();
            }
        }

        private void btnStepOver_Click(object sender, RoutedEventArgs e)
        {
            if (isDebug)
            {
                cmmDebuger.StepOver();
                debugThread.Resume();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (isDebug)
            {
                cmmDebuger.Stop();
                debugThread.Resume();
                isDebug = false;
            }
        }
    }
}
