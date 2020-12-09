//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from CMM.g4 by ANTLR 4.8

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public partial class CMMLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		If=1, Else=2, While=3, Do=4, For=5, Return=6, Break=7, Continue=8, Int=9, 
		Real=10, Bool=11, Void=12, Read=13, Write=14, Add=15, Sub=16, Mul=17, 
		Div=18, Assign=19, Equal=20, NotEqual=21, Less=22, LessEqual=23, Greater=24, 
		GreaterEqual=25, Not=26, And=27, Or=28, LeftParen=29, RightParen=30, LeftBarce=31, 
		RightBrace=32, LeftBracket=33, RightBracket=34, Semicolon=35, Comma=36, 
		Dot=37, Dash=38, Identifier=39, IntegerLiteral=40, RealLiteral=41, Alpha=42, 
		Digit=43, Whitespce=44, Comment=45;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"If", "Else", "While", "Do", "For", "Return", "Break", "Continue", "Int", 
		"Real", "Bool", "Void", "Read", "Write", "Add", "Sub", "Mul", "Div", "Assign", 
		"Equal", "NotEqual", "Less", "LessEqual", "Greater", "GreaterEqual", "Not", 
		"And", "Or", "LeftParen", "RightParen", "LeftBarce", "RightBrace", "LeftBracket", 
		"RightBracket", "Semicolon", "Comma", "Dot", "Dash", "Identifier", "IntegerLiteral", 
		"RealLiteral", "Alpha", "Digit", "Whitespce", "Comment"
	};


	public CMMLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public CMMLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'if'", "'else'", "'while'", "'do'", "'for'", "'return'", "'break'", 
		"'continue'", "'int'", "'real'", "'bool'", "'void'", "'read'", "'write'", 
		"'+'", "'-'", "'*'", "'/'", "'='", "'=='", "'<>'", "'<'", "'<='", "'>'", 
		"'>='", "'!'", "'&&'", "'||'", "'('", "')'", "'{'", "'}'", "'['", "']'", 
		"';'", "','", "'.'", "'_'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "If", "Else", "While", "Do", "For", "Return", "Break", "Continue", 
		"Int", "Real", "Bool", "Void", "Read", "Write", "Add", "Sub", "Mul", "Div", 
		"Assign", "Equal", "NotEqual", "Less", "LessEqual", "Greater", "GreaterEqual", 
		"Not", "And", "Or", "LeftParen", "RightParen", "LeftBarce", "RightBrace", 
		"LeftBracket", "RightBracket", "Semicolon", "Comma", "Dot", "Dash", "Identifier", 
		"IntegerLiteral", "RealLiteral", "Alpha", "Digit", "Whitespce", "Comment"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "CMM.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static CMMLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '/', '\x110', '\b', '\x1', '\x4', '\x2', '\t', '\x2', 
		'\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', 
		'\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', 
		'\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', 
		'\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x4', '\xE', 
		'\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', '\x10', '\t', '\x10', '\x4', 
		'\x11', '\t', '\x11', '\x4', '\x12', '\t', '\x12', '\x4', '\x13', '\t', 
		'\x13', '\x4', '\x14', '\t', '\x14', '\x4', '\x15', '\t', '\x15', '\x4', 
		'\x16', '\t', '\x16', '\x4', '\x17', '\t', '\x17', '\x4', '\x18', '\t', 
		'\x18', '\x4', '\x19', '\t', '\x19', '\x4', '\x1A', '\t', '\x1A', '\x4', 
		'\x1B', '\t', '\x1B', '\x4', '\x1C', '\t', '\x1C', '\x4', '\x1D', '\t', 
		'\x1D', '\x4', '\x1E', '\t', '\x1E', '\x4', '\x1F', '\t', '\x1F', '\x4', 
		' ', '\t', ' ', '\x4', '!', '\t', '!', '\x4', '\"', '\t', '\"', '\x4', 
		'#', '\t', '#', '\x4', '$', '\t', '$', '\x4', '%', '\t', '%', '\x4', '&', 
		'\t', '&', '\x4', '\'', '\t', '\'', '\x4', '(', '\t', '(', '\x4', ')', 
		'\t', ')', '\x4', '*', '\t', '*', '\x4', '+', '\t', '+', '\x4', ',', '\t', 
		',', '\x4', '-', '\t', '-', '\x4', '.', '\t', '.', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x5', '\x3', '\x5', '\x3', 
		'\x5', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', 
		'\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', 
		'\x3', '\a', '\x3', '\b', '\x3', '\b', '\x3', '\b', '\x3', '\b', '\x3', 
		'\b', '\x3', '\b', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', '\t', 
		'\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', 
		'\n', '\x3', '\n', '\x3', '\n', '\x3', '\n', '\x3', '\v', '\x3', '\v', 
		'\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', 
		'\f', '\x3', '\f', '\x3', '\f', '\x3', '\r', '\x3', '\r', '\x3', '\r', 
		'\x3', '\r', '\x3', '\r', '\x3', '\xE', '\x3', '\xE', '\x3', '\xE', '\x3', 
		'\xE', '\x3', '\xE', '\x3', '\xF', '\x3', '\xF', '\x3', '\xF', '\x3', 
		'\xF', '\x3', '\xF', '\x3', '\xF', '\x3', '\x10', '\x3', '\x10', '\x3', 
		'\x11', '\x3', '\x11', '\x3', '\x12', '\x3', '\x12', '\x3', '\x13', '\x3', 
		'\x13', '\x3', '\x14', '\x3', '\x14', '\x3', '\x15', '\x3', '\x15', '\x3', 
		'\x15', '\x3', '\x16', '\x3', '\x16', '\x3', '\x16', '\x3', '\x17', '\x3', 
		'\x17', '\x3', '\x18', '\x3', '\x18', '\x3', '\x18', '\x3', '\x19', '\x3', 
		'\x19', '\x3', '\x1A', '\x3', '\x1A', '\x3', '\x1A', '\x3', '\x1B', '\x3', 
		'\x1B', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1D', '\x3', 
		'\x1D', '\x3', '\x1D', '\x3', '\x1E', '\x3', '\x1E', '\x3', '\x1F', '\x3', 
		'\x1F', '\x3', ' ', '\x3', ' ', '\x3', '!', '\x3', '!', '\x3', '\"', '\x3', 
		'\"', '\x3', '#', '\x3', '#', '\x3', '$', '\x3', '$', '\x3', '%', '\x3', 
		'%', '\x3', '&', '\x3', '&', '\x3', '\'', '\x3', '\'', '\x3', '(', '\x3', 
		'(', '\x3', '(', '\x3', '(', '\a', '(', '\xE1', '\n', '(', '\f', '(', 
		'\xE', '(', '\xE4', '\v', '(', '\x3', '(', '\x3', '(', '\x5', '(', '\xE8', 
		'\n', '(', '\x5', '(', '\xEA', '\n', '(', '\x3', ')', '\x3', ')', '\a', 
		')', '\xEE', '\n', ')', '\f', ')', '\xE', ')', '\xF1', '\v', ')', '\x3', 
		'*', '\x3', '*', '\x3', '*', '\x5', '*', '\xF6', '\n', '*', '\x3', '+', 
		'\x3', '+', '\x3', ',', '\x3', ',', '\x3', '-', '\x6', '-', '\xFD', '\n', 
		'-', '\r', '-', '\xE', '-', '\xFE', '\x3', '-', '\x3', '-', '\x3', '.', 
		'\x3', '.', '\x3', '.', '\x3', '.', '\a', '.', '\x107', '\n', '.', '\f', 
		'.', '\xE', '.', '\x10A', '\v', '.', '\x3', '.', '\x3', '.', '\x3', '.', 
		'\x3', '.', '\x3', '.', '\x3', '\x108', '\x2', '/', '\x3', '\x3', '\x5', 
		'\x4', '\a', '\x5', '\t', '\x6', '\v', '\a', '\r', '\b', '\xF', '\t', 
		'\x11', '\n', '\x13', '\v', '\x15', '\f', '\x17', '\r', '\x19', '\xE', 
		'\x1B', '\xF', '\x1D', '\x10', '\x1F', '\x11', '!', '\x12', '#', '\x13', 
		'%', '\x14', '\'', '\x15', ')', '\x16', '+', '\x17', '-', '\x18', '/', 
		'\x19', '\x31', '\x1A', '\x33', '\x1B', '\x35', '\x1C', '\x37', '\x1D', 
		'\x39', '\x1E', ';', '\x1F', '=', ' ', '?', '!', '\x41', '\"', '\x43', 
		'#', '\x45', '$', 'G', '%', 'I', '&', 'K', '\'', 'M', '(', 'O', ')', 'Q', 
		'*', 'S', '+', 'U', ',', 'W', '-', 'Y', '.', '[', '/', '\x3', '\x2', '\x5', 
		'\x4', '\x2', '\x43', '\\', '\x63', '|', '\x3', '\x2', '\x32', ';', '\x5', 
		'\x2', '\v', '\f', '\xF', '\xF', '\"', '\"', '\x2', '\x118', '\x2', '\x3', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x5', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\a', '\x3', '\x2', '\x2', '\x2', '\x2', '\t', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\v', '\x3', '\x2', '\x2', '\x2', '\x2', '\r', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\xF', '\x3', '\x2', '\x2', '\x2', '\x2', '\x11', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x13', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x15', '\x3', '\x2', '\x2', '\x2', '\x2', '\x17', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x19', '\x3', '\x2', '\x2', '\x2', '\x2', '\x1B', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x1D', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x1F', '\x3', '\x2', '\x2', '\x2', '\x2', '!', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '#', '\x3', '\x2', '\x2', '\x2', '\x2', '%', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\'', '\x3', '\x2', '\x2', '\x2', '\x2', ')', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '+', '\x3', '\x2', '\x2', '\x2', '\x2', '-', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '/', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x31', '\x3', '\x2', '\x2', '\x2', '\x2', '\x33', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x35', '\x3', '\x2', '\x2', '\x2', '\x2', '\x37', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\x39', '\x3', '\x2', '\x2', '\x2', '\x2', 
		';', '\x3', '\x2', '\x2', '\x2', '\x2', '=', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '?', '\x3', '\x2', '\x2', '\x2', '\x2', '\x41', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x43', '\x3', '\x2', '\x2', '\x2', '\x2', '\x45', '\x3', 
		'\x2', '\x2', '\x2', '\x2', 'G', '\x3', '\x2', '\x2', '\x2', '\x2', 'I', 
		'\x3', '\x2', '\x2', '\x2', '\x2', 'K', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'M', '\x3', '\x2', '\x2', '\x2', '\x2', 'O', '\x3', '\x2', '\x2', '\x2', 
		'\x2', 'Q', '\x3', '\x2', '\x2', '\x2', '\x2', 'S', '\x3', '\x2', '\x2', 
		'\x2', '\x2', 'U', '\x3', '\x2', '\x2', '\x2', '\x2', 'W', '\x3', '\x2', 
		'\x2', '\x2', '\x2', 'Y', '\x3', '\x2', '\x2', '\x2', '\x2', '[', '\x3', 
		'\x2', '\x2', '\x2', '\x3', ']', '\x3', '\x2', '\x2', '\x2', '\x5', '`', 
		'\x3', '\x2', '\x2', '\x2', '\a', '\x65', '\x3', '\x2', '\x2', '\x2', 
		'\t', 'k', '\x3', '\x2', '\x2', '\x2', '\v', 'n', '\x3', '\x2', '\x2', 
		'\x2', '\r', 'r', '\x3', '\x2', '\x2', '\x2', '\xF', 'y', '\x3', '\x2', 
		'\x2', '\x2', '\x11', '\x7F', '\x3', '\x2', '\x2', '\x2', '\x13', '\x88', 
		'\x3', '\x2', '\x2', '\x2', '\x15', '\x8C', '\x3', '\x2', '\x2', '\x2', 
		'\x17', '\x91', '\x3', '\x2', '\x2', '\x2', '\x19', '\x96', '\x3', '\x2', 
		'\x2', '\x2', '\x1B', '\x9B', '\x3', '\x2', '\x2', '\x2', '\x1D', '\xA0', 
		'\x3', '\x2', '\x2', '\x2', '\x1F', '\xA6', '\x3', '\x2', '\x2', '\x2', 
		'!', '\xA8', '\x3', '\x2', '\x2', '\x2', '#', '\xAA', '\x3', '\x2', '\x2', 
		'\x2', '%', '\xAC', '\x3', '\x2', '\x2', '\x2', '\'', '\xAE', '\x3', '\x2', 
		'\x2', '\x2', ')', '\xB0', '\x3', '\x2', '\x2', '\x2', '+', '\xB3', '\x3', 
		'\x2', '\x2', '\x2', '-', '\xB6', '\x3', '\x2', '\x2', '\x2', '/', '\xB8', 
		'\x3', '\x2', '\x2', '\x2', '\x31', '\xBB', '\x3', '\x2', '\x2', '\x2', 
		'\x33', '\xBD', '\x3', '\x2', '\x2', '\x2', '\x35', '\xC0', '\x3', '\x2', 
		'\x2', '\x2', '\x37', '\xC2', '\x3', '\x2', '\x2', '\x2', '\x39', '\xC5', 
		'\x3', '\x2', '\x2', '\x2', ';', '\xC8', '\x3', '\x2', '\x2', '\x2', '=', 
		'\xCA', '\x3', '\x2', '\x2', '\x2', '?', '\xCC', '\x3', '\x2', '\x2', 
		'\x2', '\x41', '\xCE', '\x3', '\x2', '\x2', '\x2', '\x43', '\xD0', '\x3', 
		'\x2', '\x2', '\x2', '\x45', '\xD2', '\x3', '\x2', '\x2', '\x2', 'G', 
		'\xD4', '\x3', '\x2', '\x2', '\x2', 'I', '\xD6', '\x3', '\x2', '\x2', 
		'\x2', 'K', '\xD8', '\x3', '\x2', '\x2', '\x2', 'M', '\xDA', '\x3', '\x2', 
		'\x2', '\x2', 'O', '\xDC', '\x3', '\x2', '\x2', '\x2', 'Q', '\xEB', '\x3', 
		'\x2', '\x2', '\x2', 'S', '\xF2', '\x3', '\x2', '\x2', '\x2', 'U', '\xF7', 
		'\x3', '\x2', '\x2', '\x2', 'W', '\xF9', '\x3', '\x2', '\x2', '\x2', 'Y', 
		'\xFC', '\x3', '\x2', '\x2', '\x2', '[', '\x102', '\x3', '\x2', '\x2', 
		'\x2', ']', '^', '\a', 'k', '\x2', '\x2', '^', '_', '\a', 'h', '\x2', 
		'\x2', '_', '\x4', '\x3', '\x2', '\x2', '\x2', '`', '\x61', '\a', 'g', 
		'\x2', '\x2', '\x61', '\x62', '\a', 'n', '\x2', '\x2', '\x62', '\x63', 
		'\a', 'u', '\x2', '\x2', '\x63', '\x64', '\a', 'g', '\x2', '\x2', '\x64', 
		'\x6', '\x3', '\x2', '\x2', '\x2', '\x65', '\x66', '\a', 'y', '\x2', '\x2', 
		'\x66', 'g', '\a', 'j', '\x2', '\x2', 'g', 'h', '\a', 'k', '\x2', '\x2', 
		'h', 'i', '\a', 'n', '\x2', '\x2', 'i', 'j', '\a', 'g', '\x2', '\x2', 
		'j', '\b', '\x3', '\x2', '\x2', '\x2', 'k', 'l', '\a', '\x66', '\x2', 
		'\x2', 'l', 'm', '\a', 'q', '\x2', '\x2', 'm', '\n', '\x3', '\x2', '\x2', 
		'\x2', 'n', 'o', '\a', 'h', '\x2', '\x2', 'o', 'p', '\a', 'q', '\x2', 
		'\x2', 'p', 'q', '\a', 't', '\x2', '\x2', 'q', '\f', '\x3', '\x2', '\x2', 
		'\x2', 'r', 's', '\a', 't', '\x2', '\x2', 's', 't', '\a', 'g', '\x2', 
		'\x2', 't', 'u', '\a', 'v', '\x2', '\x2', 'u', 'v', '\a', 'w', '\x2', 
		'\x2', 'v', 'w', '\a', 't', '\x2', '\x2', 'w', 'x', '\a', 'p', '\x2', 
		'\x2', 'x', '\xE', '\x3', '\x2', '\x2', '\x2', 'y', 'z', '\a', '\x64', 
		'\x2', '\x2', 'z', '{', '\a', 't', '\x2', '\x2', '{', '|', '\a', 'g', 
		'\x2', '\x2', '|', '}', '\a', '\x63', '\x2', '\x2', '}', '~', '\a', 'm', 
		'\x2', '\x2', '~', '\x10', '\x3', '\x2', '\x2', '\x2', '\x7F', '\x80', 
		'\a', '\x65', '\x2', '\x2', '\x80', '\x81', '\a', 'q', '\x2', '\x2', '\x81', 
		'\x82', '\a', 'p', '\x2', '\x2', '\x82', '\x83', '\a', 'v', '\x2', '\x2', 
		'\x83', '\x84', '\a', 'k', '\x2', '\x2', '\x84', '\x85', '\a', 'p', '\x2', 
		'\x2', '\x85', '\x86', '\a', 'w', '\x2', '\x2', '\x86', '\x87', '\a', 
		'g', '\x2', '\x2', '\x87', '\x12', '\x3', '\x2', '\x2', '\x2', '\x88', 
		'\x89', '\a', 'k', '\x2', '\x2', '\x89', '\x8A', '\a', 'p', '\x2', '\x2', 
		'\x8A', '\x8B', '\a', 'v', '\x2', '\x2', '\x8B', '\x14', '\x3', '\x2', 
		'\x2', '\x2', '\x8C', '\x8D', '\a', 't', '\x2', '\x2', '\x8D', '\x8E', 
		'\a', 'g', '\x2', '\x2', '\x8E', '\x8F', '\a', '\x63', '\x2', '\x2', '\x8F', 
		'\x90', '\a', 'n', '\x2', '\x2', '\x90', '\x16', '\x3', '\x2', '\x2', 
		'\x2', '\x91', '\x92', '\a', '\x64', '\x2', '\x2', '\x92', '\x93', '\a', 
		'q', '\x2', '\x2', '\x93', '\x94', '\a', 'q', '\x2', '\x2', '\x94', '\x95', 
		'\a', 'n', '\x2', '\x2', '\x95', '\x18', '\x3', '\x2', '\x2', '\x2', '\x96', 
		'\x97', '\a', 'x', '\x2', '\x2', '\x97', '\x98', '\a', 'q', '\x2', '\x2', 
		'\x98', '\x99', '\a', 'k', '\x2', '\x2', '\x99', '\x9A', '\a', '\x66', 
		'\x2', '\x2', '\x9A', '\x1A', '\x3', '\x2', '\x2', '\x2', '\x9B', '\x9C', 
		'\a', 't', '\x2', '\x2', '\x9C', '\x9D', '\a', 'g', '\x2', '\x2', '\x9D', 
		'\x9E', '\a', '\x63', '\x2', '\x2', '\x9E', '\x9F', '\a', '\x66', '\x2', 
		'\x2', '\x9F', '\x1C', '\x3', '\x2', '\x2', '\x2', '\xA0', '\xA1', '\a', 
		'y', '\x2', '\x2', '\xA1', '\xA2', '\a', 't', '\x2', '\x2', '\xA2', '\xA3', 
		'\a', 'k', '\x2', '\x2', '\xA3', '\xA4', '\a', 'v', '\x2', '\x2', '\xA4', 
		'\xA5', '\a', 'g', '\x2', '\x2', '\xA5', '\x1E', '\x3', '\x2', '\x2', 
		'\x2', '\xA6', '\xA7', '\a', '-', '\x2', '\x2', '\xA7', ' ', '\x3', '\x2', 
		'\x2', '\x2', '\xA8', '\xA9', '\a', '/', '\x2', '\x2', '\xA9', '\"', '\x3', 
		'\x2', '\x2', '\x2', '\xAA', '\xAB', '\a', ',', '\x2', '\x2', '\xAB', 
		'$', '\x3', '\x2', '\x2', '\x2', '\xAC', '\xAD', '\a', '\x31', '\x2', 
		'\x2', '\xAD', '&', '\x3', '\x2', '\x2', '\x2', '\xAE', '\xAF', '\a', 
		'?', '\x2', '\x2', '\xAF', '(', '\x3', '\x2', '\x2', '\x2', '\xB0', '\xB1', 
		'\a', '?', '\x2', '\x2', '\xB1', '\xB2', '\a', '?', '\x2', '\x2', '\xB2', 
		'*', '\x3', '\x2', '\x2', '\x2', '\xB3', '\xB4', '\a', '>', '\x2', '\x2', 
		'\xB4', '\xB5', '\a', '@', '\x2', '\x2', '\xB5', ',', '\x3', '\x2', '\x2', 
		'\x2', '\xB6', '\xB7', '\a', '>', '\x2', '\x2', '\xB7', '.', '\x3', '\x2', 
		'\x2', '\x2', '\xB8', '\xB9', '\a', '>', '\x2', '\x2', '\xB9', '\xBA', 
		'\a', '?', '\x2', '\x2', '\xBA', '\x30', '\x3', '\x2', '\x2', '\x2', '\xBB', 
		'\xBC', '\a', '@', '\x2', '\x2', '\xBC', '\x32', '\x3', '\x2', '\x2', 
		'\x2', '\xBD', '\xBE', '\a', '@', '\x2', '\x2', '\xBE', '\xBF', '\a', 
		'?', '\x2', '\x2', '\xBF', '\x34', '\x3', '\x2', '\x2', '\x2', '\xC0', 
		'\xC1', '\a', '#', '\x2', '\x2', '\xC1', '\x36', '\x3', '\x2', '\x2', 
		'\x2', '\xC2', '\xC3', '\a', '(', '\x2', '\x2', '\xC3', '\xC4', '\a', 
		'(', '\x2', '\x2', '\xC4', '\x38', '\x3', '\x2', '\x2', '\x2', '\xC5', 
		'\xC6', '\a', '~', '\x2', '\x2', '\xC6', '\xC7', '\a', '~', '\x2', '\x2', 
		'\xC7', ':', '\x3', '\x2', '\x2', '\x2', '\xC8', '\xC9', '\a', '*', '\x2', 
		'\x2', '\xC9', '<', '\x3', '\x2', '\x2', '\x2', '\xCA', '\xCB', '\a', 
		'+', '\x2', '\x2', '\xCB', '>', '\x3', '\x2', '\x2', '\x2', '\xCC', '\xCD', 
		'\a', '}', '\x2', '\x2', '\xCD', '@', '\x3', '\x2', '\x2', '\x2', '\xCE', 
		'\xCF', '\a', '\x7F', '\x2', '\x2', '\xCF', '\x42', '\x3', '\x2', '\x2', 
		'\x2', '\xD0', '\xD1', '\a', ']', '\x2', '\x2', '\xD1', '\x44', '\x3', 
		'\x2', '\x2', '\x2', '\xD2', '\xD3', '\a', '_', '\x2', '\x2', '\xD3', 
		'\x46', '\x3', '\x2', '\x2', '\x2', '\xD4', '\xD5', '\a', '=', '\x2', 
		'\x2', '\xD5', 'H', '\x3', '\x2', '\x2', '\x2', '\xD6', '\xD7', '\a', 
		'.', '\x2', '\x2', '\xD7', 'J', '\x3', '\x2', '\x2', '\x2', '\xD8', '\xD9', 
		'\a', '\x30', '\x2', '\x2', '\xD9', 'L', '\x3', '\x2', '\x2', '\x2', '\xDA', 
		'\xDB', '\a', '\x61', '\x2', '\x2', '\xDB', 'N', '\x3', '\x2', '\x2', 
		'\x2', '\xDC', '\xE9', '\x5', 'U', '+', '\x2', '\xDD', '\xE1', '\x5', 
		'W', ',', '\x2', '\xDE', '\xE1', '\x5', 'U', '+', '\x2', '\xDF', '\xE1', 
		'\a', '\x61', '\x2', '\x2', '\xE0', '\xDD', '\x3', '\x2', '\x2', '\x2', 
		'\xE0', '\xDE', '\x3', '\x2', '\x2', '\x2', '\xE0', '\xDF', '\x3', '\x2', 
		'\x2', '\x2', '\xE1', '\xE4', '\x3', '\x2', '\x2', '\x2', '\xE2', '\xE0', 
		'\x3', '\x2', '\x2', '\x2', '\xE2', '\xE3', '\x3', '\x2', '\x2', '\x2', 
		'\xE3', '\xE7', '\x3', '\x2', '\x2', '\x2', '\xE4', '\xE2', '\x3', '\x2', 
		'\x2', '\x2', '\xE5', '\xE8', '\x5', 'W', ',', '\x2', '\xE6', '\xE8', 
		'\x5', 'U', '+', '\x2', '\xE7', '\xE5', '\x3', '\x2', '\x2', '\x2', '\xE7', 
		'\xE6', '\x3', '\x2', '\x2', '\x2', '\xE8', '\xEA', '\x3', '\x2', '\x2', 
		'\x2', '\xE9', '\xE2', '\x3', '\x2', '\x2', '\x2', '\xE9', '\xEA', '\x3', 
		'\x2', '\x2', '\x2', '\xEA', 'P', '\x3', '\x2', '\x2', '\x2', '\xEB', 
		'\xEF', '\x5', 'W', ',', '\x2', '\xEC', '\xEE', '\x5', 'W', ',', '\x2', 
		'\xED', '\xEC', '\x3', '\x2', '\x2', '\x2', '\xEE', '\xF1', '\x3', '\x2', 
		'\x2', '\x2', '\xEF', '\xED', '\x3', '\x2', '\x2', '\x2', '\xEF', '\xF0', 
		'\x3', '\x2', '\x2', '\x2', '\xF0', 'R', '\x3', '\x2', '\x2', '\x2', '\xF1', 
		'\xEF', '\x3', '\x2', '\x2', '\x2', '\xF2', '\xF5', '\x5', 'Q', ')', '\x2', 
		'\xF3', '\xF4', '\a', '\x30', '\x2', '\x2', '\xF4', '\xF6', '\x5', 'Q', 
		')', '\x2', '\xF5', '\xF3', '\x3', '\x2', '\x2', '\x2', '\xF5', '\xF6', 
		'\x3', '\x2', '\x2', '\x2', '\xF6', 'T', '\x3', '\x2', '\x2', '\x2', '\xF7', 
		'\xF8', '\t', '\x2', '\x2', '\x2', '\xF8', 'V', '\x3', '\x2', '\x2', '\x2', 
		'\xF9', '\xFA', '\t', '\x3', '\x2', '\x2', '\xFA', 'X', '\x3', '\x2', 
		'\x2', '\x2', '\xFB', '\xFD', '\t', '\x4', '\x2', '\x2', '\xFC', '\xFB', 
		'\x3', '\x2', '\x2', '\x2', '\xFD', '\xFE', '\x3', '\x2', '\x2', '\x2', 
		'\xFE', '\xFC', '\x3', '\x2', '\x2', '\x2', '\xFE', '\xFF', '\x3', '\x2', 
		'\x2', '\x2', '\xFF', '\x100', '\x3', '\x2', '\x2', '\x2', '\x100', '\x101', 
		'\b', '-', '\x2', '\x2', '\x101', 'Z', '\x3', '\x2', '\x2', '\x2', '\x102', 
		'\x103', '\a', '\x31', '\x2', '\x2', '\x103', '\x104', '\a', ',', '\x2', 
		'\x2', '\x104', '\x108', '\x3', '\x2', '\x2', '\x2', '\x105', '\x107', 
		'\v', '\x2', '\x2', '\x2', '\x106', '\x105', '\x3', '\x2', '\x2', '\x2', 
		'\x107', '\x10A', '\x3', '\x2', '\x2', '\x2', '\x108', '\x109', '\x3', 
		'\x2', '\x2', '\x2', '\x108', '\x106', '\x3', '\x2', '\x2', '\x2', '\x109', 
		'\x10B', '\x3', '\x2', '\x2', '\x2', '\x10A', '\x108', '\x3', '\x2', '\x2', 
		'\x2', '\x10B', '\x10C', '\a', ',', '\x2', '\x2', '\x10C', '\x10D', '\a', 
		'\x31', '\x2', '\x2', '\x10D', '\x10E', '\x3', '\x2', '\x2', '\x2', '\x10E', 
		'\x10F', '\b', '.', '\x2', '\x2', '\x10F', '\\', '\x3', '\x2', '\x2', 
		'\x2', '\v', '\x2', '\xE0', '\xE2', '\xE7', '\xE9', '\xEF', '\xF5', '\xFE', 
		'\x108', '\x3', '\b', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
