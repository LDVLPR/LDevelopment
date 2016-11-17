using System;
using System.ComponentModel.DataAnnotations;

namespace LDevelopment.ViewModels
{
    public class CommentViewModel
    {
        [Required]
        public string Text { get; set; } = "";

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        public string AuthorName { get; set; }
        public string AuthorMail { get; set; }

        public int PostId { get; set; }
    }
}