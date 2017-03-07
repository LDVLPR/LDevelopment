using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using LDevelopment.ActionResults;
using LDevelopment.Helpers;
using LDevelopment.Models;
using LDevelopment.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;
using WebGrease.Css.Extensions;

namespace LDevelopment.Controllers
{
    public class BlogController : BaseController
    {
        private readonly int _pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);

        public ActionResult Index(SearchViewModel searchViewModel, int pageNumber = 1)
        {
            var search = searchViewModel.SearchString;

            var posts = GetPosts(searchText: search);
            var tags = GetTags();

            var result = new BlogViewModel
            {
                Posts = posts.Select(GetPostViewModel).Select(BlogHelper.HasReadMoreLink).ToPagedList(pageNumber, _pageSize),
                Tags = BlogHelper.GetTagsList(tags),
                Search = new SearchViewModel
                {
                    SearchString = search,
                    SearchResult = !string.IsNullOrEmpty(search)
                }
            };

            return View(result);
        }

        public ActionResult Tag(string tag, int pageNumber = 1)
        {
            var id = int.Parse(tag);

            var posts = GetPosts().Where(x => x.PostTags.Any(t => t.Id == id));
            var tags = GetTags();

            var blogViewModel = new BlogViewModel
            {
                Posts = posts.Select(GetPostViewModel).Select(BlogHelper.HasReadMoreLink).ToPagedList(pageNumber, _pageSize),
                Tags = BlogHelper.GetTagsList(tags),
                Search = new SearchViewModel
                {
                    Tag = tag,
                    SearchResult = true
                }
            };

            return View("Index", blogViewModel);
        }

        public ActionResult Details(string url)
        {
            int id;

            var post = int.TryParse(url, out id)
                ? Repository.Find<PostModel>(id, x => x.PostTags, x => x.Comments)
                : Repository.Find<PostModel>(x => x.Url == url, x => x.PostTags, x => x.Comments);

            var readMore = ConfigurationManager.AppSettings["ReadMore"];

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text.Replace(readMore, string.Empty),
                ReleaseDate = post.ReleaseDate,
                Tags = BlogHelper.GetTagsList(post.PostTags),
                ImageUrl = post.Image,
                Comments = post.Comments.Select(c => new CommentViewModel
                {
                    PostId = post.Id,
                    Text = c.Text,
                    ReleaseDate = c.ReleaseDate,
                    AuthorName = c.Author.UserName,
                    AuthorMail = c.Author.Email
                })
                .ToList()
            };

            Repository.Context.Entry(post).Entity.ViewsCount++;
            Repository.Save();

