using CommandLine;

namespace ImageGenerator.ConsoleApp
{
    [Verb("posts", HelpText = "Generate from posts")]
    public class PostOptions : DefaultOptions
    {
        [Option('p', Required = false, HelpText = "Path to posts")]
        public string PostsDirectory { get; set; }

        [Option('l', "layout", Required = false, HelpText = "Layout type (devlog)")]
        public string Layout { get; set; }
    }
}
