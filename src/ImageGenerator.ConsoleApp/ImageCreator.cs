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
    public TextBlock(IPathCollection glyphs, Color color)
    {
        Glyphs = glyphs;
        Color = color;
    }
    public IPathCollection Glyphs { get; set; }

    public Color Color { get; set; }
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
        var center = 1168;
        var padding = 100;
        var left = 1260;
        var lines = new List<TextBlock>();
        var titleHeight = subTitleText == null ? 500 : 375;

        if (!string.IsNullOrWhiteSpace(subTitleText))
            lines.Add(new TextBlock(GetGlyphs(subTitleText, new Size(2340, 100), _font), _textColor));

        lines.Add(new TextBlock(GetGlyphs(titleText, new Size(2340, titleHeight), _font), _textColor));

        if (!string.IsNullOrWhiteSpace(tagsText))
            lines.Add(new TextBlock(getTags(tagsText, center), _textColor));

        lines.Add(new TextBlock(GetGlyphs(bottomText, new Size(2340, 75), _font), Color.FromRgb(200, 200, 200)));

        var totalHeight = lines.Sum(line => line.Glyphs.Max(p => p.Bounds.Bottom)) + (lines.Count - 1) * padding;

        for (int i = 0; i < lines.Count; i++)
        {
            var previous = lines.Take(i).Sum(p => p.Glyphs.Max(p => p.Bounds.Bottom) + padding * i);
            lines.ElementAt(i).Glyphs = lines.ElementAt(i).Glyphs.Translate(left, center - totalHeight / 2 + previous);
        }

        using (Image img = Image.Load(_backgroundFilePath))
        {
            img.Mutate(i =>
            {
                lines.ForEach(line => i.Fill(Brushes.Solid(line.Color), line.Glyphs));
            });

            img.Save(outputFileName);
        }
    }

    private IPathCollection getTags(string tagsText, int center)
    {
        if (string.IsNullOrWhiteSpace(tagsText)) return null;
        var tags = GetGlyphs(tagsText, new Size(2340, 75), _font);
        tags = tags.Translate(1260, center + 50);
        return tags;
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