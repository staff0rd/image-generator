using System;
using ImageGenerator.ConsoleApp;
using System.Linq;

public class PostGenerator : Generator<PostOptions>
{
    public PostGenerator(PostOptions options) : base(options) { }

    public override void Generate(System.IO.DirectoryInfo outputDirectory)
    {
        var cardCreator = new CardCreator();
        var cards = cardCreator.GetFromPosts(_o.PostsDirectory.Trim()).OrderBy(c => c.Date);
        int skippedCount = 0;

        foreach (var card in cards)
        {
            string tagsText = getTagsText(card);
            var rectangleOutputName = System.IO.Path.Combine(outputDirectory.FullName, card.FileName);
            var squareOutputName = System.IO.Path.Combine(outputDirectory.FullName, card.FileName.Replace(".png", "-square.png"));

            // Check if files exist and skip if force is false
            bool squareExists = _squareImageCreator != null && System.IO.File.Exists(squareOutputName);
            if (!_o.Force && System.IO.File.Exists(rectangleOutputName) && (_squareImageCreator == null || squareExists))
            {
                skippedCount++;
                continue;
            }

            try
            {
                var titleText = SplitTextIntoTwoLines(card.Title);
                var bottomText = $"staffordwilliams.com • {card.Date:yyyy-MM-dd}";
                _rectangleImageCreator.RenderAndWrite(rectangleOutputName, titleText, bottomText, tagsText, card.Layout == "devlog" ? "devlog" : null);

                if (_squareImageCreator != null)
                {
                    _squareImageCreator.RenderAndWrite(squareOutputName, titleText, bottomText, tagsText, card.Layout == "devlog" ? "devlog" : null);
                }

                Console.WriteLine($"Built card for {card.Title}: {rectangleOutputName}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not build card for {card.Title}\n\t{e.Message}");
            }
        }

        if (skippedCount > 0)
        {
            Console.WriteLine($"Skipped {skippedCount} file{(skippedCount == 1 ? "" : "s")} that already exist (use --force to overwrite)");
        }
    }
}