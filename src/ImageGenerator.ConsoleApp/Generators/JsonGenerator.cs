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
        site.Pages
            .Where(p => p.Card)
            .ToList()
            .ForEach(page =>
            {
                try
                {
                    var outputFileName = System.IO.Path.Combine(outputDirectory.FullName, $"{page.Title}.png");
                    var titleText = SplitTextIntoTwoLines(page.Title);
                    var bottomText = $"staffordwilliams.com";
                    _rectangleImageCreator.RenderAndWrite(outputFileName, titleText, bottomText);
                    Console.WriteLine($"Built {page.Title} to {outputFileName}");
                }
                catch
                {
                    Console.WriteLine($"Could not build {page.Title}");
                }
            });


    }
}