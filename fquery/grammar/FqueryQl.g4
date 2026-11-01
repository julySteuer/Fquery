grammar FqueryQl;

expr: statement link*;

link: (DISJ | CONJ) statement;

statement: func_call | scoped_expr;

func_call: count_file_set | match | includes;

count_file_set: COUNT_OCCURENCESES_NAME OBRACE file_set_expr COMMA range_qualifier CBRACE; 
// Make this: match(<ProperyName>, <RangeDescriptor> | <ExactValue>) with ProperyName = FilePath | FileName | Size
match: MATCH_NAME OBRACE STRING CBRACE;
includes: INCLUDES_NAME OBRACE STRING COMMA range_qualifier CBRACE;

file_set_expr: expr | scoped_expr | func_call;

range_qualifier: ordering NUM;

ordering: GT | ST | EQ;

scoped_expr: OBRACE expr CBRACE;

COUNT_OCCURENCESES_NAME: 'count_file_set';
MATCH_NAME: 'match';
INCLUDES_NAME: 'includes';
OBRACE: '(';
CBRACE: ')';
COMMA: ',';
STRING: '"' .+? '"';
NUM: [0-9]+;

GT: '>';
ST: '<';
EQ: '=';

DISJ: 'AND';
CONJ: 'OR';

WS: [ \t] -> skip;