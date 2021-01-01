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

namespace IDE_UI
{
    public partial class MainWindow
    {

        private void loadSample_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
            textEditor.Text = "int a = 10;\nwhile (a <> 0) {\n\ta = a - 1;\n\twrite(a);\n}";
            textEditor.CurrentDebugLine = 2;
        }

        /// <summary>
        /// 代码编辑器中代码改变的事件
        /// </summary>
        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            State.FileModified = true;
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
                visitor.buffer = text;
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

        private void handleNeedInput()
        {
            Dispatcher.Invoke(() => {
                consoleTextBox.IsReadOnly = false;
                consoleTextBox.Select(consoleTextBox.Text.Length - 1, 0);
                isInputMode = true;
            });
        }

        private GridLength rememberedHeight = new GridLength(0);

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
                        extraWindowPresenter.Content = consoleTextBox;
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
            if (MessageBox.Show("当前文件未保存，是否需要保存？", "新建文件", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                SaveFileItem_Click(null, null);
            }
            textEditor.Text = "";
            consoleTextBox.Text = "";
        }
    }
}
