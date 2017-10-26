using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LDevelopment.Interfaces;

namespace LDevelopment.Models
{
    public class Tag : IModel
    {
        public Tag()
        {
            this.TaggedPosts = new List<Post>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public ICollection<Post> TaggedPosts { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}