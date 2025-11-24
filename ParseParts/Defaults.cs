public class Defaults
{
    public required string Author { get; set; }
    public required string[] Tags { get; set; }
    public required string Analyzer { get; set; }
    public required string ModuleMask { get; set; }
    public List<ReplaceTraceValue> ReplaceTraceValues { get; set; }

}