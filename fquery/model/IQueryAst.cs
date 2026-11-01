namespace fquery.model;

public interface IQueryAst;

public interface IExpression : IStatement, IQueryAst;

public record Expression(IStatement Statement) : IExpression;

public record AndExpression(IStatement Statement, IExpression Next) : IExpression;

public record OrExpression(IStatement Statement, IExpression Next) : IExpression;

public interface IStatement;

public interface IFunctionCall : IStatement;

public record Match(string Path) : IFunctionCall;
public record CountFileSet(IExpression Expression, RangeDescriptor RangeDescriptor) : IFunctionCall;

public record Includes(string Path, RangeDescriptor RangeDescriptor) : IFunctionCall;

public enum Range
{
    GreaterThan,
    SmallerThan,
    Equals
}

public record RangeDescriptor(Range Range, int Size);