using System.ComponentModel;


namespace IDE_UI.Helper
{
    public class IDEState: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        

        public bool IsRunning {
            get {
                return isRunning;
            }
            set {
                isRunning = value;
            }
        }
        private bool isRunning = false;


        public bool FileOpened {
            get {
                return fileOpened;
            }
            set {
                fileOpened = value;
            }
        }
        private bool fileOpened = false;


        public string OpenedFilePath {
            get {
                return openedFilePath;
            }
            set {
                openedFilePath = value;
            }
        }
        private string openedFilePath = null;


        public bool FileModified {
            get {
                return fileModified;
            }
            set {
                fileModified = value;
            }
        }
        private bool fileModified = false;


        public bool ConsoleShowed {
            get {
                return consoleShowed;
            }
            set {
                consoleShowed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConsoleShowed"));
            }
        }
        private bool consoleShowed = false;


        public bool DebugWindowShowed {
            get {
                return debugWindowShowed;
            }
            set {
                debugWindowShowed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DebugWindowShowed"));
            }
        }
        private bool debugWindowShowed = false;

        public bool ErrorWindowShowed {
            get {
                return errorWindowShowed;
            }
            set {
                errorWindowShowed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ErrorWindowShowed"));
            }
        }
        private bool errorWindowShowed = false;

        public bool TreeWindowShowed {
            get {
                return treeWindowShowed;
            }
            set {
                treeWindowShowed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TreeWindowShowed"));
            }
        }
        private bool treeWindowShowed = false;

    }
}
