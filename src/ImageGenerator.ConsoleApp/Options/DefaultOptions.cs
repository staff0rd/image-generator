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
        [Option('s', Required = false, HelpText = "Path to square background image")]
        public string SquareBackgroundImagePath { get; set; }
        [Option("force", Required = false, Default = false, HelpText = "Force overwrite existing files")]
        public bool Force { get; set; }
    }
}
