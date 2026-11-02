using fquery.config;

namespace fquery.domain;

public class EvaluateRunConfig(ParseQuery parseQuery, EvaluateQuery evaluateQuery, GenerateOutput generateOutput)
{
    public void Execute(RunConfig runConfig)
    {
        var ast = parseQuery.Execute(runConfig.Query);
        var result = evaluateQuery.Execute(ast, runConfig.BasePath);
        generateOutput.Execute(result.Files, runConfig.Format);
    }
}