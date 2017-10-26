using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using LDevelopment.Interfaces;

namespace LDevelopment.Models
{
    public class Post : IModel
    {
        public Post()
        {
            this.PostTags = new List<Tag>();
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

        [MaxLength(200)]
        [Required]
        public string Url { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public ICollection<Tag> PostTags { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}