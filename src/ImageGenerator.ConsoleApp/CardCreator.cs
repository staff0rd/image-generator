using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace ImageGenerator.ConsoleApp
{
    public class CardCreator
    {
        private IEnumerable<string> GetPosts(string pathToPosts)
        {
            FileAttributes attr = File.GetAttributes(pathToPosts);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                return Directory
                    .GetFiles(pathToPosts, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".md") || s.EndsWith(".html"));
            }
            else
            {
                return new[] { pathToPosts };
            }
        }

        public IEnumerable<Card> GetFromPosts(string pathToPosts)
        {
            var posts = GetPosts(pathToPosts);

            foreach (var file in posts)
            {
                var lines = File.ReadAllLines(file);
                var tagsLine = lines.FirstOrDefault(line => line.StartsWith("tags"));

                // no tags, no render
                if (tagsLine == null)
                    continue;

                var title = lines.FirstOrDefault(line => line.StartsWith("title:"))
                    .Replace("title:", "")
                    .Replace("\"", "")
                    .Trim();
                var dateText = lines.FirstOrDefault(line => line.StartsWith("date:"))
                    .Replace("date: ", "")
                    .Trim('"');

                tagsLine = tagsLine
                    .Replace("tags:", "")
                    .Replace("[", "")
                    .Replace("]", "");

                var layout = lines.FirstOrDefault(line => line.StartsWith("layout:"))
                    .Replace("layout:", "")
                    .Trim()
                    .Trim('"');

                var version = lines.FirstOrDefault(line => line.StartsWith("version:"))
                    ?.Replace("version:", "")
                    ?.Trim()
                    ?.Trim('"');

                yield return new Card
                {
                    Title = title,
                    Tags = tagsLine.Split(',').Select(tag => tag.Trim()).ToArray(),
                    FileName = GenerateFileName(pathToPosts, file),
                    Date = DateTimeOffset.Parse(dateText),
                    Layout = layout,
                    Version = version
                };
            }
        }

        private static string GenerateFileName(string pathToPosts, string file)
        {
            var fileName = file;

            if (pathToPosts != file)
            {
                fileName = fileName
                    .Replace(pathToPosts, "")
                    .Replace("/", "");
            }

            fileName = fileName
                .Replace(".html", ".png")
                .Replace(".md", ".png");
            return fileName;
        }

        public void RenderAndWrite(string inputFile, string tagsText, string outputFileName, string titleText, Color textColor, FontFamily font, string bottomText, string subTitleText = null)
        {
            var titleHeight = subTitleText == null ? 500 : 375;
            var center = 1168;



            var title = GetGlyphs(titleText, new Size(2340, titleHeight), font);
            System.Console.WriteLine(title.Bounds.Height);
            if (title.Bounds.Height > 300)
                center += (int)(title.Bounds.Height - 300);
            title = title.Translate(1260, center - title.Bounds.Height);

            var tags = GetGlyphs(tagsText, new Size(2340, 75), font);
            tags = tags.Translate(1260, center + 50);

            var site = GetGlyphs(bottomText, new Size(2340, 75), font);
            site = site.Translate(1260, center + 200);
            var siteColor = Color.FromRgb(200, 200, 200);

            var subTitle = GetGlyphs(subTitleText, new Size(2340, 100), font);
            subTitle = subTitle.Translate(1260, center - title.Bounds.Height - subTitle.Bounds.Height - 50);

            using (Image img = Image.Load(inputFile))
            {
                img.Mutate(i =>
                {
                    i.Fill(Brushes.Solid(textColor), subTitle);
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
            style.Origin = new System.Numerics.Vector2(targetSize.Width / 2, 0);
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
    }
}
