program TestSyntax;

var
  number1: integer;
  number2: real;
  numbers: array[1..10] of integer;

procedure Calculate(a: integer; b: real);
var
  result: real;
begin
  result := a + b;
  number1 := 10;
end;

begin
  number1 := 10;
  number2 := 25.5;
  numbers[1] := number1;

  Calculate(number1, number2);

  number1 := numbers[1];
end.