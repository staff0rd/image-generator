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
    }
}
