public class WorkItem
{
    private string _author;
    private string[] _tags;
    private string _module;
    private string _cweName;
    private string _description;
    private string _trace;
    private string _comment;
    private string _analizer;
    private string _hash;
    private string _cweId;
    private string _priority;

    public WorkItem(Entry entry, Defaults defaults)
    {
        _author = defaults.Author;
        _tags = defaults.Tags;
        _analizer = defaults.Analyzer;

        _module = entry.file_path.Split("/").FirstOrDefault(x => x.StartsWith(defaults.ModuleMask)) ?? "";

        _cweName = entry.cwe_name;
        _description = entry.description.Replace("\"", "\"\"");
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
        var tracelog = string.Join("<br><br>", traces.Select(x => $"<a href=\"{ReplaceFilePath(x.file_path, defaults.ReplaceTraceValues)}\">{x.file_path}</a><br>{x.line_num}:{string.Join("<br>", x.src_lines)}"));
        _trace = tracelog.Replace("\"", "\"\"");
        _comment = entry.comment;
        _hash = entry.hash;
        _cweId = entry.cwe_id;
        var priority = double.Parse(entry.priority);
        var priorityName = priority switch
        {
            <= 0.2 => "Низкий",
            <= 0.5 => "Средний",
            _ => "Критичный"
        };
        _priority = $"{priorityName}:{entry.priority}";
    }

    public override string ToString()
    {
        return @$",Bug,,""{_cweName}"",""{_author}"",New,""{string.Join(';', _tags)}"",""Статический анализатор выявил программную ошибку в коде модуля {_module}<br><br><b>Описание программной ошибки: </b><br>{_cweName}.<br>{_description}<br><br><b>Расположение: </b><br>{_trace}<br><br><b>Оценка от Security Champion: </b><br>{_comment}"",""Статический анализатор {_analizer}.<br>Хэш программной ошибки: {_hash}<br>Тип ошибки: CWE-{_cweId}<br>Уровень: <b>{_priority}</b><br>""";
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


public record TraceLine
{
    public string file_path;
    public string line_num;
    public List<string> src_lines;
}