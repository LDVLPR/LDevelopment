using System.Text.RegularExpressions;

namespace LDevelopment.Helpers
{
    public class BlogHelper
    {
        public static string GeneratePostUrl(string title)
        {
            var result = title.Trim();

            var specialCharacters = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            result = specialCharacters.Replace(result, string.Empty);

            var whitespaces = new Regex("[ ]{1,}", RegexOptions.None);
            result = whitespaces.Replace(result, "-");

            return result.ToLower();
        }
    }
}