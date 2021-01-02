using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;
using CMMInterpreter.debuger;
using System.ComponentModel;

namespace IDE_UI.Controls
{

    public partial class DebugPanel : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<DebugOperation> requireDebugAction;

        public bool InDebugMode {
            get => inDebugMode;
            set {
                this.inDebugMode = value;
                if(!value) {
                    consolePresenter.Content = null;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InDebugMode"));
            }
        }
        private bool inDebugMode = false;

        private int preIndex = 0;

        public int SelectedIndex {
            get => selectedIndex;
            set {
                preIndex = selectedIndex;
                selectedIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedIndex"));
            }
        }
        private int selectedIndex = 0;


        /// <summary>
        /// 一群栈帧
        /// </summary>
        public List<StackFrameInformation> StackFrameSymbols {
            get {
                return stackFrameSymbols;
            }
            set {
                stackFrameSymbols = value;
                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StackFrameSymbols"));
                if (preIndex < value.Count - 1) {
                    SelectedIndex = preIndex;
                }
                else {
                    SelectedIndex = value.Count - 1;
                }
            }
        }
        private List<StackFrameInformation> stackFrameSymbols;

        /// <summary>
        /// 当前栈帧
        /// </summary>
        public StackFrameInformation CurrentFrame {
            get {
                return currentFrame;
            }
            set {
                currentFrame = value;
                if(value == null) {
                    currentFrameVariables = null;
                    return;
                }
                CurrentFrameVariables = value.Frame;
            }
        }
        private StackFrameInformation currentFrame;

        /// <summary>
        /// 当前栈帧的变量
        /// </summary>
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
        }

    }

    public enum DebugOperation
    {
        Continue, StepOver, StepInto, StepOut
    }
}
