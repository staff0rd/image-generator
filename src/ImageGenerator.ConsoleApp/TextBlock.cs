using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;

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
