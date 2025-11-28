public class WorkItem
{
    public OutputData Output => _output;
    private OutputData _output;

    public WorkItem(Entry entry, Defaults defaults)
    {
        var traceLines = entry.trace.Split("\r\n");
        var traces = new List<TraceLine>();
        TraceLine? current = null;
        int i = 0;
        while (i < traceLines.Length)
        {
            var line = traceLines[i];
            if (line.StartsWith("file_path:"))
            {
                if (current != null)
                {
                    current.src_lines.RemoveAt(current.src_lines.Count - 1);
                    traces.Add(current);
                }
                current = new()
                {
                    file_path = line.Split(":").Last().Trim()
                };
            }
            else if (line.StartsWith("line_num:"))
            {
                current.line_num = line.Split(":").Last().Trim();
            }
            else if (line.StartsWith("src_lines:"))
            {
                current.src_lines = new();
            }
            else
            {
                current.src_lines.Add(line.Trim());
            }
            i++;
        }
        traces.Add(current);
        var file_path = traces.First().file_path;
        var tracelog = $"<a href=\"{ReplaceFilePath(file_path, defaults.ReplaceTraceValues)}\">{file_path}</a><br>{string.Join("<br>", traces.Select(x => $"{x.line_num}:{string.Join("<br>", x.src_lines)}"))}";
        var priority = double.Parse(entry.priority);
        var priorityName = priority switch
        {
            <= 0.2 => "Низкий",
            <= 0.5 => "Средний",
            _ => "Критичный"
        };

        _output = new()
        {
            Author = defaults.Author,
            Tags = defaults.Tags,
            Analizer = defaults.Analyzer,
            Status = entry.status,
            Module = entry.file_path.Split("/").FirstOrDefault(x => x.StartsWith(defaults.ModuleMask)) ?? "",
            CweName = entry.cwe_name,
            Description = entry.description.Replace("\"", "\"\""),
            Trace = tracelog.Replace("\"", "\"\""),
            Comment = entry.comment,
            Hash = entry.hash,
            CweId = entry.cwe_id,
            Priority = $"{priorityName}:{entry.priority}",
        };
    }

    private string ReplaceFilePath(string path, IEnumerable<ReplaceTraceValue> replaceTraceValues)
    {
        foreach (var replaceValue in replaceTraceValues)
        {
            if (path.StartsWith(replaceValue.Original))
            {
                return path.Replace(replaceValue.Original, replaceValue.ReplaceValue) + replaceValue.Suffix;
            }
        }
        return path;
    }
}
