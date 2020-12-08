using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class VarValue
{
	public int StartIndex;
	public object Value;

	public VarValue(int startIndex, object value)
	{
		StartIndex = startIndex;
		Value = value;
	}
}

public class TestVisitor: CMMMBaseVisitor<object>
{
	private Dictionary<string, VarValue> Variables = new Dictionary<string, VarValue>();

	public outputStreamDelegate outputStream = null;

	

	public override object VisitIfStatement([NotNull] CMMMParser.IfStatementContext context) {
		var condition = (bool)VisitQuoteExpr(context.quoteExpr());
		if (condition) {
			VisitBlockStatement(context.blockStatement(0));
		}
		else if (context.ChildCount == 5) {
			VisitBlockStatement(context.blockStatement(1));
		}
		return null;
	}


	public override object VisitQuoteExpr([NotNull] CMMMParser.QuoteExprContext context) {
		return VisitExpression(context.expression());
	}

	public override object VisitPrintStatement([NotNull] CMMMParser.PrintStatementContext context) {
		
		var r = VisitExpression(context.expression());
		outputStream?.Print(r.ToString());
		return null;
	}


	public override object VisitWhileStatement([NotNull] CMMMParser.WhileStatementContext context)
	{
		while (true)
		{
			var condition = (bool)VisitExpression(context.expression());
			if (!(condition)) { break; }
			VisitBlockStatement(context.blockStatement());
		}
		return null;
	}


	public override object VisitDoWhileStatement([NotNull] CMMMParser.DoWhileStatementContext context) {
		bool condition;
		do {
			VisitBlockStatement(context.blockStatement());
			condition = (bool)VisitExpression(context.expression());
		} while (condition);
		return null;
	}


	public override object VisitForStatement([NotNull] CMMMParser.ForStatementContext context) {
		for (VisitCommonExpression(context.commonExpression());
		(bool)VisitExpression(context.expression());
		VisitAssignAbleStatement(context.assignAbleStatement())) {
			VisitBlockStatement(context.blockStatement());
		}
		return null;
	}


	public override object VisitDeclareExpression([NotNull] CMMMParser.DeclareExpressionContext context) {
		var assigns = context.declarators().assign();
		foreach (var assign in assigns) {
			var name = assign.Identifier().GetText();
			VarValue obj;
			if (Variables.TryGetValue(name, out obj)) {
				if (obj.StartIndex == assign.Start.StartIndex) {
					return base.VisitDeclareExpression(context);
				}
				throw new Exception($"Variable [{name}] already defined.");
			}
			switch (context.basicType().GetText()) {
				case "number":
					Variables.Add(name, new VarValue(assign.Start.StopIndex, (decimal)0));
					break;
				case "string":
					Variables.Add(name, new VarValue(assign.Start.StopIndex, ""));
					break;
				case "bool":
					Variables.Add(name, new VarValue(assign.Start.StopIndex, false));
					break;
			}
		}
		return base.VisitDeclareExpression(context);
	}


	public override object VisitExpression([NotNull] CMMMParser.ExpressionContext context) {
		var a = VisitAndAndExpression(context.andAndExpression(0));
		for (int i = 1; i < context.ChildCount; i += 2) {
			var b = (bool)VisitAndAndExpression((CMMMParser.AndAndExpressionContext)context.GetChild(i + 1));
			a = (bool)a || b;
		}
		return a;
	}


	public override object VisitAndAndExpression([NotNull] CMMMParser.AndAndExpressionContext context) {
		var a = VisitCmpExpression(context.cmpExpression(0));
		for (int i = 1; i < context.ChildCount; i += 2) {
			var b = (bool)VisitCmpExpression((CMMMParser.CmpExpressionContext)context.GetChild(i + 1));
			a = (bool)a && b;
		}
		return a;
	}


	public override object VisitCmpExpression([NotNull] CMMMParser.CmpExpressionContext context) {
		var a = VisitAddExpression(context.addExpression(0));
		if (context.ChildCount > 2) {
			var b = VisitAddExpression(context.addExpression(1));
			var op = context.GetChild(1).GetText();
			switch (op) {
				case "==":
					return ValueEquals(a, b, context);
				case "<>":
					return !ValueEquals(a, b, context);
				case "<":
					return (decimal)a < (decimal)b;
				case "<=":
					return (decimal)a <= (decimal)b;
				case ">":
					return (decimal)a > (decimal)b;
				case ">=":
					return (decimal)a >= (decimal)b;
			}
			throw new Exception("Unsupported operation.");
		}
		return a;
	}


