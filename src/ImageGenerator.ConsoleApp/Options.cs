using CommandLine;

namespace ImageGenerator.ConsoleApp
{
    public class Options
    {
        [Option('p', Required = true, HelpText = "Path to posts")]
        public string PostsDirectory { get; set; }
        [Option('i', Required = true, HelpText = "Path to background image")]
        public string BackgroundImagePath { get; set; }
        [Option('f', Required = true, HelpText = "Path to font")]
        public string FontPath { get; set; }
        [Option('o', Required = true, HelpText = "Output directory")]
        public string OutputDirectory { get; set; }
    }
}
