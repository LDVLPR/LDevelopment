using System;
using System.Linq;
using System.Linq.Expressions;

namespace LDevelopment.Interfaces
{
    public interface IRepository
    {
        IQueryable<T> All<T>(Expression<Func<T, bool>> predicate = null) where T : class, IModel;

        T Find<T>(int id, params Expression<Func<T, object>>[] includeItems) where T : class, IModel;

        T Find<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeItems) where T : class, IModel;

        void Add<T>(T model) where T : class, IModel;

        void Update<T>(T model) where T : class, IModel;

        void Delete<T>(int id) where T : class, IModel;

        void Save();
    }
}
