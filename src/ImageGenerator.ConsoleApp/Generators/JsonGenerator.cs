using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using ImageGenerator.ConsoleApp;

public class Site
{
    [JsonPropertyName("pages")]
    public Page[] Pages { get; set; }
}
public class Page
{
    [JsonPropertyName("layout")]
    public string Layout { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }

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
            .Where(p => p.Title != "404" && p.Title != "null" && p.Type != "note" && p.Url != "/" && !p.Url.StartsWith("/games"))
            .ToList()
            .ForEach(page =>
            {
                Console.WriteLine($"title: {page.Title}, type: {page.Type}, url: {page.Url}");
                try
                {

                    var outputFileName = System.IO.Path.Combine(outputDirectory.FullName, $"{HttpUtility.UrlEncode(page.Title)}.png");
                    var titleText = SplitTextIntoTwoLines(page.Title);
                    var bottomText = $"staffordwilliams.com";
                    _imageCreator.RenderAndWrite(outputFileName, titleText, bottomText);
                    Console.WriteLine($"Built {page.Title}");
                }
                catch
                {
                    Console.WriteLine($"Could not build {page.Title}");
                }
            });


    }
}