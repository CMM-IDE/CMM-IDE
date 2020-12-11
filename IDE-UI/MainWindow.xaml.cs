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
            this.DataContext = this;
            textBox = new TextBox();
            textBox.FontSize = 14;
            textBox.AcceptsReturn = true;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.TextChanged += TextChangedEventHandler;
            textBox.KeyUp += textBox_KeyUp;

            extraWindowPresenter.SizeChanged += handleSizeChanged;
        }

        private TextBox textBox;

        private bool isInputMode = false;

        private int inputLength = 0;

        public IDEState State {
            get {
                return state;
            }
            set {
                state = value;
            }
        }

        public IDEState state = new IDEState();

        private Thread runnerThread = null;

        private RefPhase visitor = null;

        private void loadSample_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Text = "int a = 10;\nwhile (a <> 0) {\n\ta = a - 1;\n\twrite(a);\n}";
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

            if(!state.ConsoleShowed) {
                windowButton_Click(btnConsole, null);
            }
            
            textBox.Text = "";
            String input = textEditor.Text;
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new CMMLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CMMParser parser = new CMMParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.statements();

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
                    //Print(e1.Message);
                }
                catch (Exception e2) {
                    Print(e2.Message);
                }
            });

            runnerThread.Start();
        }

        public void Print(string s)
        {
            Dispatcher.Invoke(() => {
                textBox.Text += s;
            });
            
        }

        private void handleSizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            state.FileModified = true;
            if(isInputMode) {
                foreach (var change in e.Changes) {
                    inputLength += change.AddedLength;
                    inputLength -= change.RemovedLength;
                }
            }
            
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (isInputMode && e.Key == Key.Enter) {
                var text = textBox.Text.Substring(textBox.Text.Length - inputLength - 1, inputLength - 1);
                isInputMode = false;
                inputLength = 0;

                visitor.buffer = text;
                runnerThread.Resume();
                e.Handled = false;
            }
        }

        private async void OpenFileItem_Click(object sender, RoutedEventArgs e)
        {
            try {
                var path = FileHelper.PickFileAsync();
                if (path != null) {
                    textEditor.Text = await FileHelper.ReadStringFromFileAsync(path);
                    state.FileOpened = true;
                    state.OpenedFilePath = path;
                    state.FileModified = false;
                }
                
            }
            catch {
                MessageBox.Show("打开文件出错，请重试。");
            }
            
        }

        private async void SaveFileItem_Click(object sender, RoutedEventArgs e)
        {

            var path = state.FileOpened ? state.OpenedFilePath : FileHelper.SaveFileAsync();
            bool succeed = await FileHelper.WriteFileAsync(path, textEditor.Text);
            if(succeed) {
                state.FileOpened = true;
                state.OpenedFilePath = path;
                state.FileModified = false;
            }
        }

        private void handleNeedInput()
        {
            Dispatcher.Invoke(() => {
                textBox.Select(textBox.Text.Length - 1, 0);
                isInputMode = true;
            });
            
        }

        private void windowButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var tag = btn.Tag as string;

            switch (tag) {
                case "console":
                    state.ConsoleShowed = !state.ConsoleShowed;
                    if(state.ConsoleShowed) {
                        state.DebugWindowShowed = false;
                        extraWindowPresenter.Content = textBox;
                    }
                    break;
                case "debug":
                    state.DebugWindowShowed = !state.DebugWindowShowed;
                    if (state.DebugWindowShowed) {
                        state.ConsoleShowed = false;
                        extraWindowPresenter.Content = null;
                    }
                    break;
            }

            if(state.DebugWindowShowed || state.ConsoleShowed) {
                splitterRow.Height = new GridLength(10);
                extraWindowRow.Height = new GridLength(Height * 0.3);
            }
            else {
                splitterRow.Height = new GridLength(0);
                extraWindowRow.Height = new GridLength(0);
            }
        }
    }
}
