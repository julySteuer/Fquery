using fquery.model;

namespace fquery.domain;

public class GenerateOutput(GenerateOutput.IPort port)
{
    public void Execute(List<FqueryFile> fileSet, string format) // Not strictly Ports and adapters 
    {
        var parsedFormat = port.ParseFormat(format);
        
        port.PrintTable(BuildOutput(parsedFormat, fileSet));
    }

    private Table BuildOutput(List<IOutputFormat> formats, List<FqueryFile> files)
    {
        List<Row> rows = [.. files.Select(file => new Row([.. formats.Select(format => format.GetProperty(file))]))];
        return new Table(rows);
    }
    
    public interface IPort
    {
        List<IOutputFormat> ParseFormat(string format);
        void PrintTable(Table table);
    }
}