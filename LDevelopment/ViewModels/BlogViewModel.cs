using System.Web.Mvc;
using PagedList;

namespace LDevelopment.ViewModels
{
    public class BlogViewModel
    {
        public IPagedList<PostViewModel> Posts { get; set; }

        public MultiSelectList Tags { get; set; }

        public SearchViewModel Search { get; set; }

        public int PageCount { get; set; }
    }
}