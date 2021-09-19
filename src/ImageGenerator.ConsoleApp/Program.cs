using System;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using CommandLine;

namespace ImageGenerator.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                var cardCreator = new CardCreator();
                var cards = cardCreator.GetFromPosts(o.PostsDirectory.Trim());

                var textColor = Color.FromRgb(17, 17, 17);
                var fonts = new FontCollection();
                var font = fonts.Install(o.FontPath.Trim());

                var outputDirectory = System.IO.Directory.CreateDirectory(o.OutputDirectory.Trim());

                foreach (var card in cards)
                {
                    string tagsText = getTagsText(card);
                    var outputFileName = System.IO.Path.Combine(outputDirectory.FullName, card.FileName);

                    try
                    {
                        var titleText = SplitTextIntoTwoLines(card.Title);
                        var bottomText = $"staffordwilliams.com • {card.Date:yyyy-MM-dd}";
                        cardCreator.RenderAndWrite(o.BackgroundImagePath.Trim(), tagsText, outputFileName, titleText, textColor, font, bottomText, card.Layout == "devlog" ? "devlog" : null);
                        Console.WriteLine($"Built card for {card.Title}: {outputFileName}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Could not build card for {card.Title}\n\t{e.Message}");
                    }
                }
            });

        }

        private static string getTagsText(Card card)
        {
            if (card.Layout != "devlog")
                return string.Join(" • ", card.Tags);
            else
            {
                var first = $"{card.Tags.First()} {card.Version}";
                return string.Join(" • ", new[] { first }.Concat(card.Tags.Skip(1)));
            }
        }

        private static string SplitTextIntoTwoLines(string text)
        {
            if (text.Length <= 20)
                return text;
            var middle = (int)(text.Length * .6);
            var splitAt = String.Concat(text.Take(middle)).LastIndexOf(' ');
            if (splitAt > -1)
                return text.ReplaceAtIndex(splitAt, '\n');
            return text;
        }
    }
}
