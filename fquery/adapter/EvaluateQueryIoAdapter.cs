using System.Text.RegularExpressions;
using fquery.domain;
using fquery.model;

namespace fquery.adapter;

public class EvaluateQueryIoAdapter : EvaluateQuery.IPort
{
    public List<FqueryFile> GetFilesMatchingRegex(string regex, string basePath)
    {
        return [.. Directory.GetFiles(basePath, regex).Select(path => new FqueryFile(path))];
    }

    // Peter this is just ... horrible for performance
    public List<FqueryFile> GetFilesIncludingRegexWithRangeDescriptor(string regex, RangeDescriptor rangeDescriptor,
        string basePath)
    {
        var files = Directory.GetFiles(basePath).Where(fileName => Regex.IsMatch(File.ReadAllText(fileName), regex))
            .Select(path => new FqueryFile(path));
        return [.. files];
    }

    public string LoadFileContent(string path)
    {
        return File.ReadAllText(path);
    }
}