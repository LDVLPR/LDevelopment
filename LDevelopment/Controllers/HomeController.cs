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
                    .All<PostModel>(x => x.IsReleased)
                    .OrderByDescending(x => x.ReleaseDate)
                    .Take(3)
                    .Select(GetPostViewModel),

                PopularPosts = Repository
                    .All<PostModel>(x => x.IsReleased)
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

        private static PostViewModel GetPostViewModel(PostModel postModel)
        {
            return new PostViewModel
            {
                Id = postModel.Id,
                Title = postModel.Title,
                Text = BlogHelper.GetFirstParagraph(postModel.Text),
                ReleaseDate = postModel.ReleaseDate,
                ImageUrl = postModel.Image,
                Url = postModel.Url
            };
        }
    }
}