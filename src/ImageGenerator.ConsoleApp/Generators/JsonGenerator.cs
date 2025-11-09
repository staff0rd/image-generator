using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImageGenerator.ConsoleApp;

public class Site
{
    [JsonPropertyName("pages")]
    public Page[] Pages { get; set; }
}
public class Page
{
    [JsonPropertyName("card")]
    public bool Card { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ConvertTitle { get; set; }

    [JsonIgnore]
    public string Title
    {
        get
        {
            return ConvertTitle?["title"].GetRawText().Trim('"');
        }
    }

    [JsonIgnore]
    public string Type
    {
        get
        {
            return ConvertTitle?.ContainsKey("type") == true
                ? ConvertTitle["type"].GetRawText().Trim('"')
                : null;
        }
    }

    [JsonIgnore]
    public string Layout
    {
        get
        {
            return ConvertTitle?.ContainsKey("layout") == true
                ? ConvertTitle["layout"].GetRawText().Trim('"')
                : null;
        }
    }
}

public class JsonGenerator : Generator<JsonOptions>
{
    public JsonGenerator(JsonOptions options) : base(options) { }

    public override void Generate(DirectoryInfo outputDirectory)
    {
        var path = _o.JsonPath.Trim();
        var json = File.ReadAllText(path);
        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var site = JsonSerializer.Deserialize<Site>(json, options);
        int skippedCount = 0;
        var pages = site.Pages.Where(p => p.Card);

        // Filter by type if specified
        if (!string.IsNullOrEmpty(_o.Type))
        {
            pages = pages.Where(p => p.Type == _o.Type);
        }

        pages.ToList()
            .ForEach(page =>
            {
                var outputFileName = System.IO.Path.Combine(outputDirectory.FullName, $"{page.Title}.png");

                // Check if file exists and skip if force is false
                if (!_o.Force && System.IO.File.Exists(outputFileName))
                {
                    skippedCount++;
                    return;
                }

                try
                {
                    var titleText = SplitTextIntoTwoLines(page.Title);
                    var bottomText = $"staffordwilliams.com";
                    var subTitleText = page.Layout == "note" ? "notes on" : null;
                    _rectangleImageCreator.RenderAndWrite(outputFileName, titleText, bottomText, null, subTitleText);
                    Console.WriteLine($"Built {page.Title} to {outputFileName}");
                }
                catch
                {
                    Console.WriteLine($"Could not build {page.Title}");
                }
            });

        if (skippedCount > 0)
        {
            Console.WriteLine($"Skipped {skippedCount} file{(skippedCount == 1 ? "" : "s")} that already exist (use --force to overwrite)");
        }
    }
}