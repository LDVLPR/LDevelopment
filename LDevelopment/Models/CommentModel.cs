using System;
using System.ComponentModel.DataAnnotations;

namespace LDevelopment.Models
{
    public class CommentModel
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime ReleaseDate { get; set; }

        [Required]
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        [Required]
        public int PostId { get; set; }
        public virtual PostModel Post { get; set; }
    }
}