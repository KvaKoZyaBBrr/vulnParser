public class Data(Configuration configuration)
{
    public Configuration Configuration => configuration;
    public List<Entry> Entries { get; } = new();
}

public record Entry
{
    public required string file_path { get; set; }
    public required string cwe_id { get; set; }
    public required string cwe_name { get; set; }
    public required string priority { get; set; }
    public required string hash { get; set; }
    public required string category { get; set; }
    public required string description { get; set; }
    public required string trace { get; set; }
    public required string status { get; set; }
    public required string comment { get; set; }
}