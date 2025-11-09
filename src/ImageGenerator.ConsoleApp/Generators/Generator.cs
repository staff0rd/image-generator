using System;
using System.Linq;
using ImageGenerator.ConsoleApp;
using SixLabors.Fonts;
using SixLabors.ImageSharp;

public abstract class Generator<T> where T : DefaultOptions
{
    protected readonly T _o;
    protected readonly RectangleImageCreator _rectangleImageCreator;
    protected readonly SquareImageCreator _squareImageCreator;

    public Generator(T options)
    {
        _o = options;
        var fonts = new FontCollection();
        var font = fonts.Add(_o.FontPath.Trim());
        _rectangleImageCreator = new RectangleImageCreator(options.BackgroundImagePath, font);
        _squareImageCreator = !string.IsNullOrWhiteSpace(options.SquareBackgroundImagePath)
            ? new SquareImageCreator(options.SquareBackgroundImagePath, font)
            : null;
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

    protected static string SplitTextIntoTwoLines(string text)
    {
        if (text.Length <= 20)
            return text;
        return Word.Wrap(text, 30);
    }
}