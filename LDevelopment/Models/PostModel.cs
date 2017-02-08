using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using LDevelopment.Interfaces;

namespace LDevelopment.Models
{
    public class PostModel : IModel
    {
        public PostModel()
        {
            PostTags = new List<TagModel>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        public bool IsReleased { get; set; } = true;

        public int ViewsCount { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }

        [Required]
        public string Url { get; set; }

        public ICollection<TagModel> PostTags { get; set; }

        public ICollection<CommentModel> Comments { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}