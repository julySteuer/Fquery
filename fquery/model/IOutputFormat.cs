namespace fquery.model;

public interface IOutputFormat
{
    string Name
    {
        get;
    }

    Entry GetProperty(FqueryFile file);
}

public record SizeFormat : IOutputFormat
{
    public string Name => "Size";
    public Entry GetProperty(FqueryFile file)
    {
        var sizeInBytes = new FileInfo(file.FileName).Length;

        return new Entry(sizeInBytes.ToString());
    }
}

public record NameFormat : IOutputFormat
{
    public string Name => "Name";
    public Entry GetProperty(FqueryFile file)
    {
        return new Entry(Path.GetFileName(file.FileName));
    }
}

public record PathFormat : IOutputFormat
{
    public string Name => "Path";
    public Entry GetProperty(FqueryFile file)
    {
        return new Entry(file.FileName);
    }
}