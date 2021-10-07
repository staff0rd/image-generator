using CommandLine;

namespace ImageGenerator.ConsoleApp
{
    [Verb("posts", HelpText = "Generate from posts")]
    public class PostOptions : DefaultOptions
    {
        [Option('p', Required = false, HelpText = "Path to posts")]
        public string PostsDirectory { get; set; }
    }
}
