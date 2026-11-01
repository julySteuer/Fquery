using System.CommandLine;
using fquery.config;

var basePath = new Option<string>("--basePath", "-b")
{
  Description = "Base path to start the search from"
};

var query = new Option<string>("--query", "-q")
{
  Description = "Query parameter in fqueryql"
};

var rootParser = new RootCommand("Utility to search file system")
{
  basePath,
  query
};

var result = rootParser.Parse(args);

if (result.Errors.Count != 0)
{
  foreach (var resultError in result.Errors)
  {
    Console.Error.WriteLine(resultError);
  }

  return 1;
}

if (result.GetValue(basePath) is { } directoryInfo && result.GetValue(query) is { } queryValue)
{
  var runConfig = new RunConfig(directoryInfo, queryValue);
  Console.WriteLine(ProgramConfig.RunWithDi(runConfig));
}


return 0;

