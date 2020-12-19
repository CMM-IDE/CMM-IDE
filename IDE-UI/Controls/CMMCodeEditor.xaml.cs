using ScintillaNET;
using ScintillaNET.WPF;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace IDE_UI.Controls
{
    /// <summary>
    /// CMMCodeEditor.xaml 的交互逻辑
    /// </summary>
    public partial class CMMCodeEditor : UserControl
    {
        #region Fields

        private int _zoomLevel = 0;

        private const int LINE_NUMBERS_MARGIN_WIDTH = 30; // TODO - don't hardcode this

        /// <summary>
        /// the background color of the text area
        /// </summary>
        private const int BACK_COLOR = 0xCCCCCC;

        /// <summary>
        /// default text color of the text area
        /// </summary>
        private const int FORE_COLOR = 0x404040;

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;

        private const int BOOKMARK_MARKER = 2;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = false;


        public static RoutedCommand NewFileCommand = new RoutedCommand();
        public static RoutedCommand OpenFileCommand = new RoutedCommand();
        public static RoutedCommand SaveFileCommand = new RoutedCommand();
        public static RoutedCommand SaveAllFilesCommand = new RoutedCommand();
        public static RoutedCommand PrintFileCommand = new RoutedCommand();
        public static RoutedCommand UndoCommand = new RoutedCommand();
        public static RoutedCommand RedoCommand = new RoutedCommand();
        public static RoutedCommand CutCommand = new RoutedCommand();
        public static RoutedCommand CopyCommand = new RoutedCommand();
        public static RoutedCommand PasteCommand = new RoutedCommand();
        public static RoutedCommand SelectAllCommand = new RoutedCommand();
        public static RoutedCommand IncrementalSearchCommand = new RoutedCommand();
        public static RoutedCommand FindCommand = new RoutedCommand();
        public static RoutedCommand ReplaceCommand = new RoutedCommand();
        public static RoutedCommand GotoCommand = new RoutedCommand();


        #endregion Fields

        public CMMCodeEditor()
        {
            InitializeComponent();
            SetScintillaToCurrentOptions();
        }

        public string Text {
            get {
                return textEditor.Text;
            }
            set {
                textEditor.Text = value;
            }
        }

        #region Bookmarks

        private void toggleBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Line currentLine = textEditor.Lines[textEditor.CurrentLine];
            const uint mask = (1 << BOOKMARK_MARKER);
            uint markers = currentLine.MarkerGet();
            if ((markers & mask) > 0) {
                currentLine.MarkerDelete(BOOKMARK_MARKER);
            }
            else {
                currentLine.MarkerAdd(BOOKMARK_MARKER);
            }
        }

        private void previousBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int lineNumber = textEditor.Lines[textEditor.CurrentLine - 1].MarkerPrevious(1 << BOOKMARK_MARKER);
            if (lineNumber != -1)
                textEditor.Lines[lineNumber].Goto();
        }

        private void nextBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int lineNumber = textEditor.Lines[textEditor.CurrentLine + 1].MarkerNext(1 << BOOKMARK_MARKER);
            if (lineNumber != -1)
                textEditor.Lines[lineNumber].Goto();
        }

        private void clearBookmarksMenuItem_Click(object sender, RoutedEventArgs e)
        {
            textEditor.MarkerDeleteAll(BOOKMARK_MARKER);
        }

        #endregion Bookmarks

        private void InitBookmarkMargin(ScintillaWPF ScintillaNet)
        {
            //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = ScintillaNet.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = ScintillaNet.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(IntToColor(0xFF003B));
            marker.SetForeColor(IntToColor(0x000000));
            marker.SetAlpha(100);
        }

        private void InitCodeFolding(ScintillaWPF ScintillaNet)
        {
            ScintillaNet.SetFoldMarginColor(true, IntToMediaColor(BACK_COLOR));
            ScintillaNet.SetFoldMarginHighlightColor(true, IntToMediaColor(BACK_COLOR));

            // Enable code folding
            ScintillaNet.SetProperty("fold", "1");
            ScintillaNet.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            ScintillaNet.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            ScintillaNet.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            ScintillaNet.Margins[FOLDING_MARGIN].Sensitive = true;
            ScintillaNet.Margins[FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++) {
                ScintillaNet.Markers[i].SetForeColor(IntToColor(BACK_COLOR)); // styles for [+] and [-]
                ScintillaNet.Markers[i].SetBackColor(IntToColor(FORE_COLOR)); // styles for [+] and [-]
            }

            // Configure folding markers with respective symbols
            ScintillaNet.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            ScintillaNet.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            ScintillaNet.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            ScintillaNet.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            ScintillaNet.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            ScintillaNet.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            ScintillaNet.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            ScintillaNet.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        private void InitColors(ScintillaWPF ScintillaNet)
        {
            ScintillaNet.CaretForeColor = Colors.Black;
            ScintillaNet.SetSelectionBackColor(true, IntToMediaColor(0x6070CC));

            //FindReplace.Indicator.ForeColor = System.Drawing.Color.DarkOrange;
        }

        private void InitNumberMargin(ScintillaWPF ScintillaNet)
        {
            ScintillaNet.Styles[ScintillaNET.Style.LineNumber].BackColor = IntToColor(0xCCCCCC);
            ScintillaNet.Styles[ScintillaNET.Style.LineNumber].ForeColor = IntToColor(0x404040);
            ScintillaNet.Styles[ScintillaNET.Style.IndentGuide].ForeColor = IntToColor(0x404040);
            ScintillaNet.Styles[ScintillaNET.Style.IndentGuide].BackColor = IntToColor(0xCCCCCC);

            var nums = ScintillaNet.Margins[NUMBER_MARGIN];
            nums.Width = LINE_NUMBERS_MARGIN_WIDTH;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            ScintillaNet.MarginClick += TextArea_MarginClick;
        }

        private void InitSyntaxColoring(ScintillaWPF ScintillaNet)
        {
            // Configure the default style
            ScintillaNet.StyleResetDefault();
            ScintillaNet.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            ScintillaNet.Styles[ScintillaNET.Style.Default].Size = 13;
            ScintillaNet.Styles[ScintillaNET.Style.Default].BackColor = IntToColor(0xFFFFFF);
            ScintillaNet.Styles[ScintillaNET.Style.Default].ForeColor = IntToColor(0x202020);
            ScintillaNet.StyleClearAll();

            ScintillaNet.Styles[ScintillaNET.Style.BraceLight].BackColor = System.Drawing.Color.LightGray;
            ScintillaNet.Styles[ScintillaNET.Style.BraceLight].ForeColor = System.Drawing.Color.BlueViolet;
            ScintillaNet.Styles[ScintillaNET.Style.BraceBad].ForeColor = System.Drawing.Color.Red;

            // Configure the CPP (C#) lexer styles
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Identifier].ForeColor = IntToColor(0x202122);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Comment].ForeColor = IntToColor(0xBD758B);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Number].ForeColor = IntToColor(0x0F20F6);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.String].ForeColor = IntToColor(0xED7722);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Operator].ForeColor = IntToColor(0x000000);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Word].ForeColor = IntToColor(0xAA2063);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Word2].ForeColor = IntToColor(0xAA2063);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.GlobalClass].ForeColor = IntToColor(0xCC4C07);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Default].ForeColor = IntToColor(0x202122);

            ScintillaNet.Lexer = Lexer.Cpp;

            ScintillaNet.SetKeywords(0, "class extends implements import interface new case do while else if for in switch throw get set function var try catch finally while with default break continue delete return each const namespace package include use is as instanceof typeof author copy default deprecated eventType example exampleText exception haxe inheritDoc internal link mtasc mxmlc param private return see serial serialData serialField since throws usage version langversion playerversion productversion dynamic private public partial static intrinsic internal native override protected AS3 final super this arguments null Infinity NaN undefined true false abstract as base bool break by byte case catch char checked class const continue decimal default delegate do double descending explicit event extern else enum false finally fixed float for foreach from goto group if implicit in int interface internal into is lock long new null namespace object operator out override orderby params private protected public readonly ref return switch struct sbyte sealed short sizeof stackalloc static string select this throw true try typeof uint ulong unchecked unsafe ushort using var virtual volatile void while where yield");
            ScintillaNet.SetKeywords(1, "void Null ArgumentError arguments Array Boolean Class Date DefinitionError Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path File System Windows Forms ScintillaNET");
        }

        /// <summary>
        /// Converts a Win32 colour to a Drawing.Color
        /// </summary>
        public static System.Drawing.Color IntToColor(int rgb)
        {
            return System.Drawing.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        /// <summary>
        /// Converts a Win32 colour to a Media Color
        /// </summary>
        public static Color IntToMediaColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        private void MyFindReplace_KeyPressed(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            ScintillaNet_KeyDown(sender, e);
        }



        private void SetScintillaToCurrentOptions()
        {
            ScintillaWPF ScintillaNet = textEditor;
            ScintillaNet.KeyDown += ScintillaNet_KeyDown;

            // INITIAL VIEW CONFIG
            ScintillaNet.WrapMode = WrapMode.None;
            ScintillaNet.IndentationGuides = IndentView.LookBoth;

            // STYLING
            InitColors(ScintillaNet);
            InitSyntaxColoring(ScintillaNet);

            // NUMBER MARGIN
            InitNumberMargin(ScintillaNet);

            // BOOKMARK MARGIN
            InitBookmarkMargin(ScintillaNet);

            // CODE FOLDING MARGIN
            InitCodeFolding(ScintillaNet);

            // DRAG DROP
            // TODO - Enable InitDragDropFile
            //InitDragDropFile();

            // INIT HOTKEYS
            // TODO - Enable InitHotkeys
            //InitHotkeys(ScintillaNet);

            // Turn on line numbers?


            // Show EOL?
            ScintillaNet.ViewEol = false;

            // Set the zoom
            ScintillaNet.Zoom = _zoomLevel;
        }

        private void ScintillaNet_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

        }

        private void TextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            ScintillaNET.WPF.ScintillaWPF TextArea = textEditor;

            if (e.Margin == BOOKMARK_MARGIN) {
                // Do we have a marker for this line?
                const uint mask = (1 << BOOKMARK_MARKER);
                var line = TextArea.Lines[TextArea.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0) {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                }
                else {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        //在此处执行自动补全
        private void textEditor_CharAdded(object sender, CharAddedEventArgs e)
        {
            //InsertMatchedChars(e);
            //return;
            int oldPos = textEditor.SelectionStart;
            
            switch (e.Char) {
                case '{':
                    textEditor.Text = textEditor.Text.Insert(textEditor.SelectionStart, "}");
                    textEditor.SelectionStart = oldPos;
                    break;
                case '(':
                    textEditor.Text = textEditor.Text.Insert(textEditor.SelectionStart, ")");
                    textEditor.SelectionStart = oldPos;
                    break;
                case '[':
                    textEditor.Text = textEditor.Text.Insert(textEditor.SelectionStart, "]");
                    textEditor.SelectionStart = oldPos;
                    break;
                case '\n':
                    if(textEditor.Text[textEditor.SelectionStart - 3] == '{') {
                        textEditor.Text = textEditor.Text.Insert(textEditor.SelectionStart - 2, "\r\n    ");
                        textEditor.SelectionStart = oldPos + 4;
                    }
                    
                    break;
                default:
                    performAutoComplete();
                    break;
            }
            Debug.WriteLine(textEditor.SelectionStart);
        }

        private void performAutoComplete()
        {
            var currentPos = textEditor.CurrentPosition;
            var wordStartPos = textEditor.WordStartPosition(currentPos, true);

            var lenEntered = currentPos - wordStartPos;
            if (lenEntered > 0) {
                if (!textEditor.AutoCActive)
                    textEditor.AutoCShow(lenEntered, "break " + "bool int string write read" +
                        " continue do else void" +
                        " false for if" +
                        " return" +
                        " true" +
                        " while");
            }
        }

        private static bool IsBrace(int c)
        {
            switch (c) {
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                case '<':
                case '>':
                    return true;
            }

            return false;
        }

        int lastCaretPos = 0;

        private void textEditor_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            
            var caretPos = textEditor.CurrentPosition;
            if (lastCaretPos != caretPos) {
                lastCaretPos = caretPos;
                var bracePos1 = -1;
                var bracePos2 = -1;

                
                if (caretPos > 0 && IsBrace(textEditor.GetCharAt(caretPos - 1)))
                    bracePos1 = (caretPos - 1);
                else if (IsBrace(textEditor.GetCharAt(caretPos)))
                    bracePos1 = caretPos;

                if (bracePos1 >= 0) {
                    
                    bracePos2 = textEditor.BraceMatch(bracePos1);
                    if (bracePos2 == Scintilla.InvalidPosition) {
                        textEditor.BraceBadLight(bracePos1);
                        textEditor.HighlightGuide = 0;
                    }
                    else {
                        textEditor.BraceHighlight(bracePos1, bracePos2);
                        textEditor.HighlightGuide = textEditor.GetColumn(bracePos1);
                    }
                }
                else {
                    
                    textEditor.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                    textEditor.HighlightGuide = 0;
                }
            }
        }

        private void InsertMatchedChars(CharAddedEventArgs e)
        {
            var caretPos = textEditor.CurrentPosition;
            var docStart = caretPos == 1;
            var docEnd = caretPos == textEditor.Text.Length;

            var charPrev = docStart ? textEditor.GetCharAt(caretPos) : textEditor.GetCharAt(caretPos - 2);
            var charNext = textEditor.GetCharAt(caretPos);

            var isCharPrevBlank = charPrev == ' ' || charPrev == '\t' ||
                                  charPrev == '\n' || charPrev == '\r';

            var isCharNextBlank = charNext == ' ' || charNext == '\t' ||
                                  charNext == '\n' || charNext == '\r' ||
                                  docEnd;

            var isEnclosed = (charPrev == '(' && charNext == ')') ||
                                  (charPrev == '{' && charNext == '}') ||
                                  (charPrev == '[' && charNext == ']');

            var isSpaceEnclosed = (charPrev == '(' && isCharNextBlank) || (isCharPrevBlank && charNext == ')') ||
                                  (charPrev == '{' && isCharNextBlank) || (isCharPrevBlank && charNext == '}') ||
                                  (charPrev == '[' && isCharNextBlank) || (isCharPrevBlank && charNext == ']');

            var isCharOrString = (isCharPrevBlank && isCharNextBlank) || isEnclosed || isSpaceEnclosed;

            var charNextIsCharOrString = charNext == '"' || charNext == '\'';

            switch (e.Char) {
                case '(':
                    if (charNextIsCharOrString)
                        return;
                    textEditor.InsertText(caretPos, ")");
                    break;
                case '{':
                    if (charNextIsCharOrString)
                        return;
                    textEditor.InsertText(caretPos, "}");
                    break;
                case '[':
                    if (charNextIsCharOrString)
                        return;
                    textEditor.InsertText(caretPos, "]");
                    break;
                case '"':
                    // 0x22 = "
                    if (charPrev == 0x22 && charNext == 0x22) {
                        textEditor.DeleteRange(caretPos, 1);
                        textEditor.GotoPosition(caretPos);
                        return;
                    }

                    if (isCharOrString)
                        textEditor.InsertText(caretPos, "\"");
                    break;
                case '\'':
                    // 0x27 = '
                    if (charPrev == 0x27 && charNext == 0x27) {
                        textEditor.DeleteRange(caretPos, 1);
                        textEditor.GotoPosition(caretPos);
                        return;
                    }

                    if (isCharOrString)
                        textEditor.InsertText(caretPos, "'");
                    break;
            }
        }
    }
}