	public override object VisitAddExpression([NotNull] CMMMParser.AddExpressionContext context) {
		var a = VisitMulExpression(context.mulExpression(0));
		for (int i = 1; i < context.ChildCount; i += 2) {
			var op = context.GetChild(i).GetText();
			var b = VisitMulExpression((CMMMParser.MulExpressionContext)context.GetChild(i + 1));
			switch (op) {
				case "+":
					if (a is string || b is string) {
						a = a.ToString() + b.ToString();
					}
					else {
						a = (decimal)a + (decimal)b;
					}
					break;
				case "-":
					a = (decimal)a - (decimal)b;
					break;
			}
		}
		return a;
	}


	public override object VisitMulExpression([NotNull] CMMMParser.MulExpressionContext context) {
		var a = VisitUnaryExpression(context.unaryExpression(0));
		for (int i = 1; i < context.ChildCount; i += 2) {
			var op = context.GetChild(i).GetText();
			var b = (decimal)VisitUnaryExpression((CMMMParser.UnaryExpressionContext)context.GetChild(i + 1));
			switch (op) {
				case "*":
					a = (decimal)a * b;
					break;
				case "/":
					a = (decimal)a / b;
					break;
			}
		}
		return a;
	}


	public override object VisitUnaryExpression([NotNull] CMMMParser.UnaryExpressionContext context) {
		if (context.ChildCount > 1) {
			switch (context.GetChild(0).GetText()) {
				case "-":
					return -((decimal)VisitUnaryExpression(context.unaryExpression()));
				case "!":
					return !((bool)VisitUnaryExpression(context.unaryExpression()));
			}
		}
		return VisitPrimaryExpression(context.primaryExpression());
	}


	public override object VisitPrimaryExpression([NotNull] CMMMParser.PrimaryExpressionContext context) {
		if (context.ChildCount == 1) {
			var c = context.GetChild(0);
			if (c is CMMMParser.VariableExpressionContext) {
				return VisitVariableExpression((CMMMParser.VariableExpressionContext)c);
			}
			else {
				return VisitNumericLiteral(context.numericLiteral());
			}
		}
		else {
			return VisitExpression(context.expression());
		}
	}


	public override object VisitVariableExpression([NotNull] CMMMParser.VariableExpressionContext context) {
		var n = context.GetText();
		if (n == "true") {
			return true;
		}
		else if (n == "false") {
			return false;
		}
		else if (n.StartsWith("\"")) {
			return n.Substring(1, n.Length - 2);
		}
		VarValue obj;
		if (!Variables.TryGetValue(n, out obj)) {
			throw new Exception($"Use of undeclared identifier [{n}].");
		}
		return obj.Value;
	}


	public override object VisitAssign([NotNull] CMMMParser.AssignContext context) {
		var name = context.Identifier().GetText();
		VarValue obj;
		if (!Variables.TryGetValue(name, out obj)) {
			throw new Exception($"Variable [{name}] not defined.");
		}
		var r = base.VisitAssign(context);
		if (obj != null && obj.Value != null) {
			if (obj.Value.GetType() != r.GetType()) {
				throw new Exception("$Cannot convert type [ {obj.Value.GetType().Name}] to [{r.GetType().Name}].");
			}
		}
		Variables[name] = new VarValue(obj.StartIndex, r);
		return null;
	}

	private bool ValueEquals(object a, object b, ParserRuleContext ctx)
	{
		if (a is bool && b is bool) {
			return (bool)a == (bool)b;
		}
		else if (a is decimal && b is decimal) {
			return (decimal)a == (decimal)b;
		}
		else if (a is string && b is string) {
			return (string)a == (string)b;
		}
		throw new Exception($"Cannot do operation between [{a.GetType().Name}] and [{b.GetType().Name}].");
	}


	public override object VisitNumericLiteral([NotNull] CMMMParser.NumericLiteralContext context) {
		var text = context.Decimal().GetText().Replace("_", "");
		return decimal.Parse(text);
	}
}

