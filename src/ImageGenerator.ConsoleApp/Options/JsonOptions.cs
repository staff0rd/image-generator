using CommandLine;

namespace ImageGenerator.ConsoleApp
{
    [Verb("json", HelpText = "Generate from JSON")]
    public class JsonOptions : DefaultOptions
    {
        [Option('p', Required = false, HelpText = "Path to JSON")]
        public string JsonPath { get; set; }
    }
}
