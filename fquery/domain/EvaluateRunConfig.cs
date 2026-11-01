using fquery.config;

namespace fquery.domain;

public class EvaluateRunConfig(ParseQuery parseQuery, EvaluateQuery evaluateQuery)
{
    public string Execute(RunConfig runConfig)
    {
        var ast = parseQuery.Execute(runConfig.query);
        var result = evaluateQuery.Execute(ast, runConfig.basePath);
        return result.ToString();
    }
}