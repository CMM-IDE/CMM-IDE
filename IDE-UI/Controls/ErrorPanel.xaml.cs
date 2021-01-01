﻿using CMMInterpreter.CMMException;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Errors"));
            }
        }
        private List<ErrorInfo> errors;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
