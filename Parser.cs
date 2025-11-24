using System.Globalization;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

public static class Parser
{
    public static Data Read(Configuration configuration)
    {
        var data = new Data(configuration);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            HeaderValidated = null,
            Mode = CsvMode.RFC4180,
            TrimOptions = TrimOptions.Trim,
        };
        using (var reader = new StreamReader(configuration.InputPath))
        using (var csv = new CsvReader(reader, config))
        {
            data.Entries.AddRange(csv.GetRecords<Entry>());
        }
        return data;
    }

    public static bool Write(Data data)
    {
        var builder = new StringBuilder();
        builder.AppendLine("ID,Work Item Type,Title 1,Title 2,Assigned To,State,Tags,Repro Steps,System Info");
        var defaultTask = File.Exists("ParentWorkerItem.Development.txt") ? File.ReadAllLines("ParentWorkerItem.Development.txt") : File.ReadAllLines("ParentWorkerItem.txt");

        var defaultsRaw = File.Exists("Defaults.Development.json") ? File.ReadAllText("Defaults.Development.json") : File.ReadAllText("Defaults.json");
        var defaults = JsonSerializer.Deserialize<Defaults>(defaultsRaw);

        builder.AppendLine(defaultTask.First());

        foreach (var entry in data.Entries)
        {
            var bug = new WorkItem(entry, defaults!);
            if (bug.TryGetString(out string str))
            {
                builder.AppendLine(str);
            }
        }

        File.WriteAllText(data.Configuration.OutputPath, builder.ToString());
        return true;
    }

}