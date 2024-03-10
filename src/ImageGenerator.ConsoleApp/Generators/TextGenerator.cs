using System;
using System.Collections.Generic;
using System.IO;
using ImageGenerator.ConsoleApp;

public class TextGenerator : Generator<TextOptions>
{
    public TextGenerator(TextOptions options) : base(options) { }

    public override void Generate(DirectoryInfo outputDirectory)
    {
        var outputFileName = System.IO.Path.Combine(outputDirectory.FullName, _o.OutputFileName);
        try
        {
            var titleText = SplitTextIntoTwoLines(_o.Text.Trim());
            var bottomText = $"staffordwilliams.com";
            _rectangleImageCreator.RenderAndWrite(outputFileName, titleText, bottomText);
            Console.WriteLine($"Built");
        }
        catch
        {
            Console.WriteLine($"Could not build");
        }

    }
}