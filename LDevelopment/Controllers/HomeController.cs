using System.Linq;
using System.Web.Mvc;
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
                LastPosts = db.Posts
                    .Where(x => x.IsReleased)
                    .OrderByDescending(x => x.ReleaseDate)
                    .Take(3)
                    .Select(GetPostViewModel),

                PopularPosts = db.Posts
                    .Where(x => x.IsReleased)
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

        public ActionResult Error()
        {
            return View();
        }

        private static PostViewModel GetPostViewModel(PostModel postModel)
        {
            return new PostViewModel
            {
                Id = postModel.Id,
                Title = postModel.Title,
                Text = postModel.Text.Length > 200 ? postModel.Text.Substring(0, 200) + "..." : postModel.Text,
                ReleaseDate = postModel.ReleaseDate,
                ImageUrl = postModel.Image
            };
        }
    }
}