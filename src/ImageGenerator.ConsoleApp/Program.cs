using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace ImageGenerator.ConsoleApp
{
    class Program
    {
        public enum Placement
        {
            Top,
            Bottom
        }

        public class Card
        {
            public string Title { get; set; }
            public string[] Tags { get; set; }

            public string FileName { get; set; }
            public DateTimeOffset Date { get; set; }
        }

        static IEnumerable<Card> GetCards(string pathToPosts)
        {
            foreach (var file in Directory.GetFiles(pathToPosts, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".md") || s.EndsWith(".html")))
            {
                var lines = File.ReadAllLines(file);
                var tagsLine = lines.FirstOrDefault(line => line.StartsWith("tags"));
                if (tagsLine == null)
                    continue;
                var title = lines.FirstOrDefault(line => line.StartsWith("title:"))
                    .Replace("title:", "")
                    .Replace("\"", "")
                    .Trim();
                var dateText = lines.FirstOrDefault(line => line.StartsWith("date:"))
                    .Replace("date: ", "");

                tagsLine = tagsLine
                    .Replace("tags:", "")
                    .Replace("[", "")
                    .Replace("]", "");
                yield return new Card { 
                    Title = title, 
                    Tags = tagsLine.Split(',').Select(tag => tag.Trim()).ToArray(),
                    FileName = file
                        .Replace(pathToPosts, "")
                        .Replace("/", "")
                        .Replace(".html", ".png")
                        .Replace(".md", ".png"),
                    Date = DateTimeOffset.Parse(dateText),
                };
            }
        }

        static void Main(string[] args)
        {
            

            var cards = GetCards(args[0]).ToArray();

            var dir = System.IO.Directory.CreateDirectory(args[3]);
            var inputFile = args[1];
            var textColor = Color.FromRgb(17, 17, 17);
            var fonts = new FontCollection();
            var font = fonts.Install(args[2]);

            foreach (var card in cards)
            {
                var tagsText = string.Join(" • ", card.Tags);
                var outputFileName = System.IO.Path.Combine(dir.FullName, card.FileName);

                try 
                {
                    var titleText = SplitIntoTwo(card.Title);
                    var bottomText = $"staffordwilliams.com • {card.Date:yyyy-MM-dd}";
                    BuildCard(inputFile, tagsText, outputFileName, titleText, textColor, font, bottomText);
                    Console.WriteLine($"Built card for {card.Title}: {outputFileName}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not build card for {card.Title}\n\t{e.Message}");
                }
            }
        }

        private static void BuildCard(string inputFile, string tagsText, string outputFileName, string titleText, Color textColor, FontFamily font, string bottomText)
        {
            var title = GetGlyphs(titleText, new Size(2340, 500), font);
            title = title.Translate(1260, 1168 - title.Bounds.Height);

            var tags = GetGlyphs(tagsText, new Size(2340, 75), font);
            tags = tags.Translate(1260, 1168 + 50);

            var site = GetGlyphs(bottomText, new Size(2340, 75), font);
            site = site.Translate(1260, 1168 + 200);
            var siteColor = Color.FromRgb(200, 200, 200);

            using (Image img = Image.Load(inputFile))
            {
                img.Mutate(i =>
                {
                    i.Fill(Brushes.Solid(textColor), title);
                    i.Fill(Brushes.Solid(textColor), tags);
                    i.Fill(Brushes.Solid(siteColor), site);
                });

                img.Save(outputFileName);
            }
        }

        private static IPathCollection GetGlyphs(string text, Size targetSize, FontFamily fontFamily)
        {
            Font font = new Font(fontFamily, 100); // size doesn't matter too much as we will be scaling shortly anyway
            RendererOptions style = new RendererOptions(font, 72); // again dpi doesn't overlay matter as this code genreates a vector
            style.Origin = new System.Numerics.Vector2(targetSize.Width/2,0);
            // this is the important line, where we render the glyphs to a vector instead of directly to the image
            // this allows further vector manipulation (scaling, translating) etc without the expensive pixel operations.
            IPathCollection glyphs = TextBuilder.GenerateGlyphs(text, style);

            var widthScale = (targetSize.Width / glyphs.Bounds.Width);
            var heightScale = (targetSize.Height / glyphs.Bounds.Height);
            var minScale = Math.Min(widthScale, heightScale);

            // scale so that it will fit exactly in image shape once rendered
            glyphs = glyphs.Scale(minScale);

            // move the vectorised glyph so that it touchs top and left edges 
            // could be tweeked to center horizontaly & vertically here
            glyphs = glyphs.Translate(-glyphs.Bounds.Location);
            
            //glyphs = glyphs.Translate(0, targetSize.Height * 3);
            return glyphs;
        }

        private static string SplitIntoTwo(string text)
        {
            var middle = (int)(text.Length *.6);
            var splitAt = String.Concat(text.Take(middle)).LastIndexOf(' ');
            return text.ReplaceAtIndex(splitAt, '\n');
        }
    }

    public static class StringExtensions {
        public static string ReplaceAtIndex(this string text, int index, char c)
        {
            var stringBuilder = new StringBuilder(text);
            stringBuilder[index] = c;
            return stringBuilder.ToString();
        }
    }
}
