using CommandLine;

namespace ImageGenerator.ConsoleApp
{
    public class DefaultOptions
    {
        [Option('i', Required = true, HelpText = "Path to background image")]
        public string BackgroundImagePath { get; set; }
        [Option('f', Required = true, HelpText = "Path to font")]
        public string FontPath { get; set; }
        [Option('o', Required = true, HelpText = "Output directory")]
        public string OutputDirectory { get; set; }
        [Option('s', Required = true, HelpText = "Path to square background image")]
        public string SquareBackgroundImagePath { get; set; }
    }
}
