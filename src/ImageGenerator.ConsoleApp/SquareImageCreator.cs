using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

public class SquareImageCreator : ImageCreator
{
    public SquareImageCreator(string backgroundFilePath, FontFamily font) : base(backgroundFilePath, font)
    {
    }

    public override void RenderAndWrite(string outputFileName, string titleText, string bottomText, string tagsText = null, string subTitleText = null)
    {
        var centerY = 1000;
        var ySpacing = 50;
        var xPosition = 0;
        var textBlocks = new List<TextBlock>();
        var titleHeight = 500;
        var DRAW_LINE_GUIDES = false;
        var DRAW_GUIDES = false;

        // if (!string.IsNullOrWhiteSpace(subTitleText))
        //     textBlocks.Add(new TextBlock(GetGlyphs(subTitleText, new Size(2340, 100), _font), subTitleText, _textColor));

        textBlocks.Add(new TextBlock(GetGlyphs(titleText, new Size(1800, titleHeight), _font, TextAlignment.Center), titleText, _textColor));

        textBlocks.Add(new TextBlock(GetGlyphs(bottomText, new Size(1800, 75), _font, TextAlignment.Center), bottomText, Color.FromRgb(200, 200, 200)));

        var totalHeight = textBlocks.Sum(line => line.Glyphs.Bounds.Height) + ((textBlocks.Count - 1) * ySpacing);
        var startY = centerY - (totalHeight / 2) + 500;

        if (DRAW_LINE_GUIDES)
            Console.WriteLine($"Total height: {totalHeight}, start: {startY}");

        for (int i = 0; i < textBlocks.Count; i++)
        {
            var previousY = i == 0 ? startY : textBlocks.ElementAt(i - 1).Glyphs.Bounds.Bottom;
            var nextY = previousY + (i == 0 ? 0 : ySpacing);

            if (DRAW_LINE_GUIDES)
                Console.WriteLine($"previous: {previousY}, next: {nextY}, height: {textBlocks.ElementAt(i).Glyphs.Bounds.Height}");

            textBlocks.ElementAt(i).Glyphs = textBlocks.ElementAt(i).Glyphs.Translate(xPosition, nextY);
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
                        new PointF(100, centerY),
                        new PointF(3900, centerY));
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
}