using System;

namespace LDevelopment.Models
{
    public class LogModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Parameters { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}