            return View(postViewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Administrate(SearchViewModel searchViewModel, int pageNumber = 1)
        {
            var search = searchViewModel.SearchString;

            var posts = GetPosts(showAll: true, searchText: search);

            var blogViewModel = new BlogViewModel
            {
                Posts = posts.Select(GetPostViewModel).Select(BlogHelper.HasReadMoreLink).ToPagedList(pageNumber, _pageSize),
                Search = new SearchViewModel
                {
                    SearchString = search,
                    SearchResult = !string.IsNullOrEmpty(search)
                }
            };

            return View(blogViewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var tags = Repository.All<TagModel>();

            var postViewModel = new PostViewModel
            {
                Tags = BlogHelper.GetTagsList(tags)
            };

            return View(postViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var post = new PostModel
                {
                    Title = postViewModel.Title,
                    Text = postViewModel.Text,
                    ReleaseDate = DateTime.UtcNow,
                    IsReleased = postViewModel.IsReleased,
                    Url = BlogHelper.GeneratePostUrl(postViewModel.Title)
                };

                if (postViewModel.TagsIds != null)
                {
                    var tagsIds = postViewModel.TagsIds.Select(int.Parse).ToList();

                    foreach (var id in tagsIds)
                    {
                        var tag = Repository.Find<TagModel>(id);

                        post.PostTags.Add(tag);
                    }
                }

                if (postViewModel.Image != null && postViewModel.Image.ContentLength > 0)
                {
                    post.Image = AzureBlobHelper.UploadPhoto(postViewModel.Image);
                }

                Repository.Add(post);
                Repository.Save();

                return RedirectToAction("Details", new { url = post.Url });
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var post = Repository.Find<PostModel>(id, x => x.PostTags);

            var tags = Repository.All<TagModel>();

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text,
                ReleaseDate = post.ReleaseDate,
                IsReleased = post.IsReleased,
                TagsIds = post.PostTags.Select(x => x.Id.ToString()).ToList(),
                Tags = BlogHelper.GetTagsList(tags)
            };

            return View(postViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var post = Repository.Find<PostModel>(postViewModel.Id, x => x.PostTags);

                post.Title = postViewModel.Title;
                post.Text = postViewModel.Text;
                post.ReleaseDate = postViewModel.ReleaseDate;
                post.IsReleased = postViewModel.IsReleased;
                post.Url = BlogHelper.GeneratePostUrl(postViewModel.Title);

                var newTags = postViewModel.TagsIds.Select(int.Parse).ToList();
                var oldTags = post.PostTags.Select(x => x.Id).ToList();

                var tagsToAdd = newTags.Except(oldTags).ToList();
                var tagsToRemove = oldTags.Except(newTags).ToList();

                if (tagsToAdd.Any())
                {
                    foreach (var id in tagsToAdd)
                    {
                        var tag = Repository.Find<TagModel>(id);

                        post.PostTags.Add(tag);
                    }
                }

                if (tagsToRemove.Any())
                {
                    foreach (var id in tagsToRemove)
                    {
                        var tag = Repository.Find<TagModel>(id);

                        post.PostTags.Remove(tag);
                    }
                }

                if (postViewModel.Image != null && postViewModel.Image.ContentLength > 0)
                {
                    //var directory = "/Content/Posts/";
                    //var path = Path.Combine(Server.MapPath(directory), postViewModel.Image.FileName);
                    //var url = Path.Combine(directory, postViewModel.Image.FileName);

                    //postViewModel.Image.SaveAs(path);
                    //post.Image = url;

                    post.Image = AzureBlobHelper.UploadPhoto(postViewModel.Image);
                }

                Repository.Update(post);
                Repository.Save();

                return RedirectToAction("Details", new { url = post.Url });
            }

            return View(postViewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var post = Repository.Find<PostModel>(id);

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text,
                ReleaseDate = post.ReleaseDate,
                IsReleased = post.IsReleased
            };

            return View(postViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var post = Repository.Find<PostModel>(id, x => x.Comments);

            if (post.Comments.Any())
            {
                foreach (var comment in post.Comments)
                {
                    Repository.Delete<CommentModel>(comment.Id);
                }
            }

            Repository.Delete<PostModel>(post.Id);
            Repository.Save();

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Comment(int id)
        {
            var post = Repository.Find<PostModel>(id);

            var comment = new CommentViewModel
            {
                PostId = post.Id
            };

            return PartialView("Comment", comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Comment(CommentViewModel commentViewModel)
        {
            var post = Repository.Find<PostModel>(commentViewModel.PostId, x => x.Comments);

            if (ModelState.IsValid)
            {
                var username = HttpContext.User.Identity.GetUserId();

                var comment = new CommentModel()
                {
                    Text = commentViewModel.Text,
                    ReleaseDate = DateTime.UtcNow,
                    AuthorId = Repository.Context.Users.Find(username).Id,
                    Post = post
                };

                post.Comments.Add(comment);

                Repository.Save();
            }

            return RedirectToAction("Details", new { url = post.Url });
        }

        public ActionResult Feed()
        {
            if (Request.Url == null)
            {
                return null;
            }

            var items = new List<SyndicationItem>();

            Repository
                .All<PostModel>(x => x.IsReleased)
                .OrderByDescending(x => x.ReleaseDate)
                .ThenBy(x => x.Title)
                .Take(_pageSize)
                .ForEach(x => items.Add(new SyndicationItem(x.Title, x.Text, new Uri($"https://{Request.Url.Host}{Url.Action("Details")}?url={x.Url}"), x.Url, x.ReleaseDate)));

            var feed = new SyndicationFeed(Resources.Resources.SiteName, Resources.Resources.SiteDescription, new Uri($"https://{Request.Url.Host}"), items);

            return new FeedResult(feed);
        }

        private static PostViewModel GetPostViewModel(PostModel postModel)
        {
            return new PostViewModel
            {
                Id = postModel.Id,
                Title = postModel.Title,
                Text = postModel.Text,
                ReleaseDate = postModel.ReleaseDate,
                Tags = BlogHelper.GetTagsList(postModel.PostTags),
                ImageUrl = postModel.Image,
                Url = postModel.Url,
                Comments = postModel.Comments.Select(c => new CommentViewModel()).ToList()
            };
        }

        private IEnumerable<PostModel> GetPosts(bool showAll = false, string searchText = "")
        {
            var text = string.IsNullOrWhiteSpace(searchText) ? null : searchText.ToLower();

            var posts = Repository
                .All<PostModel>(x => x.IsReleased || showAll)
                .Where(x => text == null || x.Title.ToLower().Contains(text) || x.Text.ToLower().Contains(text))
                .OrderByDescending(x => x.ReleaseDate)
                .ThenBy(x => x.Title)
                .Include(x => x.PostTags)
                .Include(x => x.Comments)
                .ToList();

            return posts;
        }

        private IEnumerable<TagModel> GetTags()
        {
            var tags = Repository
                .All<TagModel>()
                .Where(x => x.TagPosts.Any(p => p.IsReleased))
                .OrderByDescending(x => x.TagPosts.Count)
                .ThenBy(x => x.Title)
                .ToList();

            return tags;
        }
    }
}
