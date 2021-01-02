using ScintillaNET;
using ScintillaNET.WPF;
using System.Windows.Media;


namespace IDE_UI.Controls
{
    public partial class CMMCodeEditor
    {

        private void InitIndicator(ScintillaWPF ScintillaNet)
        {
            const int NUM = 8;

            // Remove all uses of our indicator
            textEditor.IndicatorCurrent = NUM;
            textEditor.IndicatorClearRange(0, textEditor.TextLength);

            // Update indicator appearance
            textEditor.Indicators[NUM].Style = IndicatorStyle.FullBox;
            textEditor.Indicators[NUM].Under = false;
            textEditor.Indicators[NUM].ForeColor = System.Drawing.Color.Red;
            textEditor.Indicators[NUM].OutlineAlpha = 100;
            textEditor.Indicators[NUM].Alpha = 100;
        }

        private void InitBookmarkMargin(ScintillaWPF ScintillaNet)
        {
            //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = ScintillaNet.Margins[BREAKPOINT_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;

            // !!!!!!!!!!!注意
            margin.Mask = 1 << BREAKPOINT_MARKER;
            margin.Cursor = MarginCursor.Arrow;

            var Dmarker = ScintillaNet.Markers[DEBUG_MARKER];
            Dmarker.SetBackColor(IntToColor(0xFF003B));
            Dmarker.SetForeColor(IntToColor(0x000000));
            Dmarker.SetAlpha(100);

            var Bmarker = ScintillaNet.Markers[BREAKPOINT_MARKER];
            Bmarker.Symbol = MarkerSymbol.Circle;
            Bmarker.SetBackColor(IntToColor(0xFF003B));
            Bmarker.SetForeColor(IntToColor(0xFF003B));
            Bmarker.SetAlpha(100);

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
    }
}
