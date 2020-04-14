using System.Text;

namespace ImageGenerator.ConsoleApp
{
    // thanks stackoverflow: https://stackoverflow.com/a/24499011/469777
    public static class StringExtensions
    {
        public static string ReplaceAtIndex(this string text, int index, char c)
        {
            var stringBuilder = new StringBuilder(text);
            stringBuilder[index] = c;
            return stringBuilder.ToString();
        }
    }
}
