using fquery.domain;
using fquery.model;

namespace fquery.adapter;

internal class TablePrinter(Table table) {

    public void PrintTable()
    {
        table.Rows.ForEach(PrintRow);
    }

    private void PrintRow(Row row)
    {
        var lines = row.Entries.Aggregate("", (acc, curr) => acc + curr.Value + " ");
        Console.WriteLine(lines);
    }
}

internal class UnrecognizedOption(string name) : Exception($"Option with name: '{name}' was not recognized");

public class GenerateOutputIoAdapter : GenerateOutput.IPort
{
    private static readonly List<IOutputFormat> Formatters = [new NameFormat(), new PathFormat(), new SizeFormat()];

    public List<IOutputFormat> ParseFormat(string format)
    {
        var formatNames = format.Split(",").Select(RemoveWhitespaces);

        return [.. formatNames.Select(GetOutputFormatFromName)];
    }

    private string RemoveWhitespaces(string inp)
    {
        return inp.Where(c => !char.IsWhiteSpace(c)).Aggregate("", (frst, scnd) => frst + scnd);
    }

    private IOutputFormat GetOutputFormatFromName(string name)
    {
        var formatter = Formatters.Find(formatter => formatter.Name.Equals(name));

        return formatter ?? throw new UnrecognizedOption(name);
    }

    public void PrintTable(Table table)
    {
        new TablePrinter(table).PrintTable();
    }
}