using System;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using CommandLine;

namespace ImageGenerator.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<PostOptions, TextOptions>(args).MapResult(
                (PostOptions o) => new PostGenerator(o).Generate(),
                (TextOptions o) => new TextGenerator(o).Generate(),
                errs => 1);
        }
    }
}
