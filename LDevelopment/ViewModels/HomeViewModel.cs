using System.Collections.Generic;

namespace LDevelopment.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<PostViewModel> LastPosts { get; set; }
        public IEnumerable<PostViewModel> PopularPosts { get; set; }
    }
}