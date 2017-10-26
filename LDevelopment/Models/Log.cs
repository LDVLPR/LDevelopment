using System;
using System.ComponentModel.DataAnnotations;
using LDevelopment.Interfaces;

namespace LDevelopment.Models
{
    public class Log : IModel
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string Parameters { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}