using System;
using ImageGenerator.ConsoleApp;

public class PostGenerator : Generator<PostOptions>
{
    public PostGenerator(PostOptions options) : base(options) { }

    public override void Generate(System.IO.DirectoryInfo outputDirectory)
    {
        var cardCreator = new CardCreator();
        var cards = cardCreator.GetFromPosts(_o.PostsDirectory.Trim());

        foreach (var card in cards)
        {
            string tagsText = getTagsText(card);
            var outputFileName = System.IO.Path.Combine(outputDirectory.FullName, card.FileName);

            try
            {
                var titleText = SplitTextIntoTwoLines(card.Title);
                var bottomText = $"staffordwilliams.com • {card.Date:yyyy-MM-dd}";
                _imageCreator.RenderAndWrite(outputFileName, titleText, bottomText, tagsText, card.Layout == "devlog" ? "devlog" : null);
                Console.WriteLine($"Built card for {card.Title}: {outputFileName}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not build card for {card.Title}\n\t{e.Message}");
            }
        }
    }
}