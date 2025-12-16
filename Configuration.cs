using CommandLine;

public class Configuration
{
        [Option('i', "input",
                Required = true,
                HelpText = "Путь до входного файла.")]
        public required string InputPath { get; set; }

        [Option('o', "output",
                Required = true,
                HelpText = "Путь до выходного файла.")]
        public required string OutputPath { get; set; }

        [Option('g', "group",
                Default = "true",
                HelpText = "Должны ли срабатывания групироваться по cwe: true/false")]
        public string IsGroupConfiguration { get; set; } = "true";

        [Option('s', "statuses",
                Default = "new,confirmed",
                HelpText = "Статусы, которые будут выгружены")]
        public string RawProcessedSatuses { get; set; } = "new,confirmed";      

        public bool IsGroup => bool.Parse(IsGroupConfiguration);
        public string[] ProcessedSatuses => RawProcessedSatuses.Split(",");
}