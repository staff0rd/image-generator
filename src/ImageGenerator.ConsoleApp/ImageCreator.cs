using System;
using System.Collections.Generic;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

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
        //var lines = new List<IPathCollection>();
        var titleHeight = subTitleText == null ? 500 : 375;
        var center = 1168;
        var title = GetGlyphs(titleText, new Size(2340, titleHeight), _font);
        if (subTitleText != null && title.Bounds.Height > 300)
            center += (int)(title.Bounds.Height - 300);
        title = title.Translate(1260, center - title.Bounds.Height);
        lines.Add(title);
        var tags = getTags(tagsText, center);

        var site = GetGlyphs(bottomText, new Size(2340, 75), _font);
        site = site.Translate(1260, center + 200);
        var siteColor = Color.FromRgb(200, 200, 200);

        var subTitle = GetGlyphs(subTitleText, new Size(2340, 100), _font);
        subTitle = subTitle.Translate(1260, center - title.Bounds.Height - subTitle.Bounds.Height - 50);

        using (Image img = Image.Load(_backgroundFilePath))
        {
            img.Mutate(i =>
            {
                i.Fill(Brushes.Solid(_textColor), subTitle);
                i.Fill(Brushes.Solid(_textColor), title);
                if (tags != null)
                    i.Fill(Brushes.Solid(_textColor), tags);
                i.Fill(Brushes.Solid(siteColor), site);
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