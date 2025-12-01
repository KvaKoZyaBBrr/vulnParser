using System.Globalization;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

public static class Parser
{
    public const string OutputBugFormat = "," + //ID
    "Bug," + //Work Item Type
    "," + //Title 1
    "\"{0}\"," + //Title 2
    "\"{1}\"," + //Assigned To
    "New," + //State
    "\"{2}\"," + //Tags
    "\"Статический анализатор выявил программную ошибку в коде модуля {3}<br><br><b>Описание программной ошибки: </b><br>{4}.<br>{5}<br><br><b>Расположение: </b><br>{6}<br><br><b>Оценка от Security Champion: </b><br>{7}\"," + //Repro Steps
    "\"Статический анализатор {8}.<br>Хэш программной ошибки: {9}<br>Тип ошибки: CWE-{10}<br>Уровень: <b>{11}</b><br>\""; //System Info
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
        var bugs = new List<WorkItem>();
        foreach (var entry in data.Entries)
        {
            bugs.Add(new WorkItem(entry, defaults!));
        }

        if (data.Configuration.IsGroup)
        {
            foreach (var bugCweIdGroup in bugs.Where(x => x.Output.Status == "new").GroupBy(x => x.Output.CweId))
            {
                foreach (var bugModuleGroup in bugCweIdGroup.GroupBy(x => x.Output.Module))
                {
                     builder.AppendLine(
                        string.Format(
                            OutputBugFormat,
                            bugModuleGroup.First().Output.CweName,
                            defaults!.Author,
                            string.Join(";", defaults!.Tags),
                            bugModuleGroup.First().Output.Module,
                            bugModuleGroup.First().Output.CweName,
                            bugModuleGroup.First().Output.Description,
                            string.Join("<hr>", bugModuleGroup.Select((x,i)=>$"{i}. {x.Output.Trace}")),
                            string.Join("<hr>", bugModuleGroup.Select((x,i)=>$"{i}. {x.Output.Comment}")),
                            bugModuleGroup.First().Output.Analizer,
                            string.Join("<hr>", bugModuleGroup.Select((x,i)=>$"{i}. {x.Output.Hash}")),
                            bugModuleGroup.First().Output.CweId,
                            bugModuleGroup.First().Output.Priority));
                }
            }
        }
        else
        {
            foreach (var bug in bugs.Where(x => x.Output.Status == "new"))
            {
                builder.AppendLine(
                    string.Format(
                        OutputBugFormat,
                        bug.Output.CweName,
                        bug.Output.Author,
                        bug.Output.Tags,
                        bug.Output.Module,
                        bug.Output.CweName,
                        bug.Output.Description,
                        bug.Output.Trace,
                        bug.Output.Comment,
                        bug.Output.Analizer,
                        bug.Output.Hash,
                        bug.Output.CweId,
                        bug.Output.Priority));
            }
        }

        File.WriteAllText(data.Configuration.OutputPath, builder.ToString());
        return true;
    }

}