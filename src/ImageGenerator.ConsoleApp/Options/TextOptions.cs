using CommandLine;

namespace ImageGenerator.ConsoleApp
{
    [Verb("text", HelpText = "Generate from posts")]
    public class TextOptions : DefaultOptions
    {
        [Option('t', Required = false, HelpText = "Text to render")]
        public string Text { get; set; }

        [Option('n', Required = false, HelpText = "Output file name")]
        public string OutputFileName { get; set; }
    }
}
