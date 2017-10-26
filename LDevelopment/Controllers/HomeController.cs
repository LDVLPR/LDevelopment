using System.Linq;
using System.Web.Mvc;
using LDevelopment.Helpers;
using LDevelopment.Models;
using LDevelopment.ViewModels;

namespace LDevelopment.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var result = new HomeViewModel
            {
                LastPosts = Repository
                    .All<Post>(x => x.IsReleased)
                    .OrderByDescending(x => x.ReleaseDate)
                    .Take(3)
                    .Select(GetPostViewModel),

                PopularPosts = Repository
                    .All<Post>(x => x.IsReleased)
                    .OrderByDescending(x => x.ViewsCount)
                    .Take(3)
                    .Select(GetPostViewModel)
            };

            return View(result);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        private static PostViewModel GetPostViewModel(Post post)
        {
            return new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Text = BlogHelper.GetFirstParagraph(post.Text),
                ReleaseDate = post.ReleaseDate,
                ImageUrl = post.Image,
                Url = post.Url
            };
        }
    }
}