using CMMInterpreter.CMMException;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;


namespace IDE_UI.Controls
{
    public partial class ErrorPanel : UserControl, INotifyPropertyChanged
    {
        public ErrorPanel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public List<ErrorInfo> Errors {
            get {
                return errors;
            }
            set {
                errors = value;
                if(value != null) {

                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Errors"));
            }
        }
        private List<ErrorInfo> errors;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
