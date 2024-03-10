using System;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;

public abstract class ImageCreator
{
    protected readonly string _backgroundFilePath;
    protected readonly Color _textColor;
    protected readonly FontFamily _font;
    public ImageCreator(string backgroundFilePath, FontFamily font)
    {
        _backgroundFilePath = backgroundFilePath.Trim();
        _textColor = Color.FromRgb(17, 17, 17);
        _font = font;
    }

    public abstract void RenderAndWrite(string outputFileName, string titleText, string bottomText, string tagsText = null, string subTitleText = null);

    protected IPathCollection GetGlyphs(string text, Size targetSize, FontFamily fontFamily, TextAlignment alignment = TextAlignment.Start)
    {
        Font font = new Font(fontFamily, 100); // size doesn't matter too much as we will be scaling shortly anyway
        TextOptions style = new TextOptions(font); // again dpi doesn't overlay matter as this code genreates a vector

        var originLeft = alignment == TextAlignment.Start ? -targetSize.Width / 2 : 0;

        style.Origin = new System.Numerics.Vector2(originLeft, 0);
        style.TextAlignment = alignment;
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

        Console.WriteLine($"{text}: {glyphs.Bounds.Width}");

        return glyphs;
    }
}