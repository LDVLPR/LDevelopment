using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LDevelopment.Interfaces;

namespace LDevelopment.Models
{
    public class TagModel : IModel
    {
        public TagModel()
        {
            TagPosts = new List<PostModel>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public ICollection<PostModel> TagPosts { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}