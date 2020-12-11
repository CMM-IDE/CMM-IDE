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
            textBox.FontSize = 14;
        }

        private bool isInputMode = true;

        private int inputLength = 0;

        private IDEState state = new IDEState();

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
                Print("\nprogram exit\n");
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


        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            state.fileModified = true;
            if(isInputMode) {
                foreach (var change in e.Changes) {
                    inputLength += change.AddedLength;
                    inputLength -= change.RemovedLength;
                }
            }
            
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                var text = textBox.Text.Substring(textBox.Text.Length - inputLength, inputLength - 1);
                Debug.WriteLine(text);
                inputLength = 0;
                e.Handled = false;
            }
        }

        private async void OpenFileItem_Click(object sender, RoutedEventArgs e)
        {
            try {
                var path = FileHelper.PickFileAsync();
                if (path != null) {
                    textEditor.Text = await FileHelper.ReadStringFromFileAsync(path);
                    state.fileOpened = true;
                    state.openedFilePath = path;
                    state.fileModified = false;
                }
                
            }
            catch {
                MessageBox.Show("打开文件出错，请重试。");
            }
            
        }

        private async void SaveFileItem_Click(object sender, RoutedEventArgs e)
        {

            var path = state.fileOpened ? state.openedFilePath : FileHelper.SaveFileAsync();
            bool succeed = await FileHelper.WriteFileAsync(path, textEditor.Text);
            if(succeed) {
                state.fileOpened = true;
                state.openedFilePath = path;
                state.fileModified = false;
            }
        }
    }
}
