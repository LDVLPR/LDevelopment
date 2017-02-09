using System.Text.RegularExpressions;

namespace LDevelopment.Helpers
{
    public class BlogHelper
    {
        public static string GeneratePostUrl(string title)
        {
            var regex = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            var result = regex.Replace(title, string.Empty).ToLower().Trim().Replace(" ", "-");

            return result;
        }
    }
}