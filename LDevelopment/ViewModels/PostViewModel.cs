using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace LDevelopment.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]

        public DateTime ReleaseDate { get; set; }

        public bool IsReleased { get; set; } = true;

        public bool HasReadMoreLink { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }
        public string ImageUrl { get; set; }

        public string Url { get; set; }

        public List<string> TagsIds { get; set; }
        public MultiSelectList Tags { get; set; }

        public List<CommentViewModel> Comments { get; set; }
    }
}