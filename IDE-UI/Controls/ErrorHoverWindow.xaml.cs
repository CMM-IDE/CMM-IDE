
using CMMInterpreter.CMMException;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IDE_UI.Controls
{
    /// <summary>
    /// ErrorHoverWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorHoverWindow : Window, INotifyPropertyChanged
    {
        public ErrorHoverWindow()
        {
            InitializeComponent();
        }

        public ErrorInfo ErrorInfo {
            get => ErrorInfo;
            set {
                errorInfo = value;
                lineNumBlock.Text = value.Line.ToString();
                msgBlock.Text = value.Message;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ErrorInfo"));
            }
        }

        private ErrorInfo errorInfo;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
