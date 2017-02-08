using System;

namespace LDevelopment.Interfaces
{
    public interface IModel
    {
        int Id { get; set; }

        bool? IsDeleted { get; set; }

        DateTime? DeletedDate { get; set; }
    }
}