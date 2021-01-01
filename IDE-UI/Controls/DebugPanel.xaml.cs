using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;
using CMMInterpreter.debuger;
using IDE_UI.Model;
using System.ComponentModel;

namespace IDE_UI.Controls
{
    /// <summary>
    /// DebugPanel.xaml 的交互逻辑
    /// </summary>
    public partial class DebugPanel : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<DebugOperation> requireDebugAction;

        public bool InDebugMode { get; set; } = true;


        public ObservableCollection<StackFrameSymbol> StackFrameSymbols {
            get {
                return stackFrameSymbols;
            }
            set {
                stackFrameSymbols = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StackFrameSymbols"));
            }
        }
        private ObservableCollection<StackFrameSymbol> stackFrameSymbols;

        public StackFrameSymbol CurrentFrame {
            get {
                return currentFrame;
            }
            set {
                currentFrame = value;
                CurrentFrameVariables = value.CurrentFrame;
            }
        }
        private StackFrameSymbol currentFrame;

        public List<FrameInformation> CurrentFrameVariables {
            get => currentFrameVariables;
            set {
                currentFrameVariables = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentFrameVariables"));
            }
        }

        private List<FrameInformation> currentFrameVariables = null;


        public DebugPanel()
        {
            InitializeComponent();
            this.DataContext = this;

            stackFrameSymbols = new ObservableCollection<StackFrameSymbol>();

            List<FrameInformation> CurrentFrame = new List<FrameInformation> {
                new FrameInformation {
                    Name="aaa",
                    Value="bbb",
                    Address=11
                },
            };
            List<FrameInformation> CurrentFrame1 = new List<FrameInformation> {
                new FrameInformation {
                    Name="xxxxxxxx",
                    Value="yyyyyyy",
                    Address=12
                },
            };


            stackFrameSymbols.Add(new StackFrameSymbol("test", 5, CurrentFrame));
            stackFrameSymbols.Add(new StackFrameSymbol("main", 1, CurrentFrame1));

        }
    }

    public enum DebugOperation
    {
        Continue, StepOver, StepInto, StepOut
    }
}
