namespace fquery.model;

// There is no strict Colum ordering. Have to add one otherwise nothing is stopping the data from changing Ordering
public record Table(List<Row> Rows);

public record Row(List<Entry> Entries);

public record Entry(string Value);