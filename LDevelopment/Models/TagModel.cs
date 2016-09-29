using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LDevelopment.Models
{
    public class TagModel
    {
        public TagModel()
        {
            TagPosts = new List<PostModel>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public ICollection<PostModel> TagPosts { get; set; }
    }
}