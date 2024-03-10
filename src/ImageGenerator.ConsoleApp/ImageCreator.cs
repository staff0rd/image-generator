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
        var ySpacing = 100;
        var x = 1260;
        var textBlocks = new List<TextBlock>();
        var titleHeight = 500;
        var DRAW_LINE_GUIDES = false;
        var DRAW_GUIDES = false;

        if (!string.IsNullOrWhiteSpace(subTitleText))
            textBlocks.Add(new TextBlock(GetGlyphs(subTitleText, new Size(2340, 100), _font), subTitleText, _textColor));

        textBlocks.Add(new TextBlock(GetGlyphs(titleText, new Size(2340, titleHeight), _font), titleText, _textColor));

        if (!string.IsNullOrWhiteSpace(tagsText))
            textBlocks.Add(new TextBlock(GetGlyphs(tagsText, new Size(2340, 75), _font), tagsText, _textColor));

        textBlocks.Add(new TextBlock(GetGlyphs(bottomText, new Size(2340, 75), _font), bottomText, Color.FromRgb(200, 200, 200)));

        var totalHeight = textBlocks.Sum(line => line.Glyphs.Bounds.Height) + ((textBlocks.Count - 1) * ySpacing);
        var startY = center - (totalHeight / 2);

        if (DRAW_LINE_GUIDES)
            Console.WriteLine($"Total height: {totalHeight}, start: {startY}");

        for (int i = 0; i < textBlocks.Count; i++)
        {
            var previousY = i == 0 ? startY : textBlocks.ElementAt(i - 1).Glyphs.Bounds.Bottom;
            var nextY = previousY + (i == 0 ? 0 : ySpacing);

            if (DRAW_LINE_GUIDES)
                Console.WriteLine($"previous: {previousY}, next: {nextY}, height: {textBlocks.ElementAt(i).Glyphs.Bounds.Height}");

            textBlocks.ElementAt(i).Glyphs = textBlocks.ElementAt(i).Glyphs.Translate(x, nextY);
        }


        using (Image img = Image.Load(_backgroundFilePath))
        {
            img.Mutate(i =>
            {
                textBlocks.ForEach(tb =>
                {
                    i.Fill(Brushes.Solid(tb.Color), tb.Glyphs);

                    if (DRAW_LINE_GUIDES)
                    {
                        Console.WriteLine($"top: {tb.Glyphs.Bounds.Top}, bottom: {tb.Glyphs.Bounds.Bottom}, text: {tb.Text}");
                        i.DrawLine(Brushes.Solid(Color.Black), 3f,
                            new PointF(100, tb.Glyphs.Bounds.Top),
                            new PointF(3900, tb.Glyphs.Bounds.Top));
                        i.DrawLine(Brushes.Solid(Color.Black), 3f,
                            new PointF(100, tb.Glyphs.Bounds.Bottom),
                            new PointF(3900, tb.Glyphs.Bounds.Bottom));
                    }

                });

                if (DRAW_GUIDES)
                {
                    i.DrawLine(Brushes.Solid(Color.Red), 5f,
                        new PointF(100, center),
                        new PointF(3900, center));
                    i.DrawLine(Brushes.Solid(Color.Red), 5f,
                        new PointF(100, startY),
                        new PointF(3900, startY));
                    i.DrawLine(Brushes.Solid(Color.Red), 5f,
                        new PointF(100, startY + totalHeight),
                        new PointF(3900, startY + totalHeight));
                }
            });

            img.Save(outputFileName);
        }
    }

    private static IPathCollection GetGlyphs(string text, Size targetSize, FontFamily fontFamily)
    {
        Font font = new Font(fontFamily, 100); // size doesn't matter too much as we will be scaling shortly anyway
        TextOptions style = new TextOptions(font); // again dpi doesn't overlay matter as this code genreates a vector
        style.Origin = new System.Numerics.Vector2(-targetSize.Width / 2, 0);
        // this is the important line, where we render the glyphs to a vector instead of directly to the image
        // this allows further vector manipulation (scaling, translating) etc without the expensive pixel operations.
        IPathCollection glyphs = TextBuilder.GenerateGlyphs(text, style);

        var widthScale = (targetSize.Width / glyphs.Bounds.Width);
        var heightScale = (targetSize.Height / glyphs.Bounds.Height);
        var minScale = Math.Min(widthScale, heightScale);

        // // scale so that it will fit exactly in image shape once rendered
        glyphs = glyphs.Scale(minScale);

        // move the vectorised glyph so that it touchs top and left edges 
        // could be tweeked to center horizontaly & vertically here
        glyphs = glyphs.Translate(-glyphs.Bounds.Location);

        return glyphs;
    }
}