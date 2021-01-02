using IDE_UI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using IDE_UI.Controls;
using ScintillaNET;
using Antlr4.Runtime;
using CMMInterpreter.CMMException;
using Antlr4.Runtime.Tree;

namespace IDE_UI
{
    public partial class MainWindow: ICMMCodeEditorDelegate
    {

        private void loadSample_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            textEditor.Text = "int a = 10;\nwhile (a <> 0) {\n\ta = a - 1;\n\twrite(a);\n}";
            if(isDebug) {
                btnStop_Click(null, null);
            }
            charAdded(null, null);
        }

        /// <summary>
        /// 虚拟终端文本改变
        /// </summary>
        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (isInputMode) {
                foreach (var change in e.Changes) {
                    inputLength += change.AddedLength;
                    inputLength -= change.RemovedLength;
                }
            }
        }

        private void consoleTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (isInputMode && e.Key == Key.Enter) {
                var text = consoleTextBox.Text.Substring(consoleTextBox.Text.Length - inputLength - 1, inputLength - 1);
                isInputMode = false;
                inputLength = 0;
                vm.buffer = text;
                runnerThread.Resume();
                consoleTextBox.IsReadOnly = true;
            }
        }

        private void ConsoleTextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void ConsoleTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("ConsoleTextBox_KeyDown");
            if (isInputMode) {
                if (e.Key == Key.Back) {
                    if (inputLength <= 0) {
                        e.Handled = true;
                    }
                }
            }
        }

        private async void OpenFileItem_Click(object sender, RoutedEventArgs e)
        {
            try {
                var path = FileHelper.PickFileAsync();
                if (path != null) {
                    textEditor.Text = await FileHelper.ReadStringFromFileAsync(path);
                    State.FileOpened = true;
                    State.OpenedFilePath = path;
                    State.FileModified = false;
                }
                if (isDebug) {
                    btnStop_Click(null, null);
                }
                charAdded(null, null);
            }
            catch {
                MessageBox.Show("打开文件出错，请重试。");
            }

        }

        private async void SaveFileItem_Click(object sender, RoutedEventArgs e)
        {
            var path = State.FileOpened ? State.OpenedFilePath : FileHelper.SaveFileAsync();
            bool succeed = await FileHelper.WriteFileAsync(path, textEditor.Text);
            if (succeed) {
                State.FileOpened = true;
                State.OpenedFilePath = path;
                State.FileModified = false;
            }
        }


        private void codeItem_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(intermediateCode)) {
                run_Click(null, null);
            }
            if (String.IsNullOrEmpty(intermediateCode)) {
                return;
            }
            codeWindow = new CodeWindow();
            codeWindow.Text = intermediateCode;
            codeWindow.Show();
        }

        private void handleNeedInput()
        {
            Dispatcher.Invoke(() => {
                consoleTextBox.IsReadOnly = false;
                consoleTextBox.Select(consoleTextBox.Text.Length - 1, 0);
                isInputMode = true;
            });
        }

        private GridLength rememberedHeight = new GridLength(0);

        /// <summary>
        /// 很丑陋，如果有更多面板就要用数组来管理了。
        /// </summary>
        private void extraPanelButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var tag = btn.Tag as string;

            if (extraWindowRow.Height.Value != 0) {
                rememberedHeight = extraWindowRow.Height;
            }

            switch (tag) {
                case "console":
                    State.ConsoleShowed = !State.ConsoleShowed;
                    if (State.ConsoleShowed) {
                        State.DebugWindowShowed = false;
                        State.TreeWindowShowed = false;
                        State.ErrorWindowShowed = false;

                        extraWindowPresenter.Content = isDebug ? (object)hintControl : consoleTextBox;

                    }
                    break;
                case "debug":
                    State.DebugWindowShowed = !State.DebugWindowShowed;
                    if (State.DebugWindowShowed) {
                        State.ConsoleShowed = false;
                        State.ErrorWindowShowed = false;
                        State.TreeWindowShowed = false;
                        extraWindowPresenter.Content = debugPanel;
                    }
                    break;
                case "error":
                    State.ErrorWindowShowed = !State.ErrorWindowShowed;
                    if (State.ErrorWindowShowed) {
                        State.ConsoleShowed = false;
                        State.TreeWindowShowed = false;
                        State.DebugWindowShowed = false;
                        extraWindowPresenter.Content = errorPanel;
                    }
                    break;
                case "tree":
                    State.TreeWindowShowed = !State.TreeWindowShowed;
                    if (State.TreeWindowShowed) {
                        State.ConsoleShowed = false;
                        State.DebugWindowShowed = false;
                        State.ErrorWindowShowed = false;
                        drawTreePanel.setNeedUpdate();
                        extraWindowPresenter.Content = drawTreePanel;
                    }
                    break;
            }

            if (State.DebugWindowShowed || State.ConsoleShowed || State.TreeWindowShowed || State.ErrorWindowShowed) {
                splitterRow.Height = new GridLength(10);
                extraWindowRow.Height = rememberedHeight.Value == 0 ? new GridLength(Height * 0.3) : rememberedHeight;
            }
            else {
                splitterRow.Height = new GridLength(0);
                extraWindowRow.Height = new GridLength(0);
            }
        }

        private void NewFileItem_Click(object sender, RoutedEventArgs e)
        {
            if(State.FileOpened || State.FileModified) {
                if (MessageBox.Show("当前文件未保存，是否需要保存？", "新建文件", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                    SaveFileItem_Click(null, null);
                }
            }
            if (isDebug) {
                btnStop_Click(null, null);
            }
            charAdded(null, null);
            textEditor.Text = "";
            
        }

        private void init()
        {
            AllowsTransparency = false;
            this.DataContext = this;

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
            debugPanel.requireDebugAction += handleRequireDebugAction;

            drawTreePanel = new DrawTreePanel();

            errorPanel = new ErrorPanel();

            textEditor.editorDelegate = this;

            idleExec.timeOutAction += IdleExec_timeOutAction;
        }

        private void IdleExec_timeOutAction()
        {
            performCheck();
        }

        public void breakPointChanged(CMMCodeEditor sender, List<int> points)
        {
        }

        public void didAddOrRemoveBreakPoint(CMMCodeEditor sender, bool addOrRemove, int breakPoint)
        {
            if (!isDebug || cmmDebuger == null) {
                return;
            }
            if(addOrRemove) {
                cmmDebuger.AddBreakpoint(breakPoint);
            }
            else {
                cmmDebuger.RemoveBreakpoint(breakPoint);
            }
        }

        public void charAdded(CMMCodeEditor sender, CharAddedEventArgs e)
        {
            State.FileModified = true;
            idleExec.MarkActive();
        }


        private void performCheck()
        {
            Debug.WriteLine("perform check");
            
            Dispatcher.Invoke(() => {
                string input = textEditor.Text;
                if(string.IsNullOrEmpty(input)) {
                    return;
                }
                errorPanel.Errors = null;
                textEditor.Errors = null;
                Task.Run(() => {

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

                    if (listener.errors.Count != 0) {

                        Dispatcher.Invoke(() => {
                            errorPanel.Errors = listener.errors;
                            textEditor.Errors = listener.errors;
                        });

                    }
                });
            });
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
            if (isDebug) {
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
