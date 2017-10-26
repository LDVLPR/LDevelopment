using System;
using System.ComponentModel.DataAnnotations;
using LDevelopment.Interfaces;

namespace LDevelopment.Models
{
    public class Comment : IModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [Required]
        public int PostId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual Post Post { get; set; }
    }
}