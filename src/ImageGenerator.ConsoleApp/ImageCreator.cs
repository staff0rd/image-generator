using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

public class TextBlock
{
    public TextBlock(IPathCollection glyphs, string text, Color color)
    {
        Glyphs = glyphs;
        Color = color;
        Text = text;
    }
    public IPathCollection Glyphs { get; set; }

    public Color Color { get; set; }

    public string Text { get; set; }
}
public class ImageCreator
{
    readonly string _backgroundFilePath;
    readonly Color _textColor;
    readonly FontFamily _font;
    public ImageCreator(string backgroundFilePath, FontFamily font)
    {
        _backgroundFilePath = backgroundFilePath.Trim();
        _textColor = Color.FromRgb(17, 17, 17);
        _font = font;
    }

    public void RenderAndWrite(string outputFileName, string titleText, string bottomText, string tagsText = null, string subTitleText = null)
    {
        var center = 1130;
        var padding = 100;
        var left = 1260;
        var lines = new List<TextBlock>();
        var titleHeight = 500;
        var DRAW_LINE_GUIDES = false;
        var DRAW_GUIDES = false;

        if (!string.IsNullOrWhiteSpace(subTitleText))
            lines.Add(new TextBlock(GetGlyphs(subTitleText, new Size(2340, 100), _font), subTitleText, _textColor));

        lines.Add(new TextBlock(GetGlyphs(titleText, new Size(2340, titleHeight), _font), titleText, _textColor));

        if (!string.IsNullOrWhiteSpace(tagsText))
            lines.Add(new TextBlock(GetGlyphs(tagsText, new Size(2340, 75), _font), tagsText, _textColor));

        lines.Add(new TextBlock(GetGlyphs(bottomText, new Size(2340, 75), _font), bottomText, Color.FromRgb(200, 200, 200)));

        var totalHeight = lines.Sum(line => line.Glyphs.Bounds.Height) + ((lines.Count - 1) * padding);
        var start = center - (totalHeight / 2);

        if (DRAW_LINE_GUIDES)
            Console.WriteLine($"Total height: {totalHeight}, start: {start}");

        for (int i = 0; i < lines.Count; i++)
        {
            var previous = i == 0 ? start : lines.ElementAt(i - 1).Glyphs.Bounds.Bottom;
            var next = previous + (i == 0 ? 0 : padding);

            if (DRAW_LINE_GUIDES)
                Console.WriteLine($"previous: {previous}, next: {next}, height: {lines.ElementAt(i).Glyphs.Bounds.Height}");

            lines.ElementAt(i).Glyphs = lines.ElementAt(i).Glyphs.Translate(left, next);
        }


        using (Image img = Image.Load(_backgroundFilePath))
        {
            img.Mutate(i =>
            {
                lines.ForEach(line =>
                {
                    i.Fill(Brushes.Solid(line.Color), line.Glyphs);

                    if (DRAW_LINE_GUIDES)
                    {
                        Console.WriteLine($"top: {line.Glyphs.Bounds.Top}, bottom: {line.Glyphs.Bounds.Bottom}, text: {line.Text}");
                        i.DrawLines(Brushes.Solid(Color.Black), 3f,
                            new PointF(100, line.Glyphs.Bounds.Top),
                            new PointF(3900, line.Glyphs.Bounds.Top));
                        i.DrawLines(Brushes.Solid(Color.Black), 3f,
                            new PointF(100, line.Glyphs.Bounds.Bottom),
                            new PointF(3900, line.Glyphs.Bounds.Bottom));
                    }

                });

                if (DRAW_GUIDES)
                {
                    i.DrawLines(Brushes.Solid(Color.Red), 5f,
                        new PointF(100, center),
                        new PointF(3900, center));
                    i.DrawLines(Brushes.Solid(Color.Red), 5f,
                        new PointF(100, start),
                        new PointF(3900, start));
                    i.DrawLines(Brushes.Solid(Color.Red), 5f,
                        new PointF(100, start + totalHeight),
                        new PointF(3900, start + totalHeight));
                }
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