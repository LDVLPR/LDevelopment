using LDevelopment.Models;
using System.Collections.Generic;
using PagedList;

namespace LDevelopment.ViewModels
{
    public class BlogViewModel
    {
        public IPagedList<PostViewModel> Posts { get; set; }

        public ICollection<TagModel> Tags { get; set; }

        public SearchViewModel Search { get; set; }

        public int PageCount { get; set; }
    }
}