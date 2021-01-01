using ScintillaNET;
using ScintillaNET.WPF;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace IDE_UI.Controls
{

    public interface ICMMCodeEditorDelegate
    {
        void breakPointChanged(CMMCodeEditor sender, List<int> points);

        void didAddOrRemoveBreakPoint(CMMCodeEditor sender, bool addOrRemove, int breakPoint);
    }

    public partial class CMMCodeEditor : UserControl
    {

        
        #region 其它属性

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
        private const int BREAKPOINT_MARGIN = 2;

        private const int DEBUG_MARKER = 2;

        private const int BREAKPOINT_MARKER = 4;

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

        public ICMMCodeEditorDelegate editorDelegate;

        private int currentDebugLine = -1;

        public int CurrentDebugLine {
            get {
                return currentDebugLine;
            }
            set {
                setDebugMarker(currentDebugLine, value);
                currentDebugLine = value;
            }
        }

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

        #region Bookmarks // 调试标记

        public void ClearDebugMarker()
        {
            CurrentDebugLine = -1;
        }

        private void setDebugMarker(int oldLine, int newLine)
        {
            const uint mask = (1 << DEBUG_MARKER);
            if (oldLine >= 0) {
                Line currentLine = textEditor.Lines[oldLine];
                
                uint oldMarkers = currentLine.MarkerGet();
                if ((oldMarkers & mask) > 0) {
                    currentLine.MarkerDelete(DEBUG_MARKER);
                }
            }
            if(newLine < 0) {
                return;
            }
            
            Line newL = textEditor.Lines[newLine];
            uint newMarkers = newL.MarkerGet();
            if ((newMarkers & mask) == 0) {
                newL.MarkerAdd(DEBUG_MARKER);
            }
        }
        

        private void toggleBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Line currentLine = textEditor.Lines[textEditor.CurrentLine];
            const uint mask = (1 << DEBUG_MARKER);
            uint markers = currentLine.MarkerGet();
            if ((markers & mask) > 0) {
                currentLine.MarkerDelete(DEBUG_MARKER);
            }
            else {
                currentLine.MarkerAdd(DEBUG_MARKER);
            }
        }

        private void previousBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int lineNumber = textEditor.Lines[textEditor.CurrentLine - 1].MarkerPrevious(1 << DEBUG_MARKER);
            if (lineNumber != -1)
                textEditor.Lines[lineNumber].Goto();
        }

        private void nextBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int lineNumber = textEditor.Lines[textEditor.CurrentLine + 1].MarkerNext(1 << DEBUG_MARKER);
            if (lineNumber != -1)
                textEditor.Lines[lineNumber].Goto();
        }

        private void clearBookmarksMenuItem_Click(object sender, RoutedEventArgs e)
        {
            textEditor.MarkerDeleteAll(DEBUG_MARKER);
        }

        #endregion Bookmarks


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

            // Show EOL?
            ScintillaNet.ViewEol = false;

            // Set the zoom
            ScintillaNet.Zoom = _zoomLevel;
        }

        private void ScintillaNet_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

        }

        /// <summary>
        /// 点击边栏时。目前主要用来加断点。
        /// </summary>
        private void TextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            const uint mask = (1 << BREAKPOINT_MARKER);

            //
            if (e.Margin == BREAKPOINT_MARGIN) {
                
                var line = textEditor.Lines[textEditor.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0) {
                    Debug.WriteLine(line.MarkerGet());
                    line.MarkerDelete(BREAKPOINT_MARKER);
                    editorDelegate?.didAddOrRemoveBreakPoint(this, false, line.Index + 1);
                }
                else {
                    line.MarkerAdd(BREAKPOINT_MARKER);
                    editorDelegate?.didAddOrRemoveBreakPoint(this, true, line.Index + 1);
                }
                editorDelegate?.breakPointChanged(this, GetBreakPoints());
            }
        }

        /// <summary>
        /// 获取断点。
        /// </summary>
        public List<int> GetBreakPoints()
        {
            const uint mask = (1 << BREAKPOINT_MARKER);

            List<int> points = new List<int>();
            foreach (Line l in textEditor.Lines) {
                if ((l.MarkerGet() & mask) > 0) {
                    points.Add(l.Index + 1);
                }
            }
            return points;
        }

        #region 自动补全
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
                    if (textEditor.SelectionStart - 3 >= 0 && textEditor.Text[textEditor.SelectionStart - 3] == '{') {

                        textEditor.Text = textEditor.Text.Insert(textEditor.SelectionStart - 2, "\r\n");
                        textEditor.SelectionStart = oldPos;
                        //Debug.WriteLine(textEditor.CurrentLine);
                        var indent = textEditor.Lines[textEditor.CurrentLine].Indentation = textEditor.Lines[textEditor.CurrentLine - 1].Indentation + 4;
                        textEditor.Lines[textEditor.CurrentLine + 1].Indentation = textEditor.Lines[textEditor.CurrentLine - 1].Indentation;
                        textEditor.SelectionStart = oldPos + indent;
                        //Debug.WriteLine(textEditor.CurrentLine);

                    }
                    //否则，维持上一行的缩进。
                    else {
                        var indent = textEditor.Lines[textEditor.CurrentLine].Indentation = textEditor.Lines[textEditor.CurrentLine - 1].Indentation;
                        textEditor.SelectionStart += indent;
                    }

                    break;
                default:
                    performAutoComplete();
                    break;
            }
            //Debug.WriteLine(textEditor.SelectionStart);
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

        #endregion


        /// <summary>
        /// 暂时没用
        /// </summary>
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
