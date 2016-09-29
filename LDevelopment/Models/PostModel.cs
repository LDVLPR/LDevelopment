using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LDevelopment.Models
{
    public class PostModel
    {
        public PostModel()
        {
            PostTags = new List<TagModel>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;
        public bool IsReleased { get; set; } = true;

        public int ViewsCount { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }

        public ICollection<TagModel> PostTags { get; set; }

        public ICollection<CommentModel> Comments { get; set; }
    }
}