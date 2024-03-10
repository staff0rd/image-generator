using System;
using ImageGenerator.ConsoleApp;
using System.Linq;

public class PostGenerator : Generator<PostOptions>
{
    public PostGenerator(PostOptions options) : base(options) { }

    public override void Generate(System.IO.DirectoryInfo outputDirectory)
    {
        var cardCreator = new CardCreator();
        var cards = cardCreator.GetFromPosts(_o.PostsDirectory.Trim()).OrderBy(c => c.Date).Take(1);

        foreach (var card in cards)
        {
            string tagsText = getTagsText(card);
            var rectangleOutputName = System.IO.Path.Combine(outputDirectory.FullName, card.FileName);
            var squareOutputName = System.IO.Path.Combine(outputDirectory.FullName, card.FileName.Replace(".png", "-square.png"));

            try
            {
                var titleText = SplitTextIntoTwoLines(card.Title);
                var bottomText = $"staffordwilliams.com • {card.Date:yyyy-MM-dd}";
                _rectangleImageCreator.RenderAndWrite(rectangleOutputName, titleText, bottomText, tagsText, card.Layout == "devlog" ? "devlog" : null);
                _squareImageCreator.RenderAndWrite(squareOutputName, titleText, bottomText, tagsText, card.Layout == "devlog" ? "devlog" : null);
                Console.WriteLine($"Built card for {card.Title}: {rectangleOutputName}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not build card for {card.Title}\n\t{e.Message}");
            }
        }
    }
}