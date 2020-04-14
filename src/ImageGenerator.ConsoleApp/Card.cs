using System;

namespace ImageGenerator.ConsoleApp
{
    public class Card
    {
        public string Title { get; set; }
        public string[] Tags { get; set; }
        public string FileName { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
