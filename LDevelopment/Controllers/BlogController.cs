﻿using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
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
        readonly int _pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);

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

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var post = db.Posts
                .Include(x => x.PostTags)
                .Include(x => x.Comments)
                .SingleOrDefault(x => x.Id == id.Value);

            if (post == null)
            {
                return HttpNotFound();
            }

            var readMore = ConfigurationManager.AppSettings["ReadMore"];

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text.Replace(readMore, ""),
                ReleaseDate = post.ReleaseDate,
                ImageUrl = post.Image,
                Comments = post.Comments.Select(c => new CommentViewModel
                {
                    PostId = post.Id,
                    Text = c.Text,
                    ReleaseDate = c.ReleaseDate,
                    AuthorName = c.Author.UserName,
                    AuthorMail = c.Author.Email
                }).ToList()
            };

            foreach (var comment in post.Comments)
            {
                db.Entry(comment).Reference(x => x.Author).Load();
            }

            db.Entry(post).Entity.ViewsCount++;
            db.SaveChanges();

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
            var tags = db.Tags
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
                var postModel = new PostModel
                {
                    Title = postViewModel.Title,
                    Text = postViewModel.Text,
                    ReleaseDate = DateTime.UtcNow,
                    IsReleased = true
                };

                if (postViewModel.TagsIds != null)
                {
                    var tagsIds = postViewModel.TagsIds.Select(int.Parse).ToList();

                    foreach (var tagId in tagsIds)
                    {
                        var tag = db.Tags.SingleOrDefault(x => x.Id == tagId);

                        postModel.PostTags.Add(tag);
                    }
                }

                if (postViewModel.Image != null && postViewModel.Image.ContentLength > 0)
                {
                    postModel.Image = UploadPhoto(postViewModel.Image);
                }

                try
                {
                    db.Posts.Add(postModel);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }

                return RedirectToAction("Details", new { id = postModel.Id });
            }

            return RedirectToAction("Index");
        }

        // GET: Blog/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var post = db.Posts
                .Include(x => x.PostTags)
                .SingleOrDefault(x => x.Id == id);

            if (post == null)
            {
                return HttpNotFound();
            }

            var tags = db.Tags
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
                var postModel = db.Posts
                    .Include(x => x.PostTags)
                    .SingleOrDefault(x => x.Id == postViewModel.Id);

                if (postModel == null)
                {
                    return HttpNotFound();
                }

                postModel.Title = postViewModel.Title;
                postModel.Text = postViewModel.Text;
                postModel.ReleaseDate = postViewModel.ReleaseDate;
                postModel.IsReleased = postViewModel.IsReleased;

                var newTags = postViewModel.TagsIds.Select(int.Parse).ToList();
                var oldTags = postModel.PostTags.Select(x => x.Id).ToList();

                var tagsToAdd = newTags.Except(oldTags).ToList();
                var tagsToRemove = oldTags.Except(newTags).ToList();

                if (tagsToAdd.Any())
                {
                    foreach (var tagId in tagsToAdd)
                    {
                        var tag = db.Tags.SingleOrDefault(x => x.Id == tagId);

                        postModel.PostTags.Add(tag);
                    }
                }

                if (tagsToRemove.Any())
                {
                    foreach (var tagId in tagsToRemove)
                    {
                        var tag = db.Tags.SingleOrDefault(x => x.Id == tagId);

                        postModel.PostTags.Remove(tag);
                    }
                }

                if (postViewModel.Image != null && postViewModel.Image.ContentLength > 0)
                {
                    //var directory = "/Content/Posts/";
                    //var path = Path.Combine(Server.MapPath(directory), postViewModel.Image.FileName);
                    //var url = Path.Combine(directory, postViewModel.Image.FileName);

                    //postViewModel.Image.SaveAs(path);
                    //postModel.Image = url;

                    postModel.Image = UploadPhoto(postViewModel.Image);
                }

                try
                {
                    db.Entry(postModel).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }

                return RedirectToAction("Details", new { id = postModel.Id });
            }

            return View(postViewModel);
        }

        // GET: Blog/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var post = db.Posts.SingleOrDefault(x => x.Id == id);

            if (post == null)
            {
                return HttpNotFound();
            }

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
        public ActionResult DeleteConfirmed(int id)
        {
            var postModel = db.Posts.SingleOrDefault(x => x.Id == id);

            //TODO komentarze

            db.Posts.Remove(postModel);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Comment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var post = db.Posts.Find(id);

            if (post == null)
            {
                return HttpNotFound();
            }

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
            if (ModelState.IsValid)
            {
                var username = HttpContext.User.Identity.GetUserId();

                var post = db.Posts
                    .Include(x => x.Comments)
                    .Single(x => x.Id == commentViewModel.PostId);

                var comment = new CommentModel()
                {
                    Text = commentViewModel.Text,
                    ReleaseDate = DateTime.UtcNow,
                    AuthorId = db.Users.Find(username).Id,
                    Post = post
                };

                post.Comments.Add(comment);

                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = commentViewModel.PostId });
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
            var posts = db.Posts
                .Where(x => x.IsReleased)
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

        private static string UploadPhoto(HttpPostedFileBase image)
        {
            var utility = new AzureBlobHelper();
            var result = utility.UploadBlob(image.FileName, image.InputStream);

            return result != null ? result.Uri.ToString() : string.Empty;
        }
    }
}
