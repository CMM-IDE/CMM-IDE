grammar CMM;

import CMMLex;

statements
	: statement ( statement )*
	;

statement
	: callStatement
	| expressionStatement
	| declaration                                                   
	| assignStatement                                               
	| whileStatement                                                
	| doWhileStatement                                              
	| forStatement                                                  
	| ifStatement                                                   
	| readStatement                                                 
	| writeStatement                                                
	| jumpStatement                                                 
	;	

expressionStatement
	: ( expression )? ';'
	;

expression
	: expression '&&' boolExpression
	| expression '||' boolExpression
	| '!' expression
	| boolExpression
	;

expressionList
	: expressionList ',' expression 
	| expression
	;

boolExpression
	: additiveExpression relationalOperator additiveExpression 
	| additiveExpression
	;

relationalOperator
	: '<=' 	
	| '>=' 	
	| '==' 	
	| '<' 	
	| '>' 	
	| '<>'
	;

additiveExpression
	: additiveExpression operator=('+'|'-') term
	| term
	;

term
	: term operator=('*'|'/') factor
	| factor
	;

factor
	: Identifier
	| Identifier '[' expression ']'
	| '(' expression ')'
	| IntegerLiteral
	| RealLiteral
	| 'true'
	| 'false'
	| '-' factor
	| Identifier '(' expressionList ')'
	| Identifier '(' ')'
	;

declaration
	: variableDeclaration ';'
	| functionDeclaration
	;

variableDeclaration
	: typeSpecifier initializerList
	;

functionDeclaration
	: returnType Identifier parameterClause codeBlock
	;

initializerList
	: initializer
	| initializer ',' initializerList
	;

initializer
	: Identifier '=' expression
	| Identifier '[' expression ']' '=' '[' expression ( ',' expression )* ']'
	;

returnType
	: 'int' 	
	| 'real' 	
	| 'bool' 	
	| 'void'
	;

parameterClause
	: '(' ')' 
	| '(' parameterList ')'
	;

parameterList
	: typeSpecifier Identifier 
	| typeSpecifier Identifier '['']'
	| typeSpecifier Identifier ',' parameterList
	| typeSpecifier Identifier '['']' ',' parameterList
	;

typeSpecifier
	: 'real' 	
	| 'int' 	
	| 'bool'
	;

assignStatement
	: assignment ';'
	;

assignment
	: leftValue '=' expression
	;

leftValue
	: Identifier 
	| Identifier '[' expression ']'
	;

whileStatement
	: 'while' '(' expression ')' codeBlock
	;

doWhileStatement
	: 'do' codeBlock 'while' '(' expression ')' ';'
	;

forStatement
	: 'for' '(' ( forInitializer )? ';' ( expression )? ';' ( assignment )? ')' codeBlock
	;

forInitializer
	: variableDeclaration
	| assignment
	;

ifStatement
	: 'if' '('expression ')' codeBlock ( elseClause )*
	;

elseClause
	: 'else' codeBlock 	
	| 'else' ifStatement
	;

readStatement
	: 'read' '(' leftValue ( ',' leftValue )* ')' ';'
	;

writeStatement
	: 'write' '(' ( expression )? ( ',' expression )* ')' ';'
	;

jumpStatement
	: 'break' ';'
	| 'continue' ';'
	| 'return' ';'
	| 'return' expression ';'
	;

callStatement
	: Identifier '(' expressionList ')' ';'
	| Identifier '(' ')' ';'
	;

codeBlock
	: '{' statements '}'
	;

