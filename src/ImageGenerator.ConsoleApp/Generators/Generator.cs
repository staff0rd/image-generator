using System;
using System.Collections.Generic;
using System.Linq;
using ImageGenerator.ConsoleApp;
using SixLabors.Fonts;
using SixLabors.ImageSharp;

public abstract class Generator<T> where T : DefaultOptions
{
    protected readonly T _o;
    protected readonly ImageCreator _imageCreator;

    public Generator(T options)
    {
        _o = options;
        var fonts = new FontCollection();
        var font = fonts.Install(_o.FontPath.Trim());
        _imageCreator = new ImageCreator(options.BackgroundImagePath, font);
    }

    public abstract void Generate(System.IO.DirectoryInfo outputDirectory);

    public int Generate()
    {
        var outputDirectory = System.IO.Directory.CreateDirectory(_o.OutputDirectory.Trim());

        Generate(outputDirectory);

        return 0;
    }

    protected static string getTagsText(Card card)
    {
        if (card.Layout != "devlog")
            return string.Join(" • ", card.Tags);
        else
        {
            var first = $"{card.Tags.First()} {card.Version}";
            return string.Join(" • ", new[] { first }.Concat(card.Tags.Skip(1)));
        }
    }

    protected static string SplitTextIntoTwoLines(string text, int lineLength = 20)
    {
        if (text.Length <= lineLength)
            return text;
        var middle = (int)(text.Length * .6);
        var splitAt = String.Concat(text.Take(middle)).LastIndexOf(' ');
        if (splitAt > -1)
            return text.ReplaceAtIndex(splitAt, '\n');
        return text;
    }
}