using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LDevelopment.ViewModels;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using LDevelopment.Models;

namespace LDevelopment.Helpers
{
    public class BlogHelper
    {
        public static PostViewModel HasReadMoreLink(PostViewModel postViewModel)
        {
            var readMore = ConfigurationManager.AppSettings["ReadMore"];

            if (postViewModel.Text.Contains(readMore))
            {
                postViewModel.Text = postViewModel.Text.Substring(0, postViewModel.Text.IndexOf(readMore, StringComparison.Ordinal));
                postViewModel.HasReadMoreLink = true;
            }

            return postViewModel;
        }

        public static string GetFirstParagraph(string text)
        {
            var paragraph = new Regex(@"<p>\s*(.+?)\s*</p>");
            var result = paragraph.Match(text);

            return result.Success ? result.Groups[1].Value : text;
        }

        public static string GeneratePostUrl(string title)
        {
            var result = title.Trim();

            var specialCharacters = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            result = specialCharacters.Replace(result, string.Empty);

            var whitespaces = new Regex("[ ]{1,}", RegexOptions.None);
            result = whitespaces.Replace(result, "-");

            return result.ToLower();
        }

        public static MultiSelectList GetTagsList(IEnumerable<TagModel> tags)
        {
            var list = tags
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Title
                })
                .ToList();

            return new MultiSelectList(list, "Value", "Text");
        }
    }
}