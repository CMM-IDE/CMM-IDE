lexer grammar CMMLex;

If
	: 'if'
	;

Else
	: 'else'
	;
	
While
	: 'while'
	;
	
Do
	: 'do'
	;
	
For
	: 'for'
	;

Return
	: 'return'
	;

Break
	: 'break'
	;

Continue
	: 'continue'
	;

Int
	: 'int'
	;
	
Real
	: 'real'
	;
	
Bool
	: 'bool'
	;
	
Void
	: 'void'
	;

Read
	: 'read'
	;

Write
	: 'write'
	;

True
	: 'true'
	;

False
	: 'false'
	;

Add
	: '+'
	;
	
Sub
	: '-'
	;
	
Mul
	: '*'
	;

Div
	: '/'
	;

Assign
	: '='
	;

Equal
	: '=='
	;

NotEqual
	: '<>'
	;
	
Less
	: '<'
	;

LessEqual
	: '<='
	;
	
Greater
	: '>'
	;
	
GreaterEqual
	: '>='
	;
	
Not
	: '!'
	;

And
	: '&&'
	;
	
Or
	: '||'
	;
	
LeftParen
	: '('
	;
	
RightParen
	: ')'
	;
	
LeftBarce
	: '{'
	;

RightBrace
	: '}'
	;
	
LeftBracket
	: '['
	;
	
RightBracket
	: ']'
	;
	
Semicolon
	: ';'
	;
	
Comma
	: ','
	;

Dot
	: '.'
	;

Dash
	: '_'
	;

Identifier
	: Alpha (( Digit | Alpha | '_' )* ( Digit | Alpha ))?
	;

IntegerLiteral
	: Digit ( Digit )*
	;

RealLiteral
	: IntegerLiteral ( '.' IntegerLiteral )?
	;

Alpha
	: [a-zA-Z]
	;

Digit
	: [0-9]
	;
	
Whitespce
	: [ \t\n\r]+ -> skip;

Comment
	: '/*' (.)*? '*/' -> skip
	;