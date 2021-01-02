using System.Windows.Input;

namespace IDE_UI
{
    public partial class MainWindow
    {
        public static RoutedCommand NewFileCommand = new RoutedCommand();
        public static RoutedCommand OpenFileCommand = new RoutedCommand();
        public static RoutedCommand SaveFileCommand = new RoutedCommand();
        public static RoutedCommand SelectAllCommand = new RoutedCommand();
        public static RoutedCommand PrintFileCommand = new RoutedCommand();
        public static RoutedCommand UndoCommand = new RoutedCommand();
        public static RoutedCommand RedoCommand = new RoutedCommand();
        public static RoutedCommand CutCommand = new RoutedCommand();
        public static RoutedCommand CopyCommand = new RoutedCommand();
        public static RoutedCommand PasteCommand = new RoutedCommand();
        public static RoutedCommand runCommand = new RoutedCommand();
        public static RoutedCommand debugCommand = new RoutedCommand();
        public static RoutedCommand stopCommand = new RoutedCommand();

        public static RoutedCommand continueCommand = new RoutedCommand();
        public static RoutedCommand stepOverCommand = new RoutedCommand();
        public static RoutedCommand stepIntoCommand = new RoutedCommand();

    }
}
