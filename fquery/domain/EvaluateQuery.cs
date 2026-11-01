using System.Text.RegularExpressions;
using fquery.model;
using fquery.util;
using Match = fquery.model.Match;

namespace fquery.domain;

using MatchVisitor = Func<Match, FileSet>;
using IncludesVisitor = Func<Includes, FileSet>;
using CountFileSetVisitor = Func<CountFileSet, FileSet>;

internal record FileSet(List<FqueryFile> Files)
{
    public static FileSet operator +(FileSet left, FileSet right)
    {
        return new FileSet([.. left.Files, .. right.Files]);
    }

    public int Length => Files.Count;

    public static FileSet Empty()
    {
        return new FileSet([]);
    }

    public FileSet RemoveWithPredicate(Func<FqueryFile, bool> predicate)
    {
        return new FileSet([.. Files.Where(predicate)]);
    }

    public FileSet And(FileSet other)
    {
        return new FileSet([.. Files.Where(file => other.Files.Contains(file))]);
    }
}

internal class ExecutionContext(EvaluateQuery.IPort port, string basePath)
{
    public FileSet ExecuteExpr(IExpression expression)
    {
        return expression switch
        {
            AndExpression andExpression => ExecuteAndExpression(andExpression),
            OrExpression orExpression => ExecuteOrExpression(orExpression),
            Expression baseExpression => ExecuteBaseExpression(baseExpression),
        };
    }

    private FileSet ExecuteBaseExpression(Expression baseExpression)
    {
        var visitorSet = new VisitorSet(ExecuteAddingMatchVisitor, ExecuteAddingIncludesVisitor,
            ExecuteAddingCountFileSetVisitor);
        return ExecuteStatement(baseExpression.Statement, visitorSet);
    }

    private FileSet ExecuteStatement(IStatement statement, VisitorSet visitorSet)
    {
        return statement switch
        {
            IFunctionCall functionCall => ExecuteFunctionCall(functionCall, visitorSet),
            IExpression expression => ExecuteExpr(expression)
        };
    }

    private FileSet ExecuteFunctionCall(IFunctionCall functionCall, VisitorSet visitorSet)
    {
        return functionCall switch
        {
            Match match => visitorSet.MatchVisitor(match),
            Includes includes => visitorSet.IncludesVisitor(includes),
            CountFileSet countFileSet => visitorSet.CountFileSetVisitor(countFileSet)
        };
    }

    private FileSet ExecuteAndExpression(AndExpression andExpression)
    {
        var fileSetNext = ExecuteExpr(andExpression.Next);
        var visitorSet = new VisitorSet(ExecuteSubtractingMatchVisitor(fileSetNext),
            ExecuteSubtractingIncludesVisitor(fileSetNext), ExecuteSubtractingCountFileSetVisitor(fileSetNext));
        
        return andExpression.Statement switch
        {
            IFunctionCall functionCall => ExecuteFunctionCall(functionCall, visitorSet),
            IExpression expression => ExecuteExpr(expression).And(fileSetNext)
        }
        ;
    }

    private FileSet ExecuteOrExpression(OrExpression orExpression)
    {
        var visitorSet = new VisitorSet(ExecuteAddingMatchVisitor, ExecuteAddingIncludesVisitor,
            ExecuteAddingCountFileSetVisitor);
        var fileSet = ExecuteStatement(orExpression.Statement, visitorSet);
        var fileSetNext = ExecuteExpr(orExpression.Next);
        return fileSet + fileSetNext;
    }

    private FileSet ExecuteAddingMatchVisitor(Match match)
    {
        var files = port.GetFilesMatchingRegex(match.Path, basePath);
        return new FileSet(files);
    }

    private FileSet ExecuteAddingIncludesVisitor(Includes includes)
    {
        var files = port.GetFilesIncludingRegexWithRangeDescriptor(includes.Path, includes.RangeDescriptor, basePath);
        return new FileSet(files);
    }

    private FileSet ExecuteAddingCountFileSetVisitor(CountFileSet countFileSet)
    {
        var fileSet = ExecuteExpr(countFileSet.Expression);
        var filesLength = fileSet.Length;
        return RangeDescriptorUtil.IsInRangeDescriptor(countFileSet.RangeDescriptor, filesLength) ? fileSet : FileSet.Empty();
    }

    private MatchVisitor ExecuteSubtractingMatchVisitor(FileSet fileSet)
    {
        return match => fileSet.RemoveWithPredicate(file => Regex.IsMatch(file.FileName, match.Path));
    }

    private IncludesVisitor ExecuteSubtractingIncludesVisitor(FileSet fileSet)
    {
        return includes =>
            fileSet.RemoveWithPredicate(file =>
                RangeDescriptorUtil.IsInRangeDescriptor(includes.RangeDescriptor,
                    Regex.Count(port.LoadFileContent(file.FileName), includes.Path)
                )
            );
    }

    private CountFileSetVisitor ExecuteSubtractingCountFileSetVisitor(FileSet fileSet)
    {
        return countFileSet =>
            RangeDescriptorUtil.IsInRangeDescriptor(countFileSet.RangeDescriptor, fileSet.Length) ? fileSet : FileSet.Empty();
    }
}

internal record VisitorSet(
    MatchVisitor MatchVisitor,
    IncludesVisitor IncludesVisitor,
    CountFileSetVisitor CountFileSetVisitor);

public class EvaluateQuery(EvaluateQuery.IPort port)
{
    public QueryResult Execute(IQueryAst queryAst, string basePath)
    {
        var executionContext = new ExecutionContext(port, basePath);

        var parsedExpression = executionContext.ExecuteExpr((IExpression)queryAst);
        return new QueryResult(parsedExpression.Files);
    }

    public interface IPort
    {
        List<FqueryFile> GetFilesMatchingRegex(string regex, string basePath);

        List<FqueryFile> GetFilesIncludingRegexWithRangeDescriptor(string regex, RangeDescriptor rangeDescriptor,
            string basePath);

        string LoadFileContent(string path);
    }
}