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
            return Parser.Default.ParseArguments<PostOptions, TextOptions, JsonOptions>(args).MapResult(
                (PostOptions o) => new PostGenerator(o).Generate(),
                (TextOptions o) => new TextGenerator(o).Generate(),
                (JsonOptions o) => new JsonGenerator(o).Generate(),
                errs => 1);
        }
    }
}
