using Antlr4.Runtime;
using fquery.domain;
using fquery.model;
using Range = fquery.model.Range;

namespace fquery.adapter;

public class FqueryQlAntlrAdapter : ParseQuery.IPort
{
    public IQueryAst GetAst(string query)
    {
        var antlrInputStream = new AntlrInputStream(query);
        var lexer = new FqueryQlLexer(antlrInputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new FqueryQlParser(tokenStream);
        return ParseExpression(parser.expr());
    }

    private IExpression ParseExpression(FqueryQlParser.ExprContext exprContext)
    {
        var statement = exprContext.statement();

        var links = exprContext.link();

        var parsedLastStatement = ParseStatement(statement);

        return links.Aggregate(new Expression(parsedLastStatement) as IExpression, ParseLink);
    }

    private string TrimString(string inp)
    {
        return inp.Trim('"');
    }

    private IStatement ParseStatement(FqueryQlParser.StatementContext statementContext)
    {
        var scopedExpr = statementContext.scoped_expr();
        if (scopedExpr is not null)
        {
            return ParseExpression(scopedExpr.expr());
        }

        return ParseFunctionCall(statementContext.func_call());
    }

    private IFunctionCall ParseFunctionCall(FqueryQlParser.Func_callContext callContext)
    {
        var includes = callContext.includes();
        var match = callContext.match();
        
        if (includes is not null)
        {
            return ParseIncludes(includes);
        }

        if (match is not null)
        {
            return ParseMatch(match);
        }

        return ParseCountFileSet(callContext.count_file_set());
    }

    private Includes ParseIncludes(FqueryQlParser.IncludesContext includesContext)
    {
        var rangeDescriptor = ParseRangeDescriptor(includesContext.range_qualifier());
        return new Includes(TrimString(includesContext.STRING().GetText()), rangeDescriptor); // Process string to make the "" go away
    }

    private RangeDescriptor ParseRangeDescriptor(FqueryQlParser.Range_qualifierContext rangeQualifierContext)
    {
        var range = ParseRange(rangeQualifierContext.ordering());
        var number = TrimString(rangeQualifierContext.NUM().GetText());

        return new RangeDescriptor(range, int.Parse(number));
    }

    private Range ParseRange(FqueryQlParser.OrderingContext orderingContext)
    {
        if (orderingContext.GT() is not null)
            return Range.GreaterThan;
        if (orderingContext.ST() is not null)
            return Range.SmallerThan;
        return Range.Equals;
    }

    private Match ParseMatch(FqueryQlParser.MatchContext matchContext)
    {
        return new Match(TrimString(matchContext.STRING().GetText()));
    }

    private CountFileSet ParseCountFileSet(FqueryQlParser.Count_file_setContext countFileSetContext)
    {
        var rangeDescriptor = ParseRangeDescriptor(countFileSetContext.range_qualifier());

        var fileSetExpression = ParseFileSetExpression(countFileSetContext.file_set_expr());
        return new CountFileSet(fileSetExpression, rangeDescriptor);
    }

    private IExpression ParseFileSetExpression(FqueryQlParser.File_set_exprContext fileSetExprContext)
    {
        var scopedExpr = fileSetExprContext.scoped_expr();
        var expr = fileSetExprContext.expr();
        var funcCall = fileSetExprContext.func_call();

        if (scopedExpr is not null)
        {
            return ParseExpression(scopedExpr.expr());
        }

        if (expr is not null)
        {
            return ParseExpression(expr);
        }

        return new Expression(ParseFunctionCall(funcCall));
    }

    private IExpression ParseLink(IExpression next, FqueryQlParser.LinkContext linkContext)
    {
        var statement = ParseStatement(linkContext.statement());
        var or = linkContext.CONJ();

        if (or is null)
        {
            return new AndExpression(statement, next);
        }

        return new OrExpression(statement, next);
    }
}