using System.Text.RegularExpressions;

namespace LDevelopment.Helpers
{
    public class BlogHelper
    {
        public static string GeneratePostUrl(string title)
        {
            var result = Regex.Replace(title, "[;\\/:*?\"<>|&']", string.Empty);

            return result.Replace(" ", "-"); 
        }
    }
}