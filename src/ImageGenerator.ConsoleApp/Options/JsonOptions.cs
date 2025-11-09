using CommandLine;

namespace ImageGenerator.ConsoleApp
{
    [Verb("json", HelpText = "Generate from JSON")]
    public class JsonOptions : DefaultOptions
    {
        [Option('p', Required = false, HelpText = "Path to JSON")]
        public string JsonPath { get; set; }

        [Option('t', Required = false, HelpText = "Filter by type")]
        public string Type { get; set; }
    }
}
