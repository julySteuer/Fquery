namespace fquery.model;

public record QueryResult(List<FqueryFile> Files);

public record FqueryFile(string FileName);