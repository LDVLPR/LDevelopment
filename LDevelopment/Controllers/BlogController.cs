using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LDevelopment.Helpers;
using LDevelopment.Models;
using LDevelopment.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;

namespace LDevelopment.Controllers
{
    public class BlogController : BaseController
    {
        private readonly int _pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);

        public ActionResult Index(SearchViewModel searchViewModel, int pageNumber = 1)
        {
            var search = searchViewModel.SearchString;

            var posts = GetPosts()
                .Where(x => search == null || x.Title.ToLower().Contains(search.ToLower()) || x.Text.ToLower().Contains(search.ToLower()));

            var result = new BlogViewModel
            {
                Posts = posts.Select(GetPostViewModel).Select(HasReadMoreLink).ToPagedList(pageNumber, _pageSize),
                Tags = GetTags().ToList(),
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

            var posts = GetPosts()
                .Where(x => x.PostTags.Any(t => t.Id == id));

            var blogViewModel = new BlogViewModel
            {
                Posts = posts.Select(GetPostViewModel).Select(HasReadMoreLink).ToPagedList(pageNumber, _pageSize),
                Tags = GetTags().ToList(),
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

            var post = int.TryParse(url, out id) ? 
                Repository.Find<PostModel>(id, x => x.PostTags, x => x.Comments) : 
                Repository.Find<PostModel>(x => x.Url == url, x => x.PostTags, x => x.Comments);

            var tags = new MultiSelectList(post.PostTags.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Title
            })
            .ToList());

            var readMore = ConfigurationManager.AppSettings["ReadMore"];

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text.Replace(readMore, ""),
                ReleaseDate = post.ReleaseDate,
                Tags = tags,
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

            foreach (var comment in post.Comments)
            {
                Repository.Context.Entry(comment).Reference(x => x.Author).Load();
            }

            Repository.Context.Entry(post).Entity.ViewsCount++;
            Repository.Save();

            return View(postViewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Administrate(SearchViewModel searchViewModel, int pageNumber = 1)
        {
            var search = searchViewModel.SearchString;

            var posts = GetPosts()
                .Where(x => search == null || x.Title.Contains(search) || x.Text.Contains(search));

            var blogViewModel = new BlogViewModel
            {
                Posts = posts.Select(GetPostViewModel).Select(HasReadMoreLink).ToPagedList(pageNumber, _pageSize),
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
            var tags = Repository.All<TagModel>()
                .Select(tag => new SelectListItem
                {
                    Value = tag.Id.ToString(),
                    Text = tag.Title
                })
                .ToList();

            var postViewModel = new PostViewModel
            {
                Tags = new MultiSelectList(tags.OrderBy(x => x.Text), "Value", "Text")
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
                    IsReleased = true,
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

        // GET: Blog/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var post = Repository.Find<PostModel>(id, x => x.PostTags);

            var tags = Repository.All<TagModel>()
                .Select(tag => new SelectListItem
                {
                    Value = tag.Id.ToString(),
                    Text = tag.Title
                })
                .ToList();

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text,
                ReleaseDate = post.ReleaseDate,
                IsReleased = post.IsReleased,
                TagsIds = post.PostTags.Select(x => x.Id.ToString()).ToList(),
                Tags = new MultiSelectList(tags.OrderBy(x => x.Text), "Value", "Text")
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

        // GET: Blog/Delete/5
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

        // POST: Blog/Delete/5
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
        public ActionResult Comment([Bind(Include = "Text, PostId")] CommentViewModel commentViewModel)
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

        private static PostViewModel GetPostViewModel(PostModel postModel)
        {
            var tags = new MultiSelectList(postModel.PostTags.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Title
            })
            .ToList());

            return new PostViewModel
            {
                Id = postModel.Id,
                Title = postModel.Title,
                Text = postModel.Text,
                ReleaseDate = postModel.ReleaseDate,
                Tags = tags,
                ImageUrl = postModel.Image,
                Url = postModel.Url,
                Comments = postModel.Comments.Select(c => new CommentViewModel()).ToList()
            };
        }

        private static PostViewModel HasReadMoreLink(PostViewModel postViewModel)
        {
            var readMore = ConfigurationManager.AppSettings["ReadMore"];

            if (postViewModel.Text.Contains(readMore))
            {
                var position = postViewModel.Text.IndexOf(readMore, StringComparison.Ordinal);

                postViewModel.Text = postViewModel.Text.Substring(0, position);
                postViewModel.HasReadMoreLink = true;
            }

            return postViewModel;
        }

        private IEnumerable<PostModel> GetPosts()
        {
            var posts = Repository
                .All<PostModel>(x => x.IsReleased)
                .OrderByDescending(x => x.ReleaseDate)
                .ThenBy(x => x.Title)
                .Include(x => x.PostTags)
                .Include(x => x.Comments)
                .ToList();

            return posts;
        }

        private IEnumerable<TagModel> GetTags()
        {
            var posts = GetPosts();

            var tags = posts
                .SelectMany(x => x.PostTags)
                .Distinct()
                .OrderByDescending(t => t.TagPosts.Count)
                .ThenBy(t => t.Title)
                .ToList();

            return tags;
        }
    }
}
