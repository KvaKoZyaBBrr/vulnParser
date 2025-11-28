using CommandLine;

public class Configuration
{
        [Option('i', "input",
                Required = true,
                HelpText = "Input file.")]
        public required string InputPath { get; set; }

        [Option('o', "output",
                Required = true,
                HelpText = "Output file.")]
        public required string OutputPath { get; set; }

        [Option('g', "group",
                HelpText = "Must be group by cwe type.")]
        public string IsGroupConfiguration { get; set; } = "true";

        public bool IsGroup => bool.Parse(IsGroupConfiguration);
}