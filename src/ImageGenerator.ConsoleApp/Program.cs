using System;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;

namespace ImageGenerator.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var cardCreator = new CardCreator();
            var cards = cardCreator.GetFromPosts(args[0]);

            var backgroundImagePath = args[1];

            var textColor = Color.FromRgb(17, 17, 17);
            var fonts = new FontCollection();
            var font = fonts.Install(args[2]);
            
            var outputDirectory = System.IO.Directory.CreateDirectory(args[3]);

            foreach (var card in cards)
            {
                var tagsText = string.Join(" • ", card.Tags);
                var outputFileName = System.IO.Path.Combine(outputDirectory.FullName, card.FileName);

                try 
                {
                    var titleText = SplitTextIntoTwoLines(card.Title);
                    var bottomText = $"staffordwilliams.com • {card.Date:yyyy-MM-dd}";
                    cardCreator.RenderAndWrite(backgroundImagePath, tagsText, outputFileName, titleText, textColor, font, bottomText);
                    Console.WriteLine($"Built card for {card.Title}: {outputFileName}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not build card for {card.Title}\n\t{e.Message}");
                }
            }
        }

        private static string SplitTextIntoTwoLines(string text)
        {
            var middle = (int)(text.Length *.6);
            var splitAt = String.Concat(text.Take(middle)).LastIndexOf(' ');
            return text.ReplaceAtIndex(splitAt, '\n');
        }
    }
}